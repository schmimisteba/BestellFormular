using BestellFormular.Models.Helper;
using BestellFormular.Models.Manager;
using BestellFormular.Resources.Language;
using CommunityToolkit.Mvvm.ComponentModel;
using ExCSS;
using System.ComponentModel;
using System.Reflection;

namespace BestellFormular.Models.Window.Product
{
    /// <summary>
    /// Represents an apron element with various configurable properties for window installations.
    /// </summary>
    public partial class ApronElement : ProductBase
    {
        // Observable properties for various apron element characteristics
        [ObservableProperty] private Field system;
        [ObservableProperty] private Field typ;
        [ObservableProperty] private Field nicheLength;
        [ObservableProperty] private Field whideSurfaceIsolLeft;
        [ObservableProperty] private Field whideSurfaceIsolRight;
        [ObservableProperty] private Field apronThickness;
        [ObservableProperty] private Field thickness;
        [ObservableProperty] private Field apronHeight;
        [ObservableProperty] private Field nicheHeight;
        [ObservableProperty] private Field apronLength;
        [ObservableProperty] public Field mountingBracketEPS;
        [ObservableProperty] public Field insertForFixingBlinds;
        [ObservableProperty] private bool hasMountingBracket;
        [ObservableProperty] public Field execution;
        [ObservableProperty] private bool hasSheetMetalBackWallForClinkerBrickFacade;
        [ObservableProperty] public Field sheetMetalBackWallForClinkerBrickFacadeBS;
        [ObservableProperty] public Field sheetMetalBackWallForClinkerBrickFacadeVBLUR;
        [ObservableProperty] public Field sheetMetalBackWallForClinkerBrickFacadeVBLFS;
        [ObservableProperty] public Field connector;

        /// <summary>
        /// Initializes a new instance of the ApronElement class.
        /// </summary>
        /// <param name="windowElement">The window element associated with this apron element.</param>
        /// <exception cref="ArgumentNullException">Thrown when windowElement is null.</exception>
        public ApronElement(WindowElement windowElement) : base(windowElement)
        {
            ValidateWindowElement(windowElement);
        }

        /// <summary>
        /// Initializes a new instance of the ApronElement class with Excel helper.
        /// </summary>
        /// <param name="windowElement">The window element associated with this apron element.</param>
        /// <param name="excelLoader">Helper for retrieving field data from Excel.</param>
        /// <exception cref="ArgumentNullException">Thrown when windowElement is null.</exception>
        public ApronElement(WindowElement windowElement, ExcelLoader excelLoader) : base(windowElement)
        {
            ValidateWindowElement(windowElement);
            InitializeFields(excelLoader);
            RegisterEvents(windowElement);
        }

        /// <summary>
        /// Validates that the window element is not null.
        /// </summary>
        private void ValidateWindowElement(WindowElement windowElement)
        {
            if (windowElement == null)
                throw new ArgumentNullException(nameof(windowElement), "WindowElement cannot be null");
        }

        /// <summary>
        /// Initializes fields using the Excel helper.
        /// </summary>
        private void InitializeFields(ExcelLoader excelLoader)
        {
            Product = excelLoader.GetFieldById("ID0017");
            Pos = excelLoader.GetFieldById("ID0426");
            Count = excelLoader.GetFieldById("ID0111");
            Comment = excelLoader.GetFieldById("ID0130");
            System = excelLoader.GetFieldById("ID0102");
            Typ = excelLoader.GetFieldById("ID0427");
            Thickness = excelLoader.GetFieldById("ID0430");
            ApronHeight = excelLoader.GetFieldById("ID0428");
            ApronThickness = excelLoader.GetFieldById("ID0431");
            ApronLength = excelLoader.GetFieldById("ID0435");
            NicheHeight = excelLoader.GetFieldById("ID0429");
            NicheLength = excelLoader.GetFieldById("ID0432");
            WhideSurfaceIsolLeft = excelLoader.GetFieldById("ID0433");
            WhideSurfaceIsolRight = excelLoader.GetFieldById("ID0434");
            InsertForFixingBlinds = excelLoader.GetFieldById("ID0436");
            MountingBracketEPS = excelLoader.GetFieldById("ID0437");
            Execution = excelLoader.GetFieldById("ID0438");
            SheetMetalBackWallForClinkerBrickFacadeBS = excelLoader.GetFieldById("ID0439");
            SheetMetalBackWallForClinkerBrickFacadeVBLUR = excelLoader.GetFieldById("ID0440");
            SheetMetalBackWallForClinkerBrickFacadeVBLFS = excelLoader.GetFieldById("ID0441");
            Connector = excelLoader.GetFieldById("ID0442");
        }

