using System.Collections.ObjectModel;

namespace BestellFormular.GUI
{
    /// <summary>
    /// Provides helper methods and attached properties for managing Grid column definitions in XAML.
    /// </summary>
    public static class GridHelper
    {
        /// <summary>
        /// An attached property that allows binding an ObservableCollection of ColumnDefinition objects to a Grid.
        /// </summary>
        public static readonly BindableProperty ColumnDefinitionsProperty =
            BindableProperty.CreateAttached(
                "ColumnDefinitions",
                typeof(ObservableCollection<ColumnDefinition>),
                typeof(GridHelper),
                new ObservableCollection<ColumnDefinition>(),
                propertyChanged: OnColumnDefinitionsChanged);

        /// <summary>
        /// Gets the collection of column definitions attached to a given view.
        /// </summary>
        /// <param name="view">The view from which to get the column definitions.</param>
        /// <returns>The attached ObservableCollection of ColumnDefinition objects.</returns>
        public static ObservableCollection<ColumnDefinition> GetColumnDefinitions(BindableObject view) =>
            (ObservableCollection<ColumnDefinition>)view.GetValue(ColumnDefinitionsProperty);

        /// <summary>
        /// Sets the collection of column definitions on a given view.
        /// </summary>
        /// <param name="view">The view to which the column definitions will be attached.</param>
        /// <param name="value">The ObservableCollection of ColumnDefinition objects.</param>
        public static void SetColumnDefinitions(BindableObject view, ObservableCollection<ColumnDefinition> value) =>
            view.SetValue(ColumnDefinitionsProperty, value);

        /// <summary>
        /// Called when the ColumnDefinitions property changes. Updates the grid's columns accordingly.
        /// </summary>
        /// <param name="bindable">The bindable object (expected to be a Grid).</param>
        /// <param name="oldValue">The old collection of columns.</param>
        /// <param name="newValue">The new collection of columns.</param>
        private static void OnColumnDefinitionsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is Grid grid && newValue is ObservableCollection<ColumnDefinition> columns)
            {
                grid.ColumnDefinitions.Clear();
                foreach (var column in columns)
                {
                    grid.ColumnDefinitions.Add(column);
                }
            }
        }
    }
}