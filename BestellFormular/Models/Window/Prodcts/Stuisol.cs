using System.Reflection;
using BestellFormular.Models.Helper;
using BestellFormular.Resources.Language;
using CommunityToolkit.Mvvm.ComponentModel;
using DocumentFormat.OpenXml.Drawing.Charts;

namespace BestellFormular.Models.Window.Product
{
    /// <summary>
    /// Represents a Stuisol product, which is a type of window element with configurable properties.
    /// </summary>
    public partial class Stuisol : ProductBase
    {
        /// <summary>
        /// Gets or sets the field for the material.
        /// </summary>
        [ObservableProperty]
        private Field material;

        /// <summary>
        /// Gets or sets the field for the width of the product.
        /// </summary>
        [ObservableProperty]
        private Field wide;

        /// <summary>
        /// Gets or sets the field for the thickness of the material.
        /// </summary>
        [ObservableProperty]
        private Field thickness;

        /// <summary>
        /// Gets or sets the field for the length of the element.
        /// </summary>
        [ObservableProperty]
        private Field lengthElement;

        /// <summary>
        /// Gets or sets the field for the thickness of the material.
        /// </summary>
        [ObservableProperty]
        private Field length;

        [ObservableProperty] private Field execution;

        /// <summary>
        /// Initializes a new instance of the Stuisol class with the provided window element.
        /// </summary>
        /// <param name="windowElement">The window element associated with the product.</param>
        /// <exception cref="ArgumentNullException">Thrown when the window element is null.</exception>
        public Stuisol(WindowElement windowElement) : base(windowElement)
        {
            if (windowElement == null)
                throw new ArgumentNullException(nameof(windowElement), "WindowElement cannot be null");
        }

        /// <summary>
        /// Initializes a new instance of the Stuisol class with the provided window element and Excel helper.
        /// </summary>
        /// <param name="windowElement">The window element associated with the product.</param>
        /// <param name="excelLoader">An Excel helper for retrieving field values.</param>
        /// <exception cref="ArgumentNullException">Thrown when the window element is null.</exception>
        public Stuisol(WindowElement windowElement, ExcelLoader excelLoader) : base(windowElement)
        {
            if (windowElement == null)
                throw new ArgumentNullException(nameof(windowElement), "WindowElement cannot be null");

            InitializeFields(excelLoader);
            RegisterEvents(windowElement);
        }

        /// <summary>
        /// Initializes the fields for the Stuisol product using the provided Excel helper.
        /// </summary>
        /// <param name="excelLoader">The Excel helper to retrieve field values.</param>
        private void InitializeFields(ExcelLoader excelLoader)
        {
            Product = excelLoader.GetFieldById("ID0019");
            Pos = excelLoader.GetFieldById("ID0458");
            Material = excelLoader.GetFieldById("ID0459");
            Count = excelLoader.GetFieldById("ID0460");
            Wide = excelLoader.GetFieldById("ID0461");
            Thickness = excelLoader.GetFieldById("ID0462");
            Length = excelLoader.GetFieldById("ID0449");
            LengthElement = excelLoader.GetFieldById("ID0463");
            Execution = excelLoader.GetFieldById("ID0464");
            Comment = excelLoader.GetFieldById("ID0130");
        }

        /// <summary>
        /// Registers event handlers for changes in the position field.
        /// </summary>
        /// <param name="windowElement">The window element associated with the product.</param>
        private void RegisterEvents(WindowElement windowElement)
        {
            RegisterPosChangeHandler(Pos);

            //Disable automatisch berechnen
            Pos.PropertyChanged += (s, e) => EventCalculator.CalculateAll(e, windowElement);
            Length.PropertyChanged += (s, e) => EventCalculator.CalculateAll(e, windowElement);
            Wide.PropertyChanged += (s, e) => EventCalculator.CalculateAll(e, windowElement);

            Material.PropertyChanged += (s, e) => HandleSystemPropertyChanged(e, Material.Value, LengthElement, Material.FieldInputSelection.ToArray());

            Execution.PropertyChanged += (s, e) => ClearSurchargeChangeHandler(e, Execution, [Execution], Resource.No);
        }

        /// <summary>
        /// Creates a copy of the current Stuisol instance.
        /// </summary>
        /// <param name="windowElement">The window element associated with the copied product.</param>
        /// <returns>A new instance of Stuisol with the same properties as the current one.</returns>
        public override Stuisol Copy(WindowElement windowElement)
        {
            var copy = new Stuisol(windowElement)
            {
                Product = Product.Copy(),
                Pos = Pos.Copy(),
                Material = Material.Copy(),
                Selected = this.Selected,
                Count = Count.Copy(),
                Column = this.Column,
                Wide = Wide.Copy(),
                Thickness = Thickness.Copy(),
                Length = Length.Copy(),
                LengthElement = this.LengthElement?.Copy(),
                Execution = this.Execution?.Copy(),
                Comment = Comment.Copy()
            };

            copy.RegisterEvents(windowElement);
            return copy;
        }

        /// <summary>
        /// Updates the language for all fields in the product.
        /// </summary>
        /// <param name="language">The language to update the fields to.</param>
        public void UpdateFields(string language)
        {
            var properties = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(Field))
                {
                    var field = property.GetValue(this) as Field;
                    field?.UpdateLanguage(language);
                }
            }
        }

        public static void CountToElement(List<Stuisol> products)
        {
            foreach (Stuisol supportAngleWithSlope in products)
            {
                double length = ParseCount(supportAngleWithSlope.Length.Value);
                double elementLength = ParseCount(supportAngleWithSlope.LengthElement.Value);
                double count = ParseCount(supportAngleWithSlope.Count.Value);

                if (length < elementLength)
                {
                    supportAngleWithSlope.Count.Value = (1 * count).ToString();
                }
                else
                {
                    supportAngleWithSlope.Count.Value = Math.Ceiling(length / elementLength * count).ToString();
                }

            }
        }

        protected override string GenerateFilterKey()
        {
            return $"{Product.Value}{Pos.Value}{Material.Value}{Wide.Value}{Thickness.Value}{Length.Value}";
        }

        public override void SetGeneralMass(WindowElement windowElement)
        {
            windowElement.GeneralMass.WindowLightWidth.Selected = true;

            SetEnableEntries(false);
        }

        public override void SetEnableEntries(bool enable)
        {
            Pos.Enabled = enable;
            Length.Enabled = enable;
            Wide.Enabled = enable;
        }
        public void CalculateLenght(string windowLightWidth, string visibleFrameWidthLeft, string visibleFrameWidthRight)
        {
            int windowLight = ParseCount(windowLightWidth);
            int visibleLeft = ParseCount(visibleFrameWidthLeft);
            int visibleRight = ParseCount(visibleFrameWidthRight);

            if (!Length.Enabled)
            {
                Length.Value = (windowLight + visibleLeft + visibleRight).ToString();
            }
        }

        public void CalculateWide(string thickFacade, string apronThickness)
        {
            if (!Wide.Enabled)
            {
                Wide.Value = (ParseCount(thickFacade) - ParseCount(apronThickness)).ToString();
            }
        }
    }
}