using BestellFormular.Models.Helper;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace BestellFormular.Models.Window
{
    public partial class GeneralMass : ProductBase
    {
        [ObservableProperty] private Field thickAdhesiveLayer;
        [ObservableProperty] private Field thickLayerOfPlasterFacade;
        [ObservableProperty] private Field thickPlasterLayerSoffit;
        [ObservableProperty] private Field wideJointWindowSill;
        [ObservableProperty] private Field thickFacade;
        [ObservableProperty] private Field deepWallRevealBottom;
        [ObservableProperty] private Field frameWidthTop;
        [ObservableProperty] private Field frameWidthBottom;
        [ObservableProperty] private Field visibleFrameWidthTop;
        [ObservableProperty] private Field visibleFrameWidthBottom;
        [ObservableProperty] private Field windowLightWidth;
        [ObservableProperty] private Field visibleFrameWidthLeft;
        [ObservableProperty] private Field visibleFrameWidthRight;

        private WindowElement _windowElement;

        public GeneralMass(WindowElement windowElement) : base(windowElement)
        {
            if (windowElement == null)
                throw new ArgumentNullException(nameof(windowElement), "WindowElement cannot be null");
            _windowElement = windowElement;
        }

        public GeneralMass(WindowElement windowElement, ExcelLoader excelLoader) : base(windowElement)
        {
            InitializeFields(excelLoader);
            _windowElement = windowElement;
            RegisterEvents(windowElement);
        }

        private void InitializeFields(ExcelLoader excelLoader)
        {
            ThickAdhesiveLayer = excelLoader.GetFieldById("ID8100");
            ThickLayerOfPlasterFacade = excelLoader.GetFieldById("ID8101");
            ThickPlasterLayerSoffit = excelLoader.GetFieldById("ID8102");
            WideJointWindowSill = excelLoader.GetFieldById("ID8103"); 
            ThickFacade = excelLoader.GetFieldById("ID0114");
            DeepWallRevealBottom = excelLoader.GetFieldById("ID0116");
            FrameWidthTop = excelLoader.GetFieldById("ID0121");
            FrameWidthBottom = excelLoader.GetFieldById("ID0122");
            VisibleFrameWidthTop = excelLoader.GetFieldById("ID0123");
            VisibleFrameWidthBottom = excelLoader.GetFieldById("ID0124");
            WindowLightWidth = excelLoader.GetFieldById("ID0125");
            VisibleFrameWidthLeft = excelLoader.GetFieldById("ID0128");
            VisibleFrameWidthRight = excelLoader.GetFieldById("ID0129");
        }

        private void RegisterEvents(WindowElement windowElement)
        {
            ThickAdhesiveLayer.PropertyChanged += (s, e) => EventCalculator.CalculateAll(e, windowElement);
            ThickLayerOfPlasterFacade.PropertyChanged += (s, e) => EventCalculator.CalculateAll(e, windowElement);
            ThickPlasterLayerSoffit.PropertyChanged += (s, e) => EventCalculator.CalculateAll(e, windowElement);
            WideJointWindowSill.PropertyChanged += (s, e) => EventCalculator.CalculateAll(e, windowElement);
            ThickFacade.PropertyChanged += (s, e) => EventCalculator.CalculateAll(e, windowElement);
            DeepWallRevealBottom.PropertyChanged += (s, e) => EventCalculator.CalculateAll(e, windowElement);
            FrameWidthTop.PropertyChanged += (s, e) => EventCalculator.CalculateAll(e, windowElement);
            FrameWidthBottom.PropertyChanged += (s, e) => EventCalculator.CalculateAll(e, windowElement);
            VisibleFrameWidthTop.PropertyChanged += (s, e) => EventCalculator.CalculateAll(e, windowElement);
            VisibleFrameWidthBottom.PropertyChanged += (s, e) => EventCalculator.CalculateAll(e, windowElement);
            WindowLightWidth.PropertyChanged += (s, e) => EventCalculator.CalculateAll(e, windowElement);
            VisibleFrameWidthLeft.PropertyChanged += (s, e) => EventCalculator.CalculateAll(e, windowElement);
            VisibleFrameWidthRight.PropertyChanged += (s, e) => EventCalculator.CalculateAll(e, windowElement);
        }

        public override GeneralMass Copy(WindowElement windowElement)
        {
            var copy = new GeneralMass(windowElement)
            {
                Column = this.Column,
                Selected = this.Selected,
                ThickAdhesiveLayer = this.ThickAdhesiveLayer?.Copy(),
                ThickLayerOfPlasterFacade = this.ThickLayerOfPlasterFacade?.Copy(),
                ThickPlasterLayerSoffit = this.ThickPlasterLayerSoffit?.Copy(),
                WideJointWindowSill = this.WideJointWindowSill?.Copy(),
                ThickFacade = this.ThickFacade?.Copy(),
                DeepWallRevealBottom = this.DeepWallRevealBottom?.Copy(),
                FrameWidthTop = this.FrameWidthTop?.Copy(),
                FrameWidthBottom = this.FrameWidthBottom?.Copy(),
                VisibleFrameWidthTop = this.VisibleFrameWidthTop?.Copy(),
                VisibleFrameWidthBottom = this.VisibleFrameWidthBottom?.Copy(),
                WindowLightWidth = this.WindowLightWidth?.Copy(),
                VisibleFrameWidthLeft = this.VisibleFrameWidthLeft?.Copy(),
                VisibleFrameWidthRight = this.VisibleFrameWidthRight?.Copy()
            };

            copy.RegisterEvents(windowElement);

            return copy;
        }

        public void ResetVisability()
        {
            ThickAdhesiveLayer.Selected = false;
            ThickLayerOfPlasterFacade.Selected = false;
            ThickPlasterLayerSoffit.Selected = false;
            WideJointWindowSill.Selected = false;
            ThickFacade.Selected = false;
            DeepWallRevealBottom.Selected = false;
            FrameWidthTop.Selected = false;
            FrameWidthBottom.Selected = false;
            VisibleFrameWidthTop.Selected = false;
            VisibleFrameWidthBottom.Selected = false;
            WindowLightWidth.Selected = false;
            VisibleFrameWidthLeft.Selected = false;
            VisibleFrameWidthRight.Selected = false;
        }

        protected override string GenerateFilterKey()
        {
            // Do nothing
            return string.Empty;
        }

        public override void SetGeneralMass(WindowElement windowElement)
        {
            return;
        }

        public override void SetEnableEntries(bool enable)
        {
            return;
        }
    }
}
