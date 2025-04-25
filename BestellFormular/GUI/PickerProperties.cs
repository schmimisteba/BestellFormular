namespace BestellFormular.GUI
{
    /// <summary>
    /// Provides attached properties for the Picker control.
    /// </summary>
    public static class PickerProperties
    {
        /// <summary>
        /// Attached property for storing a field name associated with a Picker control.
        /// </summary>
        public static readonly BindableProperty FieldNameProperty =
            BindableProperty.CreateAttached("FieldName", typeof(string), typeof(PickerProperties), null);

        /// <summary>
        /// Gets the field name associated with the specified view.
        /// </summary>
        /// <param name="view">The view from which to get the field name.</param>
        /// <returns>The field name associated with the view.</returns>
        public static string GetFieldName(BindableObject view)
        {
            return (string)view.GetValue(FieldNameProperty);
        }

        /// <summary>
        /// Sets the field name for the specified view.
        /// </summary>
        /// <param name="view">The view to which the field name will be attached.</param>
        /// <param name="value">The field name to attach to the view.</param>
        public static void SetFieldName(BindableObject view, string value)
        {
            view.SetValue(FieldNameProperty, value);
        }
    }
}