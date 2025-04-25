using BestellFormular.Models.AddressHead;
using BestellFormular.Models.Helper;
using BestellFormular.Models.Manager;
using BestellFormular.Models.Window;
using BestellFormular.Resources.Language;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Globalization;

namespace BestellFormular.Models
{
    public partial class BestellFormularController : ObservableObject
    {
        private static readonly string BasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Dosteba", "BestellFormular");

        //private static readonly string IdExcelPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "backup", "id.xlsx");
        private static readonly string IdExcelPath = Path.Combine(BasePath, "id.xlsx");
        private static readonly string AddressExcelPath = Path.Combine(BasePath, "addresses.xlsx");
        private static readonly string BackupFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Dosteba", "BestellFormular", "BestellformularBackup.xlsx");

        private static BestellFormularController? _instance;
        private ExcelLoader excelLoader;

        private const string Version = "V_1.5";
        private const string Titel = "Bestellformular";
        private const string defaultCulture = "de-CH";

        //private string currentCulture = defaultCulture;
        private string currentCulture;

        private string currentFilePath = BackupFilePath;
        private DateTime lastTimeSaved = DateTime.Now;

        public static BestellFormularController Instance => _instance ??= new BestellFormularController();

        private BestellFormularController()
        {
            excelLoader = new ExcelLoader(IdExcelPath, AddressExcelPath);
            currentCulture = LanguageHandler.GetLanguageFromCulture();
        }

        public string GetWindowTitle() => $"{Titel} | {Version} | {currentFilePath} | {lastTimeSaved}";

        public string GetVersion() => $"{Titel} | {Version}";

        public string GetCurrentFilePath()
        {
            if (currentFilePath == BackupFilePath)
            {
                return BackupFilePath;
            }
            else
            {
                return currentFilePath;
            }
        }
        public string GetBackupFilePath() => BackupFilePath;
        public static void CreateInstance()
        {
            if (_instance == null)
            {
                _instance = new BestellFormularController();
            }
        }

        public AdressHead CreateDefaultAdressHead()
        {
            return new AdressHead(excelLoader);
        }
        public ButtonsController CreateButtonController()
        {
            ButtonsController buttonsController = new ButtonsController(excelLoader);
            buttonsController.UpdateLanguage(currentCulture);
            return buttonsController;
        }

        public void UpdateLanguage(ButtonsController buttonsController, AdressHead adressHead, ObservableCollection<WindowElement> elements)
        {
            UpdateErrorMessageLanguage();
            UpdateElementsLanguage(elements);
            buttonsController.UpdateLanguage(currentCulture);
            adressHead.UpdateLanguage(currentCulture);
        }

        private void UpdateErrorMessageLanguage()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(currentCulture);
        }

        private void UpdateElementsLanguage(ObservableCollection<WindowElement> elements)
        {
            foreach (var element in elements)
            {
                element.UpdateLanguage(currentCulture);
                element.SetProductPositionString();
            }
        }

        public void UpdateShortCut(ObservableCollection<WindowElement> elements, bool? isOld)
        {
            foreach (var element in elements)
            {
                element.UpdateShortCut(isOld);
            }
        }

        public WindowElement CreateDefaultElement(ObservableCollection<WindowElement> elements, bool? expertModus)
        {
            WindowElement selectedWindow = new WindowElement((elements.Count + 1).ToString(), excelLoader);
            elements.Add(selectedWindow);
            UpdateExpertModus(elements, expertModus);
            UpdateElementsLanguage(elements);
            return selectedWindow;
        }

        public WindowElement CopyProduct(WindowElement? element, ObservableCollection<WindowElement> elements)
        {
            if (element is null ||
                elements is null) return null;

            var copiedProduct = element.Copy(excelLoader);
            if (copiedProduct is not null)
            {
                elements.Add(copiedProduct);
            }
            return copiedProduct;
        }

        public WindowElement? RemoveProduct(WindowElement element, ObservableCollection<WindowElement> elements)
        {
            int index = elements.IndexOf(element);
            elements.Remove(element);

            return elements.Count > 0
                ? elements.ElementAtOrDefault(Math.Max(0, index - 1))
                : null;

        }

        public void IncrementElementCount(WindowElement windowElement, int delta)
        {
            if (windowElement == null) return;
            windowElement.Count += delta;
        }

