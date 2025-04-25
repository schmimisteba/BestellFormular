using BestellFormular.Models.Helper;
using BestellFormular.Resources.Language;
using CommunityToolkit.Mvvm.ComponentModel;
using DocumentFormat.OpenXml.Drawing.Charts;
using ExCSS;
using System.Reflection;

namespace BestellFormular.Models.Window.Product

{
    /// <summary>
    /// Represents a Ravisol product, which is a type of window element with configurable properties.
    /// </summary>
    public partial class Ravisol : ProductBase
    {
        /// <summary>
        /// Gets or sets the field for the product.
        /// </summary>
        [ObservableProperty]
        private Field product;

        /// <summary>
        /// Gets or sets the field for the material.
        /// </summary>
        [ObservableProperty]
        private Field material;

        /// <summary>
        /// Gets or sets the field for the thickness of the material.
        /// </summary>
        [ObservableProperty]
        private Field thickness;

        /// <summary>
        /// Gets or sets the field for the height of the product.
        /// </summary>
        [ObservableProperty]
        private Field height;

        /// <summary>
        /// Gets or sets the field for the length of the element.
        /// </summary>
        [ObservableProperty]
        private Field lengthElement;

        /// <summary>
        /// Gets or sets the field for the length of the product.
        /// </summary>
        [ObservableProperty]
        private Field length;

        /// <summary>
        /// Gets or sets the field for the type of the material.
        /// </summary>
        [ObservableProperty]
        private Field typ;


        [ObservableProperty] private Field recessWidth;
        [ObservableProperty] private Field recessDepth;
        [ObservableProperty] private Field aerogel;
        
        [ObservableProperty] private Field bondingBridge;
        [ObservableProperty] private Field uProfil;
        [ObservableProperty] private Field uProfilWide;
        [ObservableProperty] private Field uProfilDepth;
        [ObservableProperty] private Field uProfilMaterial;
        [ObservableProperty] private Field uProfilColor;
        [ObservableProperty] private Field uProfileScrews;


        /// <summary>
        /// Initializes a new instance of the Ravisol class with the provided window element.
        /// </summary>
        /// <param name="windowElement">The window element associated with the product.</param>
        /// <exception cref="ArgumentNullException">Thrown when the window element is null.</exception>
        public Ravisol(WindowElement windowElement) : base(windowElement)
        {
            if (windowElement == null)
                throw new ArgumentNullException(nameof(windowElement), "WindowElement cannot be null");
        }

        /// <summary>
        /// Initializes a new instance of the Ravisol class with the provided window element and Excel helper.
        /// </summary>
        /// <param name="windowElement">The window element associated with the product.</param>
        /// <param name="excelLoader">An Excel helper for retrieving field values.</param>
        /// <exception cref="ArgumentNullException">Thrown when the window element is null.</exception>
        public Ravisol(WindowElement windowElement, ExcelLoader excelLoader) : base(windowElement)
        {
            if (windowElement == null)
                throw new ArgumentNullException(nameof(windowElement), "WindowElement cannot be null");

            InitializeFields(excelLoader);
            RegisterEvents(windowElement);
        }

        /// <summary>
        /// Initializes the fields for the Ravisol product using the provided Excel helper.
        /// </summary>
        /// <param name="excelLoader">The Excel helper to retrieve field values.</param>
        private void InitializeFields(ExcelLoader excelLoader)
        {
            Product = excelLoader.GetFieldById("ID0018");
            Pos = excelLoader.GetFieldById("ID0444");
            Material = excelLoader.GetFieldById("ID0445");
            Count = excelLoader.GetFieldById("ID0446");
            Comment = excelLoader.GetFieldById("ID0130");
            Thickness = excelLoader.GetFieldById("ID0447");
            Height = excelLoader.GetFieldById("ID0448");
            Length = excelLoader.GetFieldById("ID0449");
            LengthElement = excelLoader.GetFieldById("ID0456");
            Typ = excelLoader.GetFieldById("ID0457");
            RecessWidth = excelLoader.GetFieldById("ID0450");
            RecessDepth = excelLoader.GetFieldById("ID0451");
            Aerogel = excelLoader.GetFieldById("ID0452");
            BondingBridge = excelLoader.GetFieldById("ID0453");
            UProfil = excelLoader.GetFieldById("ID0454");
            UProfilWide = excelLoader.GetFieldById("ID0601");
            UProfilDepth = excelLoader.GetFieldById("ID0602");
            UProfilMaterial = excelLoader.GetFieldById("ID0600");
            UProfilColor = excelLoader.GetFieldById("ID0178");
            UProfileScrews = excelLoader.GetFieldById("ID0455");
        }

