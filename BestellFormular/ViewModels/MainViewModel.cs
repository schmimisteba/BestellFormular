using BestellFormular.Models;
using BestellFormular.Models.AddressHead;
using BestellFormular.Models.Helper;
using BestellFormular.Models.Manager;
using BestellFormular.Models.Window;
using BestellFormular.Resources.Language;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace BestellFormular.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        public MainViewModel()
        {
            Elements = new ObservableCollection<WindowElement>();
            AdressHead = BestellFormularController.Instance.CreateDefaultAdressHead();
            ButtonsController = BestellFormularController.Instance.CreateButtonController();
            WindowTitle = BestellFormularController.Instance.GetWindowTitle();
            Language = LanguageHandler.GetLanguageFromCulture();
            LoadInitialData();
        }


        [ObservableProperty]
        private ImageSource source;

        [ObservableProperty]
        private int imageHeight;

        [ObservableProperty]
        private int imageWidth;

        [ObservableProperty]
        private string language;

        [ObservableProperty]
        private string windowTitle;

        [ObservableProperty]
        private ObservableCollection<WindowElement> elements;

        [ObservableProperty]
        private WindowElement selectedElement;

        [ObservableProperty]
        private AdressHead adressHead;

        [ObservableProperty]
        private ButtonsController buttonsController;

        [ObservableProperty]
        private PathF drawingPath = new PathF();

        [ObservableProperty]
        private bool imageVisible = false;

        [ObservableProperty]
        private bool adressVisible = false;

        [ObservableProperty]
        private bool productVisible = false;

        [ObservableProperty]
        private bool skizzeVisible = false;

        [ObservableProperty]
        private bool filesMenuVisible = false;

        partial void OnSelectedElementChanged(WindowElement? oldValue, WindowElement newValue)
        {
            ScrollManager.ScrollViewToTop((App.Current.MainPage as ContentPage));
        }

        private async Task LoadInitialData()
        {
            try
            {
                AdressVisible = true;
                (Elements, SelectedElement, AdressHead, WindowTitle) = BestellFormularController.Instance.ImportFromExcel(BestellFormularController.Instance.GetCurrentFilePath());
                ButtonsController.ExpertModus.Selected = SelectedElement?.GeneralMass?.Selected ?? false;
                BestellFormularController.Instance.UpdateExpertModus(Elements, ButtonsController.ExpertModus.Selected);
            }
            catch (Exception ex)
            {
                await ErrorManager.HandleErrorMessage(Resource.LoadErrorMessage, ex);
            }
        }

        [RelayCommand]
        private void ToggleCheckbox(ProductBase prod)
        {     
            prod.Selected = !prod.Selected;
        }

        [RelayCommand]
        private void ShowFilesMenu()
        {
            FilesMenuVisible = !FilesMenuVisible;
        }

        [RelayCommand]
        private void UpdateImage(Field selectedField)
        {
            (ImageVisible, Source, ImageHeight, ImageWidth) = ImageLoader.LoadImage(selectedField.Image);
        }

        [RelayCommand]
        private void CloseImage()
        {
            Source = null;
            ImageVisible = false;
        }

        [RelayCommand]
        private void CopyWindow()
        {
            BestellFormularController.Instance.IncrementElementCount(SelectedElement, 1);
        }

        [RelayCommand]
        private void RemoveWindow()
        {
            if (SelectedElement != null && SelectedElement.Count > 1)
            {
                BestellFormularController.Instance.IncrementElementCount(SelectedElement, -1);
            }
        }

        [RelayCommand]
        private async void AddElement()
        {
            SelectedElement = BestellFormularController.Instance.CreateDefaultElement(Elements, ButtonsController.ExpertModus.Selected);
            ScrollManager.ListViewToSelected();
        }

        [RelayCommand]
        private async void RemoveElement()
        {
            bool answer = await PopUpManager.ShowConfirmationPopup(Resource.RemoveElementTitle, Resource.RemoveElementMessage);

        if (answer)
            {
                SelectedElement = BestellFormularController.Instance.RemoveProduct(SelectedElement, Elements);
                if (SelectedElement == null)
                {
                    ShowAdress();
                }
            }
        }

        [RelayCommand]
        private void CopyElement()
        {
            SelectedElement = BestellFormularController.Instance.CopyProduct(SelectedElement, Elements);
            ScrollManager.ListViewToSelected();
        }

        [RelayCommand]
        private void SetExpertModus()
        {
            ButtonsController.ExpertModus.Selected = !ButtonsController.ExpertModus.Selected;
            BestellFormularController.Instance.UpdateExpertModus(Elements, ButtonsController.ExpertModus.Selected);
            FilesMenuVisible = false;
            ImageVisible = false;
        }

        [RelayCommand]
        private void ResetSurcharge()
        {
            if (SelectedElement != null)
            {
                SelectedElement.ResetSurcharge();
            }
        }

        private ProductBase? _lastSelectedProduct;

        [RelayCommand]
        private void ToggleSurcharge(ProductBase selectedProduct)
        {
            if (_lastSelectedProduct != null && _lastSelectedProduct != selectedProduct)
            {
                ResetSurcharge();
            }

            selectedProduct.Surcharges = !selectedProduct.Surcharges;
            _lastSelectedProduct = selectedProduct;
        }


        [RelayCommand]
        private void ShowSurcharge(Field selectedField)
        {
            selectedField.Selected = !selectedField.Selected;
        }

        [RelayCommand]
        private void ShowProduct()
        {
            if (Elements.Count == 0)
            {
                AddElementCommand.Execute(null);
            }
            ProductVisible = true;
            AdressVisible = false;
            FilesMenuVisible = false;

            ScrollManager.ScrollViewToTop((App.Current.MainPage as ContentPage));
        }

        [RelayCommand]
        private void ShowAdress()
        {
            AdressVisible = true;
            ProductVisible = false;
            FilesMenuVisible = false;

            ScrollManager.ScrollViewToTop((App.Current.MainPage as ContentPage));
        }

        [RelayCommand]
        private void ShowSkizze()
        {
            FilesMenuVisible = false;
            FilesMenuVisible = false;
        }

        [RelayCommand]
        private async void NewOrder()
        {
            try
            {
                if (Elements.Count > 0)
                {
                    if (await BestellFormularController.Instance.ConfirmSaveAsync())
                    {
                        await Save();
                    }
                }
                Elements.Clear();
                AdressHead = BestellFormularController.Instance.CreateDefaultAdressHead();
                WindowTitle = BestellFormularController.Instance.GetWindowTitle();
                ShowAdress();
            }
            catch (Exception ex)
            {
                await ErrorManager.HandleErrorMessage(Resource.NewOrderErrorMessage, ex, true);
            }
            finally
            {
                FilesMenuVisible = false;
            }
        }

        [RelayCommand]
        private void Clear()
        {
            DrawingPath = new PathF();
        }

        [RelayCommand]
        private async Task Load()
        {
            try
            {
                if (Elements.Count > 0)
                {
                    if (await BestellFormularController.Instance.ConfirmSaveAsync())
                    {
                        await Save();
                    }
                }

                var fileResult = await FileHelper.GetFilePathFromFilePicker();

                if (fileResult == null)
                {
                    return;
                }

                (Elements, SelectedElement, AdressHead, WindowTitle) = BestellFormularController.Instance.ImportFromExcel(fileResult.FullPath);
                if (SelectedElement != null)
                {
                    ButtonsController.ExpertModus.Selected = SelectedElement.GeneralMass.Selected;
                    ShowProduct();
                }
            }
            catch (Exception ex)
            {
                await ErrorManager.HandleErrorMessage(Resource.LoadErrorMessage, ex, true);
            }
            finally
            {
                FilesMenuVisible = false;
            }
        }

        [RelayCommand]
        private async Task SaveAs()
        {
            try
            {
                var folderResult = await FileHelper.GetFilePathFromFileSaver(AdressHead);

                if (folderResult == null)
                {
                    return;
                }

                WindowTitle = BestellFormularController.Instance.ExportToExcel(Elements, AdressHead, folderResult.FilePath);
            }
            catch (Exception ex)
            {
                await ErrorManager.HandleErrorMessage(Resource.SaveErrorMessage, ex, true);
            }
            finally
            {
                FilesMenuVisible = false;
            }
        }

        [RelayCommand]
        private async Task Save()
        {
            try
            {
                if (BestellFormularController.Instance.GetCurrentFilePath() != BestellFormularController.Instance.GetBackupFilePath())
                {
                    WindowTitle = BestellFormularController.Instance.ExportToExcel(Elements, AdressHead, BestellFormularController.Instance.GetCurrentFilePath());
                }
                else
                {
                    await SaveAs();
                }
                CloseError();
            }
            catch (Exception ex)
            {
                await ErrorManager.HandleErrorMessage(Resource.SaveErrorMessage, ex, true);
            }
            finally
            {
                FilesMenuVisible = false;
            }
        }

        [RelayCommand]
        private async Task Send()
        {
            try
            {
                if (!AdressHead.IsAddressHeadValid(Language))
                {
                    return;
                }

                if (await BestellFormularController.Instance.ConfirmSaveAsync())
                {
                    await Save();
                }

                BestellFormularController.Instance.SendEmail(AdressHead);
            }
            catch (Exception ex)
            {
                await ErrorManager.HandleErrorMessage(Resource.SendErrorMessage, ex, true);
            }
            finally
            {
                FilesMenuVisible = false;
            }
        }

        [RelayCommand]
        private async void OpenFile()
        {
            if (await BestellFormularController.Instance.ConfirmSaveAsync())
            {
                await Save();
            }

            BestellFormularController.Instance.OpenFile();
            FilesMenuVisible = false;
        }

        [RelayCommand]
        private void CloseError()
        {
            ErrorManager.ClearError();
        }
    }
}
