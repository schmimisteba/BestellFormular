using BestellFormular.Models.Helper;
using BestellFormular.Models.Manager;
using BestellFormular.Resources.Language;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Reflection;

namespace BestellFormular.Models.Window
{
    /// <summary>
    /// Abstract base class for product models, providing common properties and functionality.
    /// </summary>
    public abstract partial class ProductBase : ObservableObject
    {
        // Observable properties for various ProductBas element characteristics
        [ObservableProperty]
        private int column = 0;

        [ObservableProperty]
        private bool selected = false;

        [ObservableProperty]
        private bool surcharges = false;

        [ObservableProperty]
        private Field pos;

        [ObservableProperty]
        private Field product;

        [ObservableProperty]
        private Field count;

        [ObservableProperty]
        private Field comment;

        /// <summary>
        /// Reference to the parent window element.
        /// </summary>
        protected WindowElement ParentWindowElement { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductBase"/> class.
        /// </summary>
        /// <param name="windowElement">The parent window element.</param>
        protected ProductBase(WindowElement windowElement = null)
        {
            ParentWindowElement = windowElement;
        }

        /// <summary>
        /// Handles changes to the position field.
        /// </summary>
        /// <param name="e">Property change event arguments.</param>
        /// <param name="windowElement">The parent window element.</param>
        protected void HandlePosChange(System.ComponentModel.PropertyChangedEventArgs e, WindowElement windowElement)
        {
            if (e.PropertyName == nameof(Field.Value))
            {
                windowElement.SetProductPositionString();
            }
        }

        /// <summary>
        /// Handles changes to miter fields based on specific value patterns.
        /// </summary>
        /// <param name="e">Property change event arguments.</param>
        /// <param name="value">The updated field value.</param>
        /// <param name="outsideMiter">The outside miter field.</param>
        /// <param name="insideMiter">The inside miter field.</param>
        protected void HandleMiterChange(System.ComponentModel.PropertyChangedEventArgs e, string value, Field outsideMiter, Field insideMiter)
        {
            if (e.PropertyName == nameof(Field.Value))
            {
                value = StringExtensions.GetBetween(value, "[", "]") ?? value;
                (outsideMiter.Selected, insideMiter.Selected) = value switch
                {
                    "GA" => (true, false),
                    "GI" => (false, true),
                    _ => (false, false)
                };
            }
        }

        /// <summary>
        /// Handles general property changes and updates target field selection.
        /// </summary>
        /// <param name="e">Property change event arguments.</param>
        /// <param name="value">The updated field value.</param>
        /// <param name="targetField">The field to update.</param>
        /// <param name="trueValues">The set of values that indicate a true state.</param>
        protected void HandleSystemPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e, string value, Field targetField, string[] trueValues)
        {
            if (e.PropertyName == nameof(Field.Value))
            {
                value = StringExtensions.GetBetween(value, "[", "]") ?? value;

                if (value == "XPS")
                {
                    targetField.Value = "1250";
                }
                else
                {
                    targetField.Value = "1000";
                }
                targetField.Selected = trueValues.Contains(value) ? true : false;
            }
        }

        /// <summary>
        /// Handles general property changes and updates target field selection.
        /// </summary>
        /// <param name="e">Property change event arguments.</param>
        /// <param name="value">The updated field value.</param>
        /// <param name="targetField">The field to update.</param>
        /// <param name="trueValues">The set of values that indicate a true state.</param>
        protected void HandlePropertyChanged(System.ComponentModel.PropertyChangedEventArgs e, string value, Field targetField, string[] trueValues)
        {
            if (e.PropertyName == nameof(Field.Value))
            {
                value = StringExtensions.GetBetween(value, "[", "]") ?? value;
                targetField.Selected = trueValues.Contains(value) ? true : false;
            }
        }

        /// <summary>
        /// Registers a change handler for miter fields.
        /// </summary>
        /// <param name="connectionField">The field representing the connection.</param>
        /// <param name="miterOutside">The outside miter field.</param>
        /// <param name="miterInside">The inside miter field.</param>
        protected void RegisterMiterChangeHandler(Field connectionField, Field miterOutside, Field miterInside)
        {
            connectionField.PropertyChanged += (s, e) => HandleMiterChange(e, connectionField.Value, miterOutside, miterInside);
        }

        /// <summary>
        /// Registers a change handler for material fields.
        /// </summary>
        /// <param name="materialField">The material field.</param>
        /// <param name="colorField">The color field.</param>
        /// <param name="materialCodes">The set of valid material codes.</param>
        protected void RegisterMaterialChangeHandler(Field materialField, Field colorField, string[] materialCodes)
        {
            materialField.PropertyChanged += (s, e) => HandlePropertyChanged(e, materialField.Value, colorField, materialCodes);
        }

