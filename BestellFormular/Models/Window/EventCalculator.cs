using System.ComponentModel;

namespace BestellFormular.Models.Window
{
    internal static class EventCalculator
    {
        public static double ConvertToRadians(int degrees)
        {
            return degrees * (Math.PI / 180);
        }

        public static void CalculateAll(PropertyChangedEventArgs e, WindowElement windowElement)
        {
            if (windowElement.GeneralMass.Selected)
            {
                if (e.PropertyName == nameof(Field.Value) || e.PropertyName == nameof(Field.Enabled))
                {
                    windowElement.SetPos(windowElement.Name);

                    windowElement.AluminumWindowSill.CalculateDepth(
                        windowElement.GeneralMass.DeepWallRevealBottom.Value,
                        windowElement.GeneralMass.ThickFacade.Value,
                        windowElement.AluminumWindowSill.HeadStart.Value,
                        windowElement.GeneralMass.ThickAdhesiveLayer.Value,
                        windowElement.GeneralMass.ThickLayerOfPlasterFacade.Value,
                        windowElement.GeneralMass.WideJointWindowSill.Value);

                    windowElement.AluminumWindowSill.CalculateLengthCleaningLight(
                        windowElement.GeneralMass.WindowLightWidth.Value,
                        windowElement.GeneralMass.VisibleFrameWidthLeft.Value,
                        windowElement.GeneralMass.VisibleFrameWidthRight.Value);

                    windowElement.AluminumWindowSill.CalculateLenght(
                        windowElement.GeneralMass.WindowLightWidth.Value,
                        windowElement.GeneralMass.VisibleFrameWidthLeft.Value,
                        windowElement.GeneralMass.VisibleFrameWidthRight.Value,
                        windowElement.AluminumWindowSill.ConnectionLeft.Value,
                        windowElement.AluminumWindowSill.ConnectionRight.Value,
                        windowElement.AluminumWindowSill.WidePlasterboard.Value);

                    windowElement.AluminumWindowSill.CalculateFuge();

                    windowElement.SupportAngleWithSlope.CalculateLenght(
                        windowElement.GeneralMass.WindowLightWidth.Value,
                        windowElement.GeneralMass.VisibleFrameWidthLeft.Value,
                        windowElement.GeneralMass.VisibleFrameWidthRight.Value,
                        windowElement.AluminumWindowSill.ConnectionLeft.Value,
                        windowElement.AluminumWindowSill.ConnectionRight.Value,
                        windowElement.AluminumWindowSill.WidePlasterboard.Value);

                    windowElement.SupportAngleWithSlope.CalculateThickness(
                        windowElement.GeneralMass.ThickFacade.Value);

                    windowElement.SupportAngleWithSlope.CalculateDeepParapet(
                        windowElement.GeneralMass.DeepWallRevealBottom.Value,
                        windowElement.GeneralMass.ThickAdhesiveLayer.Value);

                    windowElement.SupportAngleWithSlope.CalculateHeightParapet(
                        windowElement.GeneralMass.FrameWidthBottom.Value,
                        windowElement.GeneralMass.ThickAdhesiveLayer.Value,
                        windowElement.GeneralMass.VisibleFrameWidthBottom.Value);

                    windowElement.SupportWedgesWithSlope.CalculateLenght(
                        windowElement.GeneralMass.WindowLightWidth.Value,
                        windowElement.GeneralMass.VisibleFrameWidthLeft.Value,
                        windowElement.GeneralMass.VisibleFrameWidthRight.Value,
                        windowElement.AluminumWindowSill.ConnectionLeft.Value,
                        windowElement.AluminumWindowSill.ConnectionRight.Value,
                        windowElement.AluminumWindowSill.WidePlasterboard.Value);

                    windowElement.SupportWedgesWithSlope.CalculateDepth(
                        windowElement.GeneralMass.ThickFacade.Value,
                        windowElement.GeneralMass.DeepWallRevealBottom.Value,
                        windowElement.GeneralMass.ThickAdhesiveLayer.Value);


                    windowElement.SupportWedgesWithSlope.CalculateHeight(
                        windowElement.AluminumWindowSill.Angle.Value);

                    windowElement.ApronElement.CalculateNicheLength(
                        windowElement.GeneralMass.WindowLightWidth.Value,
                        windowElement.GeneralMass.VisibleFrameWidthLeft.Value,
                        windowElement.GeneralMass.VisibleFrameWidthRight.Value,
                        windowElement.GeneralMass.ThickPlasterLayerSoffit.Value);

                    windowElement.ApronElement.CalculateApronThickness(
                        windowElement.GeneralMass.ThickFacade.Value);

                    windowElement.ApronElement.CalculateApronLength();

                    windowElement.Ravisol.CalculateLenght(
                        windowElement.GeneralMass.WindowLightWidth.Value,
                        windowElement.GeneralMass.VisibleFrameWidthLeft.Value,
                        windowElement.GeneralMass.VisibleFrameWidthRight.Value);

                    windowElement.Ravisol.CalculateHeight(
                        windowElement.GeneralMass.FrameWidthTop.Value,
                        windowElement.GeneralMass.VisibleFrameWidthTop.Value);

                    windowElement.Stuisol.CalculateLenght(
                        windowElement.GeneralMass.WindowLightWidth.Value,
                        windowElement.GeneralMass.VisibleFrameWidthLeft.Value,
                        windowElement.GeneralMass.VisibleFrameWidthRight.Value);

                    windowElement.Stuisol.CalculateWide(
                        windowElement.GeneralMass.ThickFacade.Value,
                        windowElement.ApronElement.ApronThickness.Value);
                }
            }
        }
    }
}
