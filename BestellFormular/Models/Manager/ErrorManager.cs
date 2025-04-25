using BestellFormular.Resources.Language;

namespace BestellFormular.Models.Manager
{
    /// <summary>
    /// Manages error messages throughout the application.
    /// </summary>
    public static class ErrorManager
    {
        private static readonly ErrorMessage _currentError = new(ErrorRed);

        /// <summary>
        /// Gets the current error message.
        /// </summary>
        public static ErrorMessage CurrentError => _currentError;

        /// <summary>
        /// Gets the standard error color from application resources.
        /// </summary>
        public static Color ErrorRed => GetResourceColor("ErrorRed");

        /// <summary>
        /// Gets the warning color from application resources.
        /// </summary>
        public static Color WarningYellow => GetResourceColor("WarningYellow");

        /// <summary>
        /// Sets an error with the specified title, message, and optional description.
        /// </summary>
        /// <param name="title">The error title.</param>
        /// <param name="message">The error message.</param>
        /// <param name="description">The detailed error description (optional).</param>
        /// <param name="color">The color indicator for the error: "red" for errors, any other value for warnings (default is "red").</param>
        public static void SetError(string title, string message, string description = "", string color = "red")
        {
            if (!_currentError.Message.Contains(message.Substring(0,10)))
            {
                _currentError.Title = title;
                _currentError.Message += message;
                _currentError.Description = description;
                _currentError.ErrorColor = color.Equals("red", StringComparison.OrdinalIgnoreCase) ? ErrorRed : WarningYellow;
            }
        }

        /// <summary>
        /// Clears the current error message.
        /// </summary>
        public static void ClearError()
        {
            _currentError.Title = string.Empty;
            _currentError.Message = string.Empty;
            _currentError.Description = string.Empty;
            _currentError.ErrorColor = ErrorRed;
        }

        /// <summary>
        /// Displays an error message dialog with localized text.
        /// </summary>
        /// <param name="errorMessageKey">Resource key for the error message.</param>
        /// <param name="language">Current language code.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        internal static async Task ShowErrorMessage(string errorMessageKey)
        {
            string errorMessage = errorMessageKey;
            string errorTitle = Resource.Error;
            string okButton = Resource.OkButton;

            await Application.Current.MainPage.DisplayAlert(errorTitle, errorMessage, okButton);
        }

        /// <summary>
        /// Handles an error by either displaying a popup or setting the current error.
        /// </summary>
        /// <param name="errorMessageKey">Resource key for the error message.</param>
        /// <param name="language">Current language code.</param>
        /// <param name="ex">Optional exception that caused the error.</param>
        /// <param name="isPopup">If true, displays a popup; otherwise sets the current error.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        internal static async Task HandleErrorMessage(string message, Exception? ex = null, bool isPopup = false)
        {
            if (ex != null)
            {
                message += " " + ex.Message;
            }

            if (isPopup)
            {
                await ShowErrorMessage(message);
            }
            else
            {
                SetError(
                    Resource.Error,
                    message
                );
            }
        }

        /// <summary>
        /// Retrieves a color from application resources.
        /// </summary>
        /// <param name="key">The resource key for the color.</param>
        /// <returns>The requested color or transparent if not found.</returns>
        private static Color GetResourceColor(string key)
        {
            return Application.Current.Resources.TryGetValue(key, out var color) && color is Color resourceColor
                ? resourceColor
                : Colors.Transparent;
        }
    }
}