        /// <summary>
        /// Registers event handlers for various field changes.
        /// </summary>
        /// <param name="windowElement">The window element associated with the product.</param>
        private void RegisterEvents(WindowElement windowElement)
        {
            RegisterPosChangeHandler(Pos);
            RegisterMaterialChangeHandler(UProfilMaterial, UProfilColor, new[] { "PB" });
            RegisterMaterialChangeHandler(UProfil, UProfilDepth, new[] { Resource.Yes });
            RegisterMaterialChangeHandler(UProfil, UProfilWide, new[] { Resource.Yes });

            //Disable automatisch berechnen
            Pos.PropertyChanged += (s, e) => EventCalculator.CalculateAll(e, windowElement);
            Length.PropertyChanged += (s, e) => EventCalculator.CalculateAll(e, windowElement);
            Height.PropertyChanged += (s, e) => EventCalculator.CalculateAll(e, windowElement);

            Material.PropertyChanged += (s, e) => HandleMaterialChange(e);
            Material.PropertyChanged += (s, e) => HandleSystemElementMassPropertyChange(e);
            Length.PropertyChanged += (s, e) => HandleSystemElementMassPropertyChange(e);
            Typ.PropertyChanged += (s, e) => HandleSystemElementMassPropertyChange(e);

            RecessWidth.PropertyChanged += (s, e) => ClearSurchargeChangeHandler(e, RecessWidth, [RecessWidth, RecessDepth]);
            Aerogel.PropertyChanged += (s, e) => ClearSurchargeChangeHandler(e, Aerogel, [Aerogel], Resource.No);
            BondingBridge.PropertyChanged += (s, e) => ClearSurchargeChangeHandler(e, BondingBridge, [BondingBridge], Resource.No);

            UProfil.PropertyChanged += (s, e) => ClearSurchargeChangeHandler(e, UProfil, [UProfil, UProfileScrews, UProfilWide, UProfilDepth, UProfilMaterial, UProfilColor]);
        }

