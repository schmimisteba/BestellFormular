using BestellFormular.Models.Helper;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Reflection;

namespace BestellFormular.Models.Window.Product
{
    public partial class SupportWedgesWithSlope : ProductBase
    {
        // Observable properties for support wedges with slope characteristics
        [ObservableProperty] private Field system;
        [ObservableProperty] private Field depth;
        [ObservableProperty] private Field height;
        [ObservableProperty] private Field lengthElement;
        [ObservableProperty] private Field length;
        [ObservableProperty] private Field typ;
        [ObservableProperty] private Field recessWidth;
        [ObservableProperty] private Field recessDepth;
        [ObservableProperty] private Field aerogel;
        public SupportWedgesWithSlope(WindowElement windowElement) : base(windowElement)
        {
            if (windowElement == null)
                throw new ArgumentNullException(nameof(windowElement), "WindowElement cannot be null");
        }

        public SupportWedgesWithSlope(WindowElement windowElement, ExcelLoader excelLoader) : base(windowElement)
        {
            if (windowElement == null)
                throw new ArgumentNullException(nameof(windowElement), "WindowElement cannot be null");

            InitializeFields(excelLoader);
            RegisterEvents(windowElement);
        }

        private void InitializeFields(ExcelLoader excelLoader)
        {
            Product = excelLoader.GetFieldById("ID0005"); // Adjust IDs as needed
            Pos = excelLoader.GetFieldById("ID0230");
            System = excelLoader.GetFieldById("ID0231");
            Count = excelLoader.GetFieldById("ID0111");
            Depth = excelLoader.GetFieldById("ID0232");
            Height = excelLoader.GetFieldById("ID0233");
            Length = excelLoader.GetFieldById("ID0449");
            LengthElement = excelLoader.GetFieldById("ID0234");
            Typ = excelLoader.GetFieldById("ID0229");
            RecessWidth = excelLoader.GetFieldById("ID0235");
            RecessDepth = excelLoader.GetFieldById("ID0236");
            Aerogel = excelLoader.GetFieldById("ID0237");
            Comment = excelLoader.GetFieldById("ID0130");
        }

        private void RegisterEvents(WindowElement windowElement)
        {
            RegisterPosChangeHandler(Pos);

            //Disable automatisch berechnen
            Pos.PropertyChanged += (s, e) => EventCalculator.CalculateAll(e, windowElement);
            Length.PropertyChanged += (s, e) => EventCalculator.CalculateAll(e, windowElement);
            Depth.PropertyChanged += (s, e) => EventCalculator.CalculateAll(e, windowElement);
            Height.PropertyChanged += (s, e) => EventCalculator.CalculateAll(e, windowElement);

            System.PropertyChanged += (s, e) => HandlePropertyChanged(e, System.Value, Typ, new[] { "XPS" }); 
            System.PropertyChanged += (s, e) => HandleSystemPropertyChanged(e, System.Value, LengthElement, System.FieldInputSelection.ToArray());

            Depth.PropertyChanged += (s, e) => EventCalculator.CalculateAll(e, windowElement);

            RecessWidth.PropertyChanged += (s, e) => ClearSurchargeChangeHandler(e, RecessWidth, [RecessWidth, RecessDepth]);
            Aerogel.PropertyChanged += (s, e) => ClearSurchargeChangeHandler(e, Aerogel, [Aerogel, Aerogel]);
        }

        public override SupportWedgesWithSlope Copy(WindowElement windowElement)
        {
            var copy = new SupportWedgesWithSlope(windowElement)
            {
                Product = this.Product?.Copy(),
                Pos = this.Pos?.Copy(),
                System = this.System?.Copy(),
                Count = this.Count?.Copy(),
                Column = this.Column,
                Selected = this.Selected,
                Depth = this.Depth?.Copy(),
                Height = this.Height?.Copy(),
                Length = this.Length?.Copy(),
                LengthElement = this.LengthElement?.Copy(),
                Typ = this.Typ?.Copy(),
                RecessWidth = this.RecessWidth?.Copy(),
                RecessDepth = this.RecessDepth?.Copy(),
                Aerogel = this.Aerogel?.Copy(),
                Comment = this.Comment?.Copy()
            };

            copy.RegisterEvents(windowElement);
            return copy;
        }

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

        public static void CountToElement(List<SupportWedgesWithSlope> products)
        {
            foreach (SupportWedgesWithSlope supportAngleWithSlope in products)
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
            return $"{Product?.Value}{System?.Value}{Depth?.Value}{Height?.Value}{Length?.Value}{Pos?.Value}";
        }

        public override void SetGeneralMass(WindowElement windowElement)
        {
            windowElement.GeneralMass.ThickFacade.Selected = true;
            windowElement.GeneralMass.WindowLightWidth.Selected = true;
            windowElement.GeneralMass.DeepWallRevealBottom.Selected = true;

            SetEnableEntries(false);
        }

        public override void SetEnableEntries(bool enable)
        {
            Pos.Enabled = enable;
            Length.Enabled = enable;
            Depth.Enabled = enable;
            Height.Enabled = enable;
        }
        public void CalculateLenght(string windowLightWidth, string visibleFrameWidthLeft, string visibleFrameWidthRight, string connectionRight, string connectionLeft, string widePlasterboard)
        {
            int lightWidth = ParseCount(windowLightWidth);
            int leftFrame = ParseCount(visibleFrameWidthLeft);
            int rightFrame = ParseCount(visibleFrameWidthRight);

            connectionRight = StringExtensions.GetBetween(connectionRight, "[", "]") ?? connectionRight;
            connectionLeft = StringExtensions.GetBetween(connectionLeft, "[", "]") ?? connectionLeft;

            int rightConnection = 0;
            int leftConnection = 0;
            switch (connectionRight)
            {
                case "S":
                    rightConnection = 2;
                    break;
                case "P":
                    rightConnection = ParseCount(widePlasterboard);
                    break;
                default:
                    break;
            }
            switch (connectionLeft)
            {
                case "S":
                    leftConnection = 2;
                    break;
                case "P":
                    leftConnection = ParseCount(widePlasterboard);
                    break;
                default:
                    break;
            }

            if (!Length.Enabled)
            {
                Length.Value = (lightWidth + leftFrame + rightFrame + rightConnection + leftConnection).ToString();
            }
        }

        public void CalculateDepth(string thickFacade, string deepWallRevealBottom, string thickAdhesiveLayer)
        {
            if (!Depth.Enabled)
            {
                Depth.Value = (ParseCount(thickFacade) + ParseCount(deepWallRevealBottom) + ParseCount(thickAdhesiveLayer)).ToString();
            }
        }

        public void CalculateHeight(string angle)
        {
            if (!Height.Enabled)
            {
                Height.Value = Math.Round(20 + ParseCount(Depth.Value) * Math.Tan(EventCalculator.ConvertToRadians(ParseCount(angle))), 0).ToString();
            }
        }
    }
}