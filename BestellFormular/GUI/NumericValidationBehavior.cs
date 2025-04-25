namespace BestellFormular.GUI
{
    /// <summary>
    /// A behavior that validates numeric input for an Entry field.
    /// Changes the background color based on validation result.
    /// </summary>
    public class NumericValidationBehavior : Behavior<Entry>
    {
        /// <summary>
        /// Attaches the behavior to the Entry and subscribes to the TextChanged event.
        /// </summary>
        /// <param name="entry">The Entry control to attach to.</param>
        protected override void OnAttachedTo(Entry entry)
        {
            entry.TextChanged += OnEntryTextChanged;
            base.OnAttachedTo(entry);
        }

        /// <summary>
        /// Detaches the behavior from the Entry and unsubscribes from the TextChanged event.
        /// </summary>
        /// <param name="entry">The Entry control to detach from.</param>
        protected override void OnDetachingFrom(Entry entry)
        {
            entry.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(entry);
        }

        /// <summary>
        /// Validates the text input and changes the background color based on whether the input is numeric.
        /// </summary>
        /// <param name="sender">The Entry control sending the event.</param>
        /// <param name="args">The text changed event arguments.</param>
        private void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            // Check if app is in dark mode
            bool isDarkMode = Application.Current.RequestedTheme == AppTheme.Dark;

            ((Entry)sender).TextColor = isDarkMode
                ? GetResourceColor("White")
                : GetResourceColor("Black");

            if (string.IsNullOrEmpty(args.NewTextValue))
            {
                ((Entry)sender).BackgroundColor = isDarkMode
                    ? Colors.Transparent
                    : GetResourceColor("White");
                return;
            }

            // Checks if the new text is a valid decimal number
            bool isValid = decimal.TryParse(args.NewTextValue, out _);

            if (isValid)
            {
                ((Entry)sender).BackgroundColor = isDarkMode
                    ? Colors.Transparent
                    : GetResourceColor("White");
            }
            else
            {
                ((Entry)sender).BackgroundColor = GetResourceColor("ErrorRed");
            }
        }

        /// <summary>
        /// Retrieves a color from application resources.
        /// </summary>
        /// <param name="key">The resource key for the color.</param>
        /// <returns>The requested color.</returns>
        private static Color GetResourceColor(string key)
        {
            return Application.Current.Resources.TryGetValue(key, out var color) && color is Color resourceColor
                ? resourceColor
                : Colors.Transparent;
        }
    }
}