using BestellFormular.Models.Helper;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Reflection;

namespace BestellFormular.Models.Window.Product
{
    public partial class SupportAngleWithSlope : ProductBase
    {
        // Observable properties for various support angle with slope characteristics
        [ObservableProperty] private Field system;
        [ObservableProperty] private Field thickness;
        [ObservableProperty] private Field lengthElement;
        [ObservableProperty] private Field length;
        [ObservableProperty] private Field deepParapet;
        [ObservableProperty] private Field heightParapet;
        [ObservableProperty] private Field wide;
        [ObservableProperty] private Field depth;
        [ObservableProperty] private Field insulationParapet;

        public SupportAngleWithSlope(WindowElement windowElement) : base(windowElement)
        {
            if (windowElement == null)
                throw new ArgumentNullException(nameof(windowElement), "WindowElement cannot be null");
        }

        public SupportAngleWithSlope(WindowElement windowElement, ExcelLoader excelLoader) : base(windowElement)
        {
            if (windowElement == null)
                throw new ArgumentNullException(nameof(windowElement), "WindowElement cannot be null");

            InitializeFields(excelLoader);
            RegisterEvents(windowElement);
        }

        private void InitializeFields(ExcelLoader excelLoader)
        {
            Product = excelLoader.GetFieldById("ID0004");
            Pos = excelLoader.GetFieldById("ID0220");
            System = excelLoader.GetFieldById("ID0221");
            Count = excelLoader.GetFieldById("ID0111");
            Thickness = excelLoader.GetFieldById("ID0222");
            Length = excelLoader.GetFieldById("ID0449");
            LengthElement = excelLoader.GetFieldById("ID0223");
            DeepParapet = excelLoader.GetFieldById("ID0224");
            HeightParapet = excelLoader.GetFieldById("ID0225");
            Wide = excelLoader.GetFieldById("ID0226");
            Depth = excelLoader.GetFieldById("ID0227");
            InsulationParapet = excelLoader.GetFieldById("ID0228");
            Comment = excelLoader.GetFieldById("ID0130");
        }

        private void RegisterEvents(WindowElement windowElement)
        {
            RegisterPosChangeHandler(Pos);

            //Disable automatisch berechnen
            Pos.PropertyChanged += (s, e) => EventCalculator.CalculateAll(e, windowElement);
            Length.PropertyChanged += (s, e) => EventCalculator.CalculateAll(e, windowElement);
            Thickness.PropertyChanged += (s, e) => EventCalculator.CalculateAll(e, windowElement);
            DeepParapet.PropertyChanged += (s, e) => EventCalculator.CalculateAll(e, windowElement);
            HeightParapet.PropertyChanged += (s, e) => EventCalculator.CalculateAll(e, windowElement);

            System.PropertyChanged += (s, e) => HandleSystemPropertyChanged(e, System.Value, LengthElement, System.FieldInputSelection.ToArray());

            Wide.PropertyChanged += (s, e) => ClearSurchargeChangeHandler(e, Wide, [Wide, Depth]);
            InsulationParapet.PropertyChanged += (s, e) => ClearSurchargeChangeHandler(e, InsulationParapet, [InsulationParapet]);
        }

        public override SupportAngleWithSlope Copy(WindowElement windowElement)
        {
            var copy = new SupportAngleWithSlope(windowElement)
            {
                Product = this.Product?.Copy(),
                Pos = this.Pos?.Copy(), // 🔹 Creates a new copy of Field
                Count = this.Count?.Copy(),
                Selected = this.Selected,
                Column = this.Column,
                System = this.System?.Copy(),
                Thickness = this.Thickness?.Copy(),
                Length = this.Length?.Copy(),
                LengthElement = this.LengthElement?.Copy(),
                DeepParapet = this.DeepParapet?.Copy(),
                HeightParapet = this.HeightParapet?.Copy(),
                Wide = this.Wide?.Copy(),
                Depth = this.Depth?.Copy(),
                InsulationParapet = this.InsulationParapet?.Copy(),
                Comment = this.Comment?.Copy(),
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

        public static void CountToElement(List<SupportAngleWithSlope> products)
        {
            foreach (SupportAngleWithSlope supportAngleWithSlope in products)
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
            return $"{Product.Value} {System.Value} {Thickness.Value} {Length.Value} {DeepParapet.Value} {HeightParapet.Value} {Wide.Value} {Depth.Value} {InsulationParapet.Value}";
        }

        public override void SetGeneralMass(WindowElement windowElement)
        {
            windowElement.GeneralMass.WindowLightWidth.Selected = true;
            windowElement.GeneralMass.ThickFacade.Selected = true;
            windowElement.GeneralMass.DeepWallRevealBottom.Selected = true;
            windowElement.GeneralMass.FrameWidthBottom.Selected = true;

            SetEnableEntries(false);
        }

        public override void SetEnableEntries(bool enable)
        {
            Pos.Enabled = enable;
            Length.Enabled = enable;
            Thickness.Enabled = enable;
            DeepParapet.Enabled = enable;
            HeightParapet.Enabled = enable;
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

        public void CalculateThickness(string thickFacade)
        {
            Thickness.Value = ParseCount(thickFacade).ToString();
        }

        public void CalculateDeepParapet(string deepWallRevealBottom, string thickAdhesiveLayer)
        {
            if (!DeepParapet.Enabled)
            {
                DeepParapet.Value = (ParseCount(deepWallRevealBottom) + ParseCount(thickAdhesiveLayer)).ToString();
            }
        }

        // C is missing
        public void CalculateHeightParapet(string frameWidthBottom, string thickAdhesiveLayer, string visibleFrameWidthBottom)
        {
            if (!HeightParapet.Enabled)
            {
                HeightParapet.Value = (ParseCount(frameWidthBottom) - ParseCount(thickAdhesiveLayer) - ParseCount(visibleFrameWidthBottom)).ToString();
            }
        }
    }
}