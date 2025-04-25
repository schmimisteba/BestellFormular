using CommunityToolkit.Mvvm.ComponentModel;

namespace BestellFormular.Models
{
    /// <summary>
    /// Represents an error message with details like title, description, and error state.
    /// </summary>
    public partial class ErrorMessage : ObservableObject
    {
        /// <summary>
        /// Indicates whether an error is present.
        /// </summary>
        [ObservableProperty]
        private bool hasError;

        /// <summary>
        /// The main message of the error.
        /// </summary>
        [ObservableProperty]
        private string message;

        /// <summary>
        /// The title of the error message.
        /// </summary>
        [ObservableProperty]
        private string title;

        /// <summary>
        /// The title of the error message.
        /// </summary>
        [ObservableProperty]
        private Color errorColor;

        /// <summary>
        /// A more detailed description of the error.
        /// </summary>
        [ObservableProperty]
        private string description;

        /// <summary>
        /// Invoked when the Message property changes.
        /// Updates the HasError flag based on whether the message is empty or not.
        /// </summary>
        /// <param name="oldValue">The previous message value.</param>
        /// <param name="newValue">The new message value.</param>
        partial void OnMessageChanged(string? oldValue, string newValue)
        {
            HasError = !string.IsNullOrEmpty(newValue);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorMessage"/> class with default values.
        /// </summary>
        public ErrorMessage(Color errorColor)
        {
            HasError = false;
            Message = string.Empty;
            Title = string.Empty;
            ErrorColor = errorColor;
            Description = string.Empty;
        }
    }
}
