using BestellFormular.Models;
using BestellFormular.ViewModels;
using BestellFormular.Views;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace BestellFormular
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.UseMauiApp<App>().UseMauiCommunityToolkit().ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            }).UseMauiCommunityToolkit();
#if DEBUG
            builder.Logging.AddDebug();
#endif
            builder.ConfigureMauiHandlers(handlers =>
            {
                handlers.AddHandler(typeof(Microsoft.Maui.Controls.MenuBarItem), typeof(Microsoft.Maui.Handlers.MenuBarItemHandler));
            });

//            EntryHandler.Mapper.AppendToMapping(nameof(Entry), (handler, view) =>
//            {
//#if WINDOWS
//                var platformView = handler.PlatformView;
//                platformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(1);

//                // Hole den Wert des StaticResource (in diesem Fall die Farbe)
//                var defaultBorderColor = (Microsoft.Maui.Graphics.Color)Application.Current.Resources["PrimaryBackground"];

//                // Setze den BorderBrush mit der abgerufenen Farbe
//                var defaultBorderBrush = new SolidColorBrush(defaultBorderColor);


//                // Set consistent border brush for all states
//                platformView.BorderBrush = (Microsoft.UI.Xaml.Media.SolidColorBrush)defaultBorderBrush;
//                platformView.Resources["TextControlBorderBrush"] = defaultBorderBrush;
//                platformView.Resources["TextControlBorderBrushPointerOver"] = defaultBorderBrush;
//                platformView.Resources["TextControlBorderBrushFocused"] = defaultBorderBrush;
//                platformView.Resources["TextControlBorderBrushDisabled"] = defaultBorderBrush;
//#endif
//            });

            builder.Services.AddSingleton<MainView>();
            builder.Services.AddSingleton<MainViewModel>();

            return builder.Build();
        }
    }
}