        /// <summary>
        /// Registers event handlers for various field changes.
        /// </summary>
        private void RegisterEvents(WindowElement windowElement)
        {
            //Disable automatisch berechnen
            Pos.PropertyChanged += (s, e) => EventCalculator.CalculateAll(e, windowElement);
            NicheLength.PropertyChanged += (s, e) => EventCalculator.CalculateAll(e, windowElement);
            Thickness.PropertyChanged += (s, e) => EventCalculator.CalculateAll(e, windowElement);
            ApronLength.PropertyChanged += (s, e) => EventCalculator.CalculateAll(e, windowElement);

            Typ.PropertyChanged += (s, e) => HandleTypChange(e, Typ.Value);
            MountingBracketEPS.PropertyChanged += (s, e) => HandleMountingBracketEPSChange(e, MountingBracketEPS.Value);
            ApronThickness.PropertyChanged += (s, e) => EventCalculator.CalculateAll(e, windowElement);

            InsertForFixingBlinds.PropertyChanged += (s, e) => ClearSurchargeChangeHandler(e, InsertForFixingBlinds, [InsertForFixingBlinds], Resource.No);
            SheetMetalBackWallForClinkerBrickFacadeBS.PropertyChanged += (s, e) => ClearSurchargeChangeHandler(e, SheetMetalBackWallForClinkerBrickFacadeBS, [SheetMetalBackWallForClinkerBrickFacadeBS, SheetMetalBackWallForClinkerBrickFacadeVBLUR, SheetMetalBackWallForClinkerBrickFacadeVBLFS]);

            SheetMetalBackWallForClinkerBrickFacadeVBLFS.PropertyChanged += (s, e) => SheetMetalBackWallForClinkerBrickFacadeHandler(e);
            SheetMetalBackWallForClinkerBrickFacadeVBLUR.PropertyChanged += (s, e) => SheetMetalBackWallForClinkerBrickFacadeHandler(e);
            Connector.PropertyChanged += (s, e) => ClearSurchargeChangeHandler(e, Connector, [Connector], Resource.No);

            RegisterPosChangeHandler(Pos);
            RegisterApronLengthHandlers();
        }


        /// <summary>
        /// Registers event handlers for apron length-related fields.
        /// </summary>
        private void RegisterApronLengthHandlers()
        {
            NicheLength.PropertyChanged += (s, e) => HandleApronLength(e);
            WhideSurfaceIsolLeft.PropertyChanged += (s, e) => HandleApronLength(e);
            WhideSurfaceIsolRight.PropertyChanged += (s, e) => HandleApronLength(e);
        }