        /// <summary>
        /// Registers a change handler for position fields.
        /// </summary>
        /// <param name="posField">The position field.</param>
        protected void RegisterPosChangeHandler(Field posField)
        {
            posField.PropertyChanged += (s, e) => HandlePosChange(e, ParentWindowElement);
        }

        /// <summary>
        /// Registers a change handler for position fields.
        /// </summary>
        /// <param name="posField">The position field.</param>
        protected async Task ClearSurchargeChangeHandler(PropertyChangedEventArgs e, Field field, List<Field> fields, string defautlValue = "")
        {
            if (e.PropertyName == "Selected")
            {
                if (!field.Selected.Value)
                {
                    foreach (Field f in fields)
                    {
                        if (f.Value != defautlValue)
                        {
                            var answer = await PopUpManager.ShowConfirmationPopup(Resource.RemoveSurchargeTitel, Resource.RemoveSurchargeMessage);
                            if (answer)
                            {
                                foreach (Field ff in fields)
                                {
                                    ff.Value = defautlValue;
                                }
                            }
                            else
                            {
                                field.Selected = true;
                            }
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates a copy of the current product with a new parent window element.
        /// </summary>
        /// <param name="windowElement">The new parent window element.</param>
        /// <returns>A new product instance.</returns>
        public abstract ProductBase Copy(WindowElement windowElement);

        /// <summary>
        /// Abstract method to generate a unique key for filtering and consolidation.
        /// Each derived class must implement its own key generation logic.
        /// </summary>
        /// <returns>A unique string key representing the product's distinguishing characteristics.</returns>
        protected abstract string GenerateFilterKey();

        /// <summary>
        /// Abstract method to set the general mass of the product.
        /// </summary>
        public abstract void SetGeneralMass(WindowElement windowElement);

        public abstract void SetEnableEntries(bool enable);

        /// <summary>
        /// Generic method to filter and consolidate elements with the same characteristics.
        /// </summary>
        /// <typeparam name="T">The type of product, which must inherit from ProductBase</typeparam>
        /// <param name="products">List of products to filter</param>
        /// <returns>A consolidated and filtered list of products</returns>
        public static List<T> FilterElements<T>(List<T> products) where T : ProductBase
        {
            if (products == null)
                return new List<T>();

            var consolidatedProducts = new Dictionary<string, T>();

            foreach (var product in products)
            {
                string fieldKey = product.GenerateFilterKey();

                if (consolidatedProducts.TryGetValue(fieldKey, out T existingProduct))
                {
                    // Merge counts
                    int currentCount = ParseCount(existingProduct.Count.Value);
                    int newCount = ParseCount(product.Count.Value);
                    existingProduct.Count.Value = (currentCount + newCount).ToString();
                }
                else
                {
                    consolidatedProducts[fieldKey] = product;
                    consolidatedProducts[fieldKey].Count.Value =
                        string.IsNullOrEmpty(product.Count.Value) ? "1" : product.Count.Value;
                }
            }

            return consolidatedProducts.Values.ToList();
        }

        // Helper method to safely parse count values
        internal static int ParseCount(string countValue)
        {
            return string.IsNullOrEmpty(countValue)
                ? 0
                : int.TryParse(countValue, out int count)
                    ? count
                    : 0;
        }

        /// <summary>
        /// Updates the language of all Field properties.
        /// </summary>
        /// <param name="language">The language to update to.</param>
        public void UpdateFields(string language)
        {
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(Field))
                {
                    var field = property.GetValue(this) as Field;
                    field?.UpdateLanguage(language);
                }
            }
        }

        public void UpdateFields(bool? isOld)
        {
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(Field))
                {
                    var field = property.GetValue(this) as Field;
                    field?.UpdateShortCut(isOld.Value);
                    field?.UpdateImage(isOld.Value);
                }
            }
        }

        /// <summary>
        /// Retrieves the field values of the product as a dictionary.
        /// </summary>
        /// <returns>A dictionary containing field names and their values.</returns>
        public Dictionary<string, string> GetFieldValues()
        {
            var values = new Dictionary<string, string>();

            foreach (var prop in this.GetType().GetProperties())
            {
                if (typeof(Field).IsAssignableFrom(prop.PropertyType))
                {
                    var field = prop.GetValue(this) as Field;
                    if (field != null)
                    {
                        values[field.Id] = field.Value;
                    }
                }
                else if (prop.PropertyType == typeof(int))
                {
                    values[prop.Name] = prop.GetValue(this)?.ToString();
                }
                else if (prop.PropertyType == typeof(bool))
                {
                    values[prop.Name] = prop.GetValue(this)?.ToString().ToLower();
                }
            }

            return values;
        }
    }
}
