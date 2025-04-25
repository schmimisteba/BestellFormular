using BestellFormular.Models;
using BestellFormular.ViewModels;

namespace BestellFormular.Views
{
    public partial class MainView : ContentPage
    {
        private static readonly string scrollViewName = "ProductScrollView";
        public MainView()
        {
            InitializeComponent();;
        }

        protected override void OnDisappearing()
        {
            MainViewModel mainViewModel = this.BindingContext as MainViewModel;
            if (mainViewModel != null)
            {
                BestellFormularController.Instance.ExportToExcel(mainViewModel.Elements, mainViewModel.AdressHead, BestellFormularController.Instance.GetCurrentFilePath());
            }
            base.OnDisappearing();
        }

        private void OnEntryUnfocused(object sender, FocusEventArgs e)
        {
            if (sender is Entry entry)
            {
                MainViewModel mainViewModel = this.BindingContext as MainViewModel;
                mainViewModel.AdressHead.SetSelection(false);
            }
        }

        private void GeneralMass_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (sender is Button button &&
                    button.Parent is Grid grid &&
                    grid.Parent is VerticalStackLayout verti)
                {

                    MainViewModel mainViewModel = this.BindingContext as MainViewModel;
                    if (mainViewModel.SelectedElement.GeneralMass.VisibleFrameWidthLeft.Selected.Value)
                    {
                        ScrollManager.ScrollViewToY(this, verti.Y);
                    }
                    else
                    {
                        ScrollManager.ScrollViewToTop(this);
                    }
                }
            }
            catch
            {
                // Exception abfangen ohne Meldung
                // App läuft einfach weiter
            }
        }

        private void AluminiumGeneralMass_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (sender is Button button &&
                    button.Parent is Grid grid &&
                    grid.Parent is VerticalStackLayout verti &&
                    verti.Parent is VerticalStackLayout vertii)
                {

                    MainViewModel mainViewModel = this.BindingContext as MainViewModel;
                    if (mainViewModel.SelectedElement.AluminumWindowSill.DeepBackBow.Selected.Value)
                    {
                        ScrollManager.ScrollViewToY(this, vertii.Y);
                    }
                    else
                    {
                        ScrollManager.ScrollViewToTop(this);
                    }
                }
            }
            catch
            {
                // Exception abfangen ohne Meldung
                // App läuft einfach weiter
            }
        }
    }
}