        public string ExportToExcel(ObservableCollection<WindowElement> elements, AdressHead adressHead, string path)
        {
            // Aktuellen Sprachstatus speichern
            string previousCulture = currentCulture;

            try
            {
                currentCulture = defaultCulture;
                // Auf Deutsch umstellen
                UpdateLanguage(new ButtonsController(excelLoader), adressHead, elements);

                // Export durchführen
                ExcelHelper.ExportTocExcel(elements.ToList(), adressHead, path);

                // Dateipfad und Speicherzeit aktualisieren
                currentFilePath = path;
                lastTimeSaved = DateTime.Now;

                return GetWindowTitle();
            }
            finally
            {
                // Sprache nur zurücksetzen, wenn es notwendig ist
                if (previousCulture != defaultCulture)
                {
                    currentCulture = previousCulture;
                    UpdateLanguage(new ButtonsController(excelLoader), adressHead, elements);
                }
            }
        }

        /// <summary>
        /// Importiert Daten aus einer Excel-Datei und bereitet sie für die Anwendung vor.
        /// </summary>
        /// <param name="path">Der Dateipfad zur Excel-Datei.</param>
        /// <returns>
        /// Ein Tupel bestehend aus importierten Elementen und Metadaten.
        /// </returns>
        public (ObservableCollection<WindowElement> Elements,
                WindowElement? SelectedElement,
                AdressHead Header,
                string Title) ImportFromExcel(string path)
        {
            // Prüfung des Pfads
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path), "Der Dateipfad darf nicht null oder leer sein.");
            }

            // Dateipfad aktualisieren
            currentFilePath = path;
            lastTimeSaved = DateTime.Now;

            // Daten aus Excel importieren
            var (importedWindows, importedAdressHead) = ExcelHelper.ImportFromExcel(path, excelLoader);

            // Sicherstellen, dass die importierten Objekte nicht null sind
            if (importedWindows == null)
                importedWindows = new List<WindowElement>();

            if (importedAdressHead == null)
                importedAdressHead = new AdressHead(excelLoader);
            else
                importedAdressHead.SetSelection(false);

            // ObservableCollection erstellen für UI-Binding
            var windowElements = new ObservableCollection<WindowElement>(importedWindows);

            UpdateElementsLanguage(windowElements);
            importedAdressHead.UpdateLanguage(currentCulture);

            // Ergebnis als benanntes Tupel zurückgeben, mit expliziter Null-Behandlung
            return (
                Elements: windowElements,
                SelectedElement: windowElements.Count > 0 ? windowElements[0] : null,
                Header: importedAdressHead,
                Title: GetWindowTitle() ?? string.Empty
            );
        }

        public void SendEmail(AdressHead adressHead)
        {
            string attachmentPath = BestellFormularController.Instance.GetCurrentFilePath();

            Task.Run(() =>
            {
                dynamic outlookApp = Activator.CreateInstance(Type.GetTypeFromProgID("Outlook.Application"));

                dynamic mailItem = outlookApp.CreateItem(0);

                mailItem.Subject = FileHelper.GetFileNameFromAdressHead(adressHead);

                mailItem.To = "dosteba@dosteba.ch";

                mailItem.Attachments.Add(attachmentPath);

                mailItem.Display(true);
            });
        }

        internal async void OpenFile()
        {
            if (File.Exists(currentFilePath))
            {
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = currentFilePath,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    await ErrorManager.HandleErrorMessage(Resource.OpenFile, ex, true);
                }
            }
            else
            {
                await ErrorManager.HandleErrorMessage(Resource.FileNotExist);
            }
        }

        public async Task<bool> ConfirmSaveAsync()
        {
            try
            {
                bool answer = await PopUpManager.ShowSavePopup(Resource.SaveTitel, Resource.Save);
                return answer;
            }
            catch (Exception ex)
            {
                await ErrorManager.HandleErrorMessage(Resource.Error, ex, true);
                return false;
            }
        }

        internal void UpdateExpertModus(ObservableCollection<WindowElement> windowElements, bool? set)
        {
            UpdateShortCut(windowElements, set);
            if (set.Value)
            {
                foreach (WindowElement element in windowElements)
                {
                    element.GeneralMass.Selected = true;
                    element.UpdateColumns();
                }
            }
            else
            {
                foreach (WindowElement element in windowElements)
                {
                    element.GeneralMass.Selected = false;
                    element.UpdateColumns();
                    element.SetEntries();
                }
            }
        }
    }
}