        /// <summary>
        /// Calculates the apron length based on niche length and surface isolation values.
        /// </summary>
        private void SheetMetalBackWallForClinkerBrickFacadeHandler(PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Field.Value))
            {
                // Nur eines von beiden darf Inhalt haben, sonst wird eine Exception erstellt
                bool hasVBLFS = !string.IsNullOrEmpty(SheetMetalBackWallForClinkerBrickFacadeVBLFS.Value);
                bool hasVBLUR = !string.IsNullOrEmpty(SheetMetalBackWallForClinkerBrickFacadeVBLUR.Value);

                if (hasVBLFS && hasVBLUR || !hasVBLFS && !hasVBLUR)
                {
                    ErrorManager.HandleErrorMessage(Resource.SheetMetalError);
                }
            }
        }

        /// <summary>
        /// Calculates the apron length based on niche length and surface isolation values.
        /// </summary>
        private void HandleApronLength(PropertyChangedEventArgs e)
        {
            int nicheValue = int.TryParse(NicheLength.Value, out int parsedNicheValue) ? parsedNicheValue : 0;
            int leftValue = int.TryParse(WhideSurfaceIsolLeft.Value, out int parsedLeftValue) ? parsedLeftValue : 0;
            int rightValue = int.TryParse(WhideSurfaceIsolRight.Value, out int parsedRightValue) ? parsedRightValue : 0;

            if (!ApronLength.Enabled)
            {
                ApronLength.Value = (nicheValue + leftValue + rightValue).ToString();
            }
        }

        /// <summary>
        /// Registers an event handler for the Pos field.
        /// </summary>
        private void HandleMountingBracketEPSChange(PropertyChangedEventArgs e, string value)
        {
            // Execution gets selected if MountingBrachetEPSChange value ist "Blech"
            if (e.PropertyName == nameof(Field.Value))
            {
                value = StringExtensions.GetBetween(value, "[", "]") ?? value;
                Execution.Selected = value == "Blech" ? true : false;
            }
        }

        /// <summary>
        /// Handles changes in the Typ field, updating selected states for various fields.
        /// </summary>
        /// <param name="e">Property changed event arguments.</param>
        /// <param name="value">The new value of the Typ field.</param>
        private void HandleTypChange(System.ComponentModel.PropertyChangedEventArgs e, string value)
        {
            if (e.PropertyName == nameof(Field.Value))
            {
                value = StringExtensions.GetBetween(value, "[", "]") ?? value;
                var state = value switch
                {
                    "S" => (true, true, true, true, true, true, true, true, false),
                    "O" => (false, false, false, true, false, true, false, true, true),
                    "A" => (false, false, false, true, true, true, true, true, false),
                    _ => (false, false, false, false, false, false, false, false, false)
                };

                (NicheLength.Selected, WhideSurfaceIsolLeft.Selected, WhideSurfaceIsolRight.Selected,
                ApronThickness.Selected, Thickness.Selected, ApronHeight.Selected, NicheHeight.Selected,
                ApronLength.Selected, MountingBracketEPS.Selected) = state;
            }
        }

        /// <summary>
        /// Creates a deep copy of the ApronElement with a new window element.
        /// </summary>
        /// <param name="windowElement">The new window element for the copied instance.</param>
        /// <returns>A new ApronElement instance with copied properties.</returns>
        public override ApronElement Copy(WindowElement windowElement)
        {
            var copy = new ApronElement(windowElement)
            {
                Product = this.Product?.Copy(),
                Pos = this.Pos?.Copy(),
                Count = this.Count?.Copy(),
                Selected = this.Selected,
                Column = this.Column,
                Typ = this.Typ?.Copy(),
                System = this.System?.Copy(),
                Thickness = this.Thickness?.Copy(),
                ApronHeight = this.ApronHeight?.Copy(),
                ApronThickness = this.ApronThickness?.Copy(),
                ApronLength = this.ApronLength?.Copy(),
                NicheHeight = this.NicheHeight?.Copy(),
                NicheLength = this.NicheLength?.Copy(),
                WhideSurfaceIsolLeft = this.WhideSurfaceIsolLeft?.Copy(),
                WhideSurfaceIsolRight = this.WhideSurfaceIsolRight?.Copy(),
                InsertForFixingBlinds = this.InsertForFixingBlinds?.Copy(),
                HasMountingBracket = this.HasMountingBracket,
                MountingBracketEPS = this.MountingBracketEPS?.Copy(),
                Execution = this.Execution?.Copy(),
                HasSheetMetalBackWallForClinkerBrickFacade = this.HasSheetMetalBackWallForClinkerBrickFacade,
                SheetMetalBackWallForClinkerBrickFacadeBS = this.SheetMetalBackWallForClinkerBrickFacadeBS?.Copy(),
                SheetMetalBackWallForClinkerBrickFacadeVBLUR = this.SheetMetalBackWallForClinkerBrickFacadeVBLUR?.Copy(),
                SheetMetalBackWallForClinkerBrickFacadeVBLFS = this.SheetMetalBackWallForClinkerBrickFacadeVBLFS?.Copy(),
                Connector = this.Connector?.Copy(),
                Comment = this.Comment?.Copy()
            };

            copy.RegisterEvents(windowElement);

            return copy;
        }

        /// <summary>
        /// Updates the language of all Field properties.
        /// </summary>
        /// <param name="language">The language to update to.</param>
        public void UpdateFields(string language)
        {
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(Field))
                {
                    var field = property.GetValue(this) as Field;
                    field?.UpdateLanguage(language);
                }
            }
        }

        protected override string GenerateFilterKey()
        {
            return $"{Product.Value}{Pos.Value}{System.Value}{Typ.Value}{Thickness.Value}";
        }

        public override void SetGeneralMass(WindowElement windowElement)
        {
            windowElement.GeneralMass.ThickFacade.Selected = true;
            windowElement.GeneralMass.WindowLightWidth.Selected = true;
            windowElement.GeneralMass.FrameWidthTop.Selected = true;

            SetEnableEntries(false);
        }

        public override void SetEnableEntries(bool enable)
        {
            Pos.Enabled = enable;
            NicheLength.Enabled = enable;
            Thickness.Enabled = enable;
            ApronLength.Enabled = enable;
        }

        public void CalculateNicheLength(string windowLightWidth, string visibleFrameWidthLeft, string visibleFrameWidthRight, string thickPlasterLayerSoffit)
        {
            int windowWidth = ParseCount(windowLightWidth);
            int leftFrameWidth = ParseCount(visibleFrameWidthLeft);
            int rightFrameWidth = ParseCount(visibleFrameWidthRight);
            int plasterThickness = ParseCount(thickPlasterLayerSoffit);

            int totalLength = windowWidth + leftFrameWidth + rightFrameWidth + (2 * plasterThickness);

            if (!NicheLength.Enabled)
            {
                NicheLength.Value = totalLength.ToString();
            }
        }

        public void CalculateApronThickness(string thickFacade)
        {
            if (!Thickness.Enabled)
            {
                Thickness.Value = (ParseCount(thickFacade).ToString());
            }
        }

        public void CalculateApronLength()
        {
            if (!ApronLength.Enabled)
            {
                ApronLength.Value = (ParseCount(NicheLength.Value) + ParseCount(WhideSurfaceIsolLeft.Value) + ParseCount(WhideSurfaceIsolRight.Value)).ToString();
            }
        }
    }
}