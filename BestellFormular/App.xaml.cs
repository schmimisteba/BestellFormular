using BestellFormular.Models;
using BestellFormular.ViewModels;
using BestellFormular.Views;

namespace BestellFormular
{
    public partial class App : Application
    {
        private MainViewModel? viewModel;
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            try
            {
                var view = new MainView();

                var window = new Window(new MainView())
                {
                    X = 0,
                    Y = 0,
                    Width = 2736 / 2,
                    Height = 1824 / 2,
                    Title = "Bestellformular",
                    MinimumHeight = 500,
                    MinimumWidth = 900,
                };

                window.BindingContext = new MainViewModel();

                BestellFormularController.CreateInstance();

                window.SetBinding(Window.TitleProperty, new Binding("WindowTitle", source: viewModel));

                AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                {
                    System.Diagnostics.Debug.WriteLine($"Fehler: {e.ExceptionObject}");
                };

                return window;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Fenster-Fehler: {ex.Message}");
                return new Window(new ContentPage { Content = new Label { Text = "Fehler beim Start" } });
            }
        }

        protected override void OnStart()
        {
            //Load
        }

    }
}