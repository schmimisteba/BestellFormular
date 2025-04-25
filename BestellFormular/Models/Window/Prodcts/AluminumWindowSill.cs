using BestellFormular.Models.Helper;
using BestellFormular.Resources.Language;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BestellFormular.Models.Window.Product
{
    /// <summary>
    /// Represents an aluminum window sill product with various configurable properties.
    /// </summary>
    public partial class AluminumWindowSill : ProductBase
    {
        // Observable properties for various window sill characteristics
        [ObservableProperty] private Field material;
        [ObservableProperty] private Field color;
        [ObservableProperty] private Field lengthCleaningLight;
        [ObservableProperty] private Field length;
        [ObservableProperty] private Field depth;
        [ObservableProperty] private Field connectionLeft;
        [ObservableProperty] private Field connectionRight;
        [ObservableProperty] private Field miterOutsideLeft;
        [ObservableProperty] private Field miterOutsideRight;
        [ObservableProperty] private Field miterInsideLeft;
        [ObservableProperty] private Field miterInsideRight;

        [ObservableProperty] private Field deepBackBow;
        [ObservableProperty] private Field height;
        [ObservableProperty] private Field highBoarding;
        [ObservableProperty] private Field highBord;
        [ObservableProperty] private Field widePlasterboard;
        [ObservableProperty] private Field headStart;
        [ObservableProperty] private Field angle;

        [ObservableProperty] private Field lengthTwoPieceLeft;
        [ObservableProperty] private Field lengthTwoPieceRight;
        [ObservableProperty] private Field fuge;

        [ObservableProperty] private Field cleaningBoardBack;

        [ObservableProperty] private Field loadingNotch;
        
        [ObservableProperty] private Field notchingForWeatherboards;

        [ObservableProperty] private Field waterDeflectorLeft;
        [ObservableProperty] private Field waterDeflectorRight;

        [ObservableProperty] private Field glueOnSupportWedge;

        /// <summary>
        /// Initializes a new instance of the AluminumWindowSill class.
        /// </summary>
        /// <param name="windowElement">The window element associated with this window sill.</param>
        /// <exception cref="ArgumentNullException">Thrown when windowElement is null.</exception>
        public AluminumWindowSill(WindowElement windowElement) : base(windowElement)
        {
            ValidateWindowElement(windowElement);
        }

        /// <summary>
        /// Initializes a new instance of the AluminumWindowSill class with Excel helper.
        /// </summary>
        /// <param name="windowElement">The window element associated with this window sill.</param>
        /// <param name="excelLoader">Helper for retrieving field data from Excel.</param>
        /// <exception cref="ArgumentNullException">Thrown when windowElement is null.</exception>
        public AluminumWindowSill(WindowElement windowElement, ExcelLoader excelLoader) : base(windowElement)
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
            Product = excelLoader.GetFieldById("ID0003");
            Pos = excelLoader.GetFieldById("ID0176");
            Count = excelLoader.GetFieldById("ID0111");
            Comment = excelLoader.GetFieldById("ID0130");
            Color = excelLoader.GetFieldById("ID0178");
            Material = excelLoader.GetFieldById("ID0177");
            LengthCleaningLight = excelLoader.GetFieldById("ID0180");
            Length = excelLoader.GetFieldById("ID0217");
            Depth = excelLoader.GetFieldById("ID0179");
            ConnectionRight = excelLoader.GetFieldById("ID0184");
            ConnectionLeft = excelLoader.GetFieldById("ID0183");
            MiterOutsideLeft = excelLoader.GetFieldById("ID0212");
            MiterOutsideRight = excelLoader.GetFieldById("ID0213");
            MiterInsideLeft = excelLoader.GetFieldById("ID0214");
            MiterInsideRight = excelLoader.GetFieldById("ID0215");

            DeepBackBow = excelLoader.GetFieldById("ID0185");
            Height = excelLoader.GetFieldById("ID0186");
            HighBoarding = excelLoader.GetFieldById("ID0187");
            HighBord = excelLoader.GetFieldById("ID0188");
            WidePlasterboard = excelLoader.GetFieldById("ID0189");
            HeadStart = excelLoader.GetFieldById("ID0190");
            Angle = excelLoader.GetFieldById("ID0191");

            LengthTwoPieceLeft = excelLoader.GetFieldById("ID0181");
            Fuge = excelLoader.GetFieldById("ID0203");
            LengthTwoPieceRight = excelLoader.GetFieldById("ID0182");

            CleaningBoardBack = excelLoader.GetFieldById("ID0207");

            LoadingNotch = excelLoader.GetFieldById("ID0208");

            NotchingForWeatherboards = excelLoader.GetFieldById("ID0209");

            WaterDeflectorLeft = excelLoader.GetFieldById("ID0210");
            WaterDeflectorRight = excelLoader.GetFieldById("ID0211");

            GlueOnSupportWedge = excelLoader.GetFieldById("ID0219");
        }

        /// <summary>
        /// Registers event handlers for various field changes.
        /// </summary>
        private void RegisterEvents(WindowElement windowElement)
        {
            RegisterPosChangeHandler(Pos);
            RegisterMaterialChangeHandler(Material, Color, new[] { "PB", "RP", "GP" });
            RegisterMiterChangeHandler(ConnectionRight, MiterOutsideRight, MiterInsideRight);
            RegisterMiterChangeHandler(ConnectionLeft, MiterOutsideLeft, MiterInsideLeft);

            //Disable automatisch berechnen
            Pos.PropertyChanged += (s, e) => EventCalculator.CalculateAll(e, windowElement);
            Depth.PropertyChanged += (s, e) => EventCalculator.CalculateAll(e, windowElement);
            LengthCleaningLight.PropertyChanged += (s, e) => EventCalculator.CalculateAll(e, windowElement);
            Length.PropertyChanged += (s, e) => EventCalculator.CalculateAll(e, windowElement);

            LengthTwoPieceLeft.PropertyChanged += (s, e) => EventCalculator.CalculateAll(e, windowElement);
            LengthTwoPieceLeft.PropertyChanged += (s, e) => ClearSurchargeChangeHandler(e, LengthTwoPieceLeft, new List<Field> { LengthTwoPieceLeft, LengthTwoPieceRight, Fuge });
            LengthTwoPieceRight.PropertyChanged += (s, e) => EventCalculator.CalculateAll(e, windowElement);

            CleaningBoardBack.PropertyChanged += (s, e) => ClearSurchargeChangeHandler(e, CleaningBoardBack, new List<Field> { CleaningBoardBack}, Resource.No);
            LoadingNotch.PropertyChanged += (s, e) => ClearSurchargeChangeHandler(e, LoadingNotch, new List<Field> { LoadingNotch }, Resource.No);
            NotchingForWeatherboards.PropertyChanged += (s, e) => ClearSurchargeChangeHandler(e, NotchingForWeatherboards, new List<Field> { NotchingForWeatherboards }, Resource.No);
            WaterDeflectorLeft.PropertyChanged += (s, e) => ClearSurchargeChangeHandler(e, WaterDeflectorLeft, new List<Field> { WaterDeflectorLeft, WaterDeflectorRight });
            GlueOnSupportWedge.PropertyChanged += (s, e) => ClearSurchargeChangeHandler(e, GlueOnSupportWedge, new List<Field> { GlueOnSupportWedge }, Resource.No);

            ConnectionRight.PropertyChanged += (s, e) => EventCalculator.CalculateAll(e, windowElement);
            ConnectionLeft.PropertyChanged += (s, e) => EventCalculator.CalculateAll(e, windowElement);
            WidePlasterboard.PropertyChanged += (s, e) => EventCalculator.CalculateAll(e, windowElement);
            HeadStart.PropertyChanged += (s, e) => EventCalculator.CalculateAll(e, windowElement);

            Angle.PropertyChanged += (s, e) => EventCalculator.CalculateAll(e, windowElement);

            MiterOutsideLeft.PropertyChanged += (s, e) => EventCalculator.CalculateAll(e, windowElement);
            MiterOutsideRight.PropertyChanged += (s, e) => EventCalculator.CalculateAll(e, windowElement);
        }

        /// <summary>
        /// Creates a deep copy of the AluminumWindowSill with a new window element.
        /// </summary>
        /// <param name="windowElement">The new window element for the copied instance.</param>
        /// <returns>A new AluminumWindowSill instance with copied properties.</returns>
        public override AluminumWindowSill Copy(WindowElement windowElement)
        {
            var copy = new AluminumWindowSill(windowElement)
            {
                Product = this.Product?.Copy(),
                Pos = this.Pos?.Copy(),
                Count = this.Count?.Copy(),
                Selected = this.Selected,
                Column = this.Column,
                Comment = this.Comment?.Copy(),
                Material = this.Material?.Copy(),
                Color = this.Color?.Copy(),
                LengthCleaningLight = this.LengthCleaningLight?.Copy(),
                Length = this.Length?.Copy(),
                Depth = this.Depth?.Copy(),
                ConnectionRight = this.ConnectionRight?.Copy(),
                ConnectionLeft = this.ConnectionLeft?.Copy(),
                MiterOutsideLeft = this.MiterOutsideLeft?.Copy(),
                MiterOutsideRight = this.MiterOutsideRight?.Copy(),
                MiterInsideLeft = this.MiterInsideLeft?.Copy(),
                MiterInsideRight = this.MiterInsideRight?.Copy(),

                DeepBackBow = this.DeepBackBow?.Copy(),
                Height = this.Height?.Copy(),
                HighBoarding = this.HighBoarding?.Copy(),
                HighBord = this.HighBord?.Copy(),
                WidePlasterboard = this.WidePlasterboard?.Copy(),
                HeadStart = this.HeadStart?.Copy(),
                Angle = this.Angle?.Copy(),

                LengthTwoPieceLeft = this.LengthTwoPieceLeft?.Copy(),
                Fuge = this.Fuge?.Copy(),
                LengthTwoPieceRight = this.LengthTwoPieceRight?.Copy(),

                CleaningBoardBack = this.CleaningBoardBack?.Copy(),

                LoadingNotch = this.LoadingNotch?.Copy(),

                NotchingForWeatherboards = this.NotchingForWeatherboards?.Copy(),

                WaterDeflectorLeft = this.WaterDeflectorLeft?.Copy(),
                WaterDeflectorRight = this.WaterDeflectorRight?.Copy(),

                GlueOnSupportWedge = this.GlueOnSupportWedge?.Copy(),

            };

            copy.RegisterEvents(windowElement);
            return copy;
        }

        /// <summary>
        /// Handles changes to the material field and updates the color field accordingly.
        /// </summary>
        /// <returns>The Unique Key.</returns>
        protected override string GenerateFilterKey()
        {
            return $"{Pos.Value}{Material.Value}{Color.Value}{LengthCleaningLight.Value}{Depth.Value}{ConnectionRight.Value}{ConnectionLeft.Value}{MiterOutsideLeft.Value}{MiterOutsideRight.Value}{MiterInsideLeft.Value}{MiterInsideRight.Value}";
        }

        public override void SetGeneralMass(WindowElement windowElement)
        {
            var generalMass = windowElement.GeneralMass;

            // Set all required properties to selected
            generalMass.ThickFacade.Selected = true;
            generalMass.DeepWallRevealBottom.Selected = true;
            generalMass.WindowLightWidth.Selected = true;

            SetEnableEntries(false);
        }

        public override void SetEnableEntries(bool enable)
        {
            Pos.Enabled = enable;
            Depth.Enabled = enable;
            LengthCleaningLight.Enabled = enable;
            Length.Enabled = enable;
            Fuge.Enabled = enable;
        }

        public void CalculateDepth(string deepWallRevealBottom, string thickFacade, string headStart,
                                  string thickAdhesiveLayer, string thickLayerOfPlasterFacade, string wideJointWindowSill)
        {
            // Parse values with default of 0 for invalid inputs
            int deepWall = ParseCount(deepWallRevealBottom);
            int facade = ParseCount(thickFacade);
            int head = ParseCount(headStart);
            int adhesiveLayer = ParseCount(thickAdhesiveLayer);
            int plaster = ParseCount(thickLayerOfPlasterFacade);
            int joint = ParseCount(wideJointWindowSill);

            // Calculate and store result

            if (!Depth.Enabled)
            {
                Depth.Value = (deepWall + facade + head + adhesiveLayer + plaster - joint).ToString();
            }
        }

        public void CalculateLengthCleaningLight(string windowLightWidth, string visibleFrameWidthLeft, string visibleFrameWidthRight)
        {
            // Parse values with default of 0 for invalid inputs
            int lightWidth = ParseCount(windowLightWidth);
            int leftFrame = ParseCount(visibleFrameWidthLeft);
            int rightFrame = ParseCount(visibleFrameWidthRight);

            // Calculate and store result
            if (!LengthCleaningLight.Enabled)
            {
                LengthCleaningLight.Value = (lightWidth + leftFrame + rightFrame).ToString();
            }
        }

        public void CalculateFuge()
        {
            int left = ParseCount(LengthTwoPieceLeft.Value);
            int right = ParseCount(LengthTwoPieceRight.Value);
            int length = ParseCount(Length.Value);

            Fuge.Value = "";

            if (left > 0 && right > 0)
            {
                if (!Fuge.Enabled)
                {
                    Fuge.Value = (length - left - right).ToString();
                }
            }
        }

        public void CalculateLenght(string windowLightWidth, string visibleFrameWidthLeft, string visibleFrameWidthRight, string connectionRight, string connectionLeft, string widePlasterboard)
        {
            // Parse input values
            int lightWidth = ParseCount(windowLightWidth);
            int leftFrame = ParseCount(visibleFrameWidthLeft);
            int rightFrame = ParseCount(visibleFrameWidthRight);

            // Clean connection values
            connectionRight = ExtractConnectionValue(connectionRight);
            connectionLeft = ExtractConnectionValue(connectionLeft);

            // Calculate connection widths
            int rightConnection = CalculateConnectionWidth(connectionRight, widePlasterboard);
            int leftConnection = CalculateConnectionWidth(connectionLeft, widePlasterboard);

            // Calculate miter outside lengths
            double lengthMiterOutsideLeft = CalculateMiterLength(MiterOutsideLeft.Value);
            double lengthMiterOutsideRight = CalculateMiterLength(MiterOutsideRight.Value);

            // Calculate total length
            if (!Length.Enabled)
            {
                Length.Value = Math.Round(lightWidth + leftFrame + rightFrame +
                                       rightConnection + leftConnection +
                                       lengthMiterOutsideLeft + lengthMiterOutsideRight).ToString();
            }
        }

        private string ExtractConnectionValue(string connection)
        {
            return StringExtensions.GetBetween(connection, "[", "]") ?? connection;
        }

        private int CalculateConnectionWidth(string connectionType, string widePlasterboard)
        {
            return connectionType switch
            {
                "S" => 2,
                "P" => ParseCount(widePlasterboard),
                _ => 0
            };
        }

        private double CalculateMiterLength(string miterValue)
        {
            if (string.IsNullOrEmpty(miterValue))
                return 0;

            return ParseCount(Depth.Value) / Math.Tan(EventCalculator.ConvertToRadians(ParseCount(miterValue)));
        }
    }
}