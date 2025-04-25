using BestellFormular.Models.Helper;
using BestellFormular.Models.Window.Product;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace BestellFormular.Models.Window
{
    /// <summary>
    /// Represents a window element, which includes various configurable products and their properties.
    /// </summary>
    public partial class WindowElement : ObservableObject
    {
        /// <summary>
        /// Gets or sets the count of the window elements (default is 1).
        /// </summary>
        [ObservableProperty]
        private int count = 1;

        /// <summary>
        /// Gets or sets the name of the window element.
        /// </summary>
        [ObservableProperty]
        private string name;

        /// <summary>
        /// Gets or sets a string that represents the product position.
        /// </summary>
        [ObservableProperty]
        private string productPositionString;

        [ObservableProperty]
        private GeneralMass generalMass;

        /// <summary>
        /// Gets or sets the aluminum window sill associated with the window element.
        /// </summary>
        [ObservableProperty]
        private AluminumWindowSill aluminumWindowSill;

        /// <summary>
        /// Gets or sets the apron element associated with the window element.
        /// </summary>
        [ObservableProperty]
        private ApronElement apronElement;

        /// <summary>
        /// Gets or sets the Ravisol element associated with the window element.
        /// </summary>
        [ObservableProperty]
        private Ravisol ravisol;

        /// <summary>
        /// Gets or sets the Stuisol element associated with the window element.
        /// </summary>
        [ObservableProperty]
        private Stuisol stuisol;

        /// <summary>
        /// Gets or sets the SupportAngleWithSlope element associated with the window element.
        /// </summary>
        [ObservableProperty]
        private SupportAngleWithSlope supportAngleWithSlope;

        /// <summary>
        /// Gets or sets the SupportAngleWithSlope element associated with the window element.
        /// </summary>
        [ObservableProperty]
        private SupportWedgesWithSlope supportWedgesWithSlope;

        /// <summary>
        /// Gets or sets the collection of column definitions for the window element.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<ColumnDefinition> columnDefinitions;

        /// <summary>
        /// Indicates whether the window element is being initialized.
        /// </summary>
        public bool IsInitializing { get; set; } = false;

        public WindowElement(string name, ExcelLoader excelLoader)
        {
            this.name = name;
            GeneralMass = new GeneralMass(this, excelLoader);
            ApronElement = new ApronElement(this, excelLoader);
            AluminumWindowSill = new AluminumWindowSill(this, excelLoader);
            Ravisol = new Ravisol(this, excelLoader);
            Stuisol = new Stuisol(this, excelLoader);
            SupportAngleWithSlope = new SupportAngleWithSlope(this, excelLoader);
            SupportWedgesWithSlope = new SupportWedgesWithSlope(this, excelLoader);

            // Initialize the column definitions.
            ColumnDefinitions = new ObservableCollection<ColumnDefinition>();
            UpdateColumns();
            RegisterEvents();
        }

        public void RegisterEvents()
        {
            AluminumWindowSill.PropertyChanged += (s, e) => OnSelectedProductChanged(e, s);
            SupportAngleWithSlope.PropertyChanged += (s, e) => OnSelectedProductChanged(e, s);
            SupportWedgesWithSlope.PropertyChanged += (s, e) => OnSelectedProductChanged(e, s);
            ApronElement.PropertyChanged += (s, e) => OnSelectedProductChanged(e, s);
            Ravisol.PropertyChanged += (s, e) => OnSelectedProductChanged(e, s);
            Stuisol.PropertyChanged += (s, e) => OnSelectedProductChanged(e, s);
        }

        partial void OnNameChanged(string? oldValue, string newValue)
        {
            SetPos(newValue);
        }

        private void OnSelectedProductChanged(PropertyChangedEventArgs e, object sender)
        {
            if (e.PropertyName != "Selected") return;

            UpdateColumns();
            SetProductPositionString();
            ResetSurcharge();

            if (sender is ProductBase { Selected: true } productBase)
            {
                ScrollManager.ScrollViewToTop(App.Current.MainPage as ContentPage);
            }
        }

        public void SetPos(string newValue)
        {
            if (!AluminumWindowSill.Pos.Enabled)
            {
                AluminumWindowSill.Pos.Value = newValue;
            }
            if (!SupportAngleWithSlope.Pos.Enabled)
            {
                SupportAngleWithSlope.Pos.Value = newValue;
            }
            if (!SupportWedgesWithSlope.Pos.Enabled)
            {
                SupportWedgesWithSlope.Pos.Value = newValue;
            }
            if (!ApronElement.Pos.Enabled)
            {
                ApronElement.Pos.Value = newValue;
            }
            if (!Ravisol.Pos.Enabled)
            {
                Ravisol.Pos.Value = newValue;
            }
            if (!Stuisol.Pos.Enabled)
            {
                Stuisol.Pos.Value = newValue;
            }
        }

        public void SetEntries()
        {
            AluminumWindowSill.SetEnableEntries(true);
            SupportAngleWithSlope.SetEnableEntries(true);
            SupportWedgesWithSlope.SetEnableEntries(true);
            ApronElement.SetEnableEntries(true);
            Ravisol.SetEnableEntries(true);
            Stuisol.SetEnableEntries(true);
        }

        public void SetGeneralMass()
        {
            GeneralMass.ResetVisability();

            if (!GeneralMass.Selected)
            {
                return;
            }

            var components = GetSelectedComponents();
            foreach (var component in components)
            {
                component.SetGeneralMass(this);
            }
        }

        public void ResetSurcharge()
        {
            AluminumWindowSill.Surcharges = false;
            SupportAngleWithSlope.Surcharges = false;
            SupportWedgesWithSlope.Surcharges = false;
            ApronElement.Surcharges = false;
            Ravisol.Surcharges = false;
            Stuisol.Surcharges = false;
        }

        /// <summary>
        /// Sets the product position string, concatenating product titles and positions.
        /// </summary>
        public void SetProductPositionString()
        {
            var positionStringBuilder = new StringBuilder();

            if (AluminumWindowSill.Selected)
            {
                positionStringBuilder.AppendLine($"{AluminumWindowSill.Product.Titel}\t\t Pos. {AluminumWindowSill.Pos.Value}");
            }
            if (SupportAngleWithSlope.Selected)
            {
                positionStringBuilder.AppendLine($"{SupportAngleWithSlope.Product.Titel}\t\t Pos. {SupportAngleWithSlope.Pos.Value}");
            }
            if (SupportWedgesWithSlope.Selected)
            {
                positionStringBuilder.AppendLine($"{SupportWedgesWithSlope.Product.Titel}\t\t Pos. {SupportWedgesWithSlope.Pos.Value}");
            }
            if (ApronElement.Selected)
            {
                positionStringBuilder.AppendLine($"{ApronElement.Product.Titel}\t\t Pos. {ApronElement.Pos.Value}");
            }
            if (Ravisol.Selected)
            {
                positionStringBuilder.AppendLine($"{Ravisol.Product.Titel}\t Pos. {Ravisol.Pos.Value}");
            }
            if (Stuisol.Selected)
            {
                positionStringBuilder.AppendLine($"{Stuisol.Product.Titel}\t\t Pos. {Stuisol.Pos.Value}");
            }

            ProductPositionString = positionStringBuilder.ToString().TrimEnd();
        }

        /// <summary>
        /// Returns a collection of selected components.
        /// </summary>
        private IEnumerable<ProductBase> GetSelectedComponents()
        {
            var components = new List<ProductBase>();

            if (AluminumWindowSill.Selected) components.Add(AluminumWindowSill);
            if (SupportAngleWithSlope.Selected) components.Add(SupportAngleWithSlope);
            if (SupportWedgesWithSlope.Selected) components.Add(SupportWedgesWithSlope);
            if (ApronElement.Selected) components.Add(ApronElement);
            if (Ravisol.Selected) components.Add(Ravisol);
            if (Stuisol.Selected) components.Add(Stuisol);

            return components;
        }

        public void UpdateLanguage(string language)
        {
            AluminumWindowSill.UpdateFields(language);
            SupportAngleWithSlope.UpdateFields(language);
            SupportWedgesWithSlope.UpdateFields(language);
            ApronElement.UpdateFields(language);
            Ravisol.UpdateFields(language);
            Stuisol.UpdateFields(language);
            GeneralMass.UpdateFields(language);
        }

        public void UpdateShortCut(bool? isOld)
        {
            GeneralMass.UpdateFields(isOld);
            AluminumWindowSill.UpdateFields(isOld);
            SupportAngleWithSlope.UpdateFields(isOld);
            SupportWedgesWithSlope.UpdateFields(isOld);
            ApronElement.UpdateFields(isOld);
            Ravisol.UpdateFields(isOld);
            Stuisol.UpdateFields(isOld);
        }

        public void UpdateColumns()
        {
            // Check if the system is still initializing; if so, exit the method early
            if (IsInitializing)
                return;

            // Collect all possible elements into a list
            var allElements = new List<dynamic>
            {
                GeneralMass,
                AluminumWindowSill,
                SupportAngleWithSlope,
                SupportWedgesWithSlope,
                ApronElement,
                Ravisol,
                Stuisol
                // Add more elements here if needed
            };

            // Filter the list to include only the elements that are selected
            var selectedElements = allElements.Where(element => element?.Selected == true).ToList();

            // Calculate the number of columns, ensuring it stays between 3 and the total number of elements
            int columnCount = Math.Clamp(selectedElements.Count, 3, allElements.Count());

            // Create the column definitions
            ColumnDefinitions = new ObservableCollection<ColumnDefinition>(
                Enumerable.Range(0, columnCount)
                    .Select(_ => new ColumnDefinition { Width = GridLength.Star }) // Each column takes equal space
            );

            // Distribute the selected elements across the available columns
            for (int i = 0; i < selectedElements.Count; i++)
            {
                // Use modulo to wrap around when the column count is exceeded, starting again from column 0
                int column = i % columnCount;
                selectedElements[i].Column = column;
            }

            SetGeneralMass();
        }


        /// <summary>
        /// Creates a copy of the current window element with updated properties.
        /// </summary>
        /// <returns>A new instance of the WindowElement class, which is a copy of the current one.</returns>
        public WindowElement Copy(ExcelLoader excelLoader)
        {
            // Create a copy with a modified name.
            string name = Name + " (Kopie)";
            WindowElement windowElement = new WindowElement(name, excelLoader);

            windowElement.GeneralMass = GeneralMass.Copy(windowElement);

            // Copy the associated elements and reset relevant properties.
            windowElement.AluminumWindowSill = AluminumWindowSill.Copy(windowElement);
            windowElement.AluminumWindowSill.Pos.Value = "";
            windowElement.AluminumWindowSill.Count.Value = "1";

            windowElement.SupportAngleWithSlope = SupportAngleWithSlope.Copy(windowElement);
            windowElement.SupportAngleWithSlope.Pos.Value = "";
            windowElement.SupportAngleWithSlope.Count.Value = "1";

            windowElement.SupportWedgesWithSlope = SupportWedgesWithSlope.Copy(windowElement);
            windowElement.SupportWedgesWithSlope.Pos.Value = "";
            windowElement.SupportWedgesWithSlope.Count.Value = "1";

            windowElement.ApronElement = ApronElement.Copy(windowElement);
            windowElement.ApronElement.Pos.Value = "";
            windowElement.ApronElement.Count.Value = "1";

            windowElement.Ravisol = Ravisol.Copy(windowElement);
            windowElement.Ravisol.Pos.Value = "";
            windowElement.Ravisol.Count.Value = "1";

            windowElement.Stuisol = Stuisol.Copy(windowElement);
            windowElement.Stuisol.Pos.Value = "";
            windowElement.Stuisol.Count.Value = "1";

            // Update the product position string and column definitions.
            windowElement.SetProductPositionString();
            windowElement.RegisterEvents();
            windowElement.ColumnDefinitions = [.. ColumnDefinitions];

            return windowElement;
        }
    }
}