        /// <summary>
        /// Handles changes to the material property and updates the type field accordingly.
        /// </summary>
        /// <param name="e">The event arguments for the property change.</param>
        private void HandleMaterialChange(System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Field.Value))
            {
                Typ.Selected = Material.Value == "XPS" ? true : false;
            }
        }

        /// <summary>
        /// Handles general property changes and updates target field selection.
        /// </summary>
        /// <param name="e">Property change event arguments.</param>
        /// <param name="value">The updated field value.</param>
        /// <param name="targetField">The field to update.</param>
        /// <param name="trueValues">The set of values that indicate a true state.</param>
        private void HandleSystemElementMassPropertyChange(System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Field.Value))
            {
                var material = StringExtensions.GetBetween(Material.Value, "[", "]") ?? Material.Value;
                var typ = StringExtensions.GetBetween(Typ.Value, "[", "]") ?? Typ.Value;

                if (material == "XPS")
                {
                    if (typ == "SL")
                    {
                        LengthElement.Value = "1250";
                    }
                    else
                    {
                        if (ParseCount(LengthElement.Value) > 2500)
                        {
                            LengthElement.Value = Length.Value;
                        }
                        else
                        {
                            LengthElement.Value = "2500";
                        }
                    }
                }
                else
                {
                    LengthElement.Value = "1000";
                }
                LengthElement.Selected = true;
            }
        }


        /// <summary>
        /// Creates a copy of the current Ravisol instance.
        /// </summary>
        /// <param name="windowElement">The window element associated with the copied product.</param>
        /// <returns>A new instance of Ravisol with the same properties as the current one.</returns>
        public override Ravisol Copy(WindowElement windowElement)
        {
            var copy = new Ravisol(windowElement)
            {
                Product = this.Product?.Copy(),
                Pos = this.Pos?.Copy(), // 🔹 Creates a new copy of Field
                Count = this.Count?.Copy(),
                Column = this.Column,
                Selected = this.Selected,
                Material = this.Material?.Copy(),
                Comment = this.Comment?.Copy(),
                Thickness = this.Thickness?.Copy(),
                Height = this.Height?.Copy(),
                Length = this.Length?.Copy(),
                LengthElement = this.LengthElement?.Copy(),
                Typ = Typ.Copy(),
                RecessWidth = RecessWidth.Copy(),
                RecessDepth = RecessDepth.Copy(),
                Aerogel = this.Aerogel?.Copy(),
                BondingBridge = this.BondingBridge?.Copy(),
                UProfil = this.UProfil?.Copy(),
                UProfilWide = this.UProfilWide?.Copy(),
                UProfilDepth = this.UProfilDepth?.Copy(),
                UProfilMaterial = this.UProfilMaterial?.Copy(),
                UProfilColor = this.UProfilColor?.Copy(),
                UProfileScrews = this.UProfileScrews?.Copy()
            };

            copy.RegisterEvents(windowElement);
            return copy;
        }

        /// <summary>
        /// Updates the language for all fields in the product.
        /// </summary>
        /// <param name="language">The language to update the fields to.</param>
        public void UpdateFields(string language)
        {
            var properties = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(Field))
                {
                    var field = property.GetValue(this) as Field;
                    field?.UpdateLanguage(language);
                }
            }
        }

        public static void CountToElement(List<Ravisol> products)
        {
            foreach (Ravisol supportAngleWithSlope in products)
            {
                double length = ParseCount(supportAngleWithSlope.Length.Value);
                double elementLength = ParseCount(supportAngleWithSlope.LengthElement.Value);
                double count = ParseCount(supportAngleWithSlope.Count.Value);

                if (length < elementLength)
                {
                    supportAngleWithSlope.Count.Value = (1 * count).ToString();
                }
                else
                {
                    supportAngleWithSlope.Count.Value = Math.Ceiling(length / elementLength * count).ToString();
                }

            }
        }

        protected override string GenerateFilterKey()
        {
            return $"{Product.Value}{Pos.Value}{Material.Value}{Thickness.Value}{Height.Value}{Length.Value}{Typ.Value}";
        }

        public override void SetGeneralMass(WindowElement windowElement)
        {
            windowElement.GeneralMass.WindowLightWidth.Selected = true;
            windowElement.GeneralMass.FrameWidthTop.Selected = true;

            SetEnableEntries(false);
        }

        public override void SetEnableEntries(bool enable)
        {
            Pos.Enabled = enable;
            Length.Enabled = enable;
            Height.Enabled = enable;
        }

        public void CalculateLenght(string windowLightWidth, string visibleFrameWidthLeft, string visibleFrameWidthRight)
        {
            int windowLight = ParseCount(windowLightWidth);
            int visibleLeft = ParseCount(visibleFrameWidthLeft);
            int visibleRight = ParseCount(visibleFrameWidthRight);

            if (!Length.Enabled)
            {
                Length.Value = (windowLight + visibleLeft + visibleRight).ToString();
            } 
        }

        public void CalculateHeight(string frameWidthTop, string visibleFrameWidthTop)
        {
            if (!Height.Enabled)
            {
                Height.Value = (ParseCount(frameWidthTop) - ParseCount(visibleFrameWidthTop)).ToString();
            }
        }
    }
}
