using BestellFormular.Resources.Language;

namespace BestellFormular.Models.Manager
{
    public static class PopUpManager
    {
        /// <summary>
        /// Displays a confirmation popup with a title, message, and "Yes"/"No" options.
        /// </summary>
        /// <param name="titleKey">The resource key for the popup title.</param>
        /// <param name="messageKey">The resource key for the popup message.</param>
        /// <param name="language">The language code for localization.</param>
        /// <returns>A task returning true if the user confirms, otherwise false.</returns>
        public static async Task<bool> ShowConfirmationPopup(string title, string message)
        {
            return await Application.Current.MainPage.DisplayAlert(title, message, Resource.ConfirmButton, Resource.CancelButton);
        }

        /// <summary>
        /// Displays a save confirmation popup with options to save or discard changes.
        /// </summary>
        /// <param name="titleKey">The resource key for the popup title.</param>
        /// <param name="messageKey">The resource key for the popup message.</param>
        /// <param name="language">The language code for localization.</param>
        /// <returns>A task returning true if the user chooses to save, otherwise false.</returns>
        public static async Task<bool> ShowSavePopup(string title, string message)
        {
            return await Application.Current.MainPage.DisplayAlert(title, message, Resource.ConfirmSave, Resource.DeclientSave);
        }
    }
}
