using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BestellFormular.Models.Window
{
    /// <summary>
    /// Represents a field in the form, with properties and translations.
    /// </summary>
    public partial class Field : ObservableObject
    {
        // Observable properties automatically implement INotifyPropertyChanged
        [ObservableProperty]
        private string id;

        [ObservableProperty]
        private string product;

        [ObservableProperty]
        private string titel;

        [ObservableProperty]
        private string value;

        [ObservableProperty]
        private string oldShortCut;

        [ObservableProperty]
        private string image;

        [ObservableProperty]
        private string accessoriesGroup;

        [ObservableProperty]
        private string groupName;

        [ObservableProperty]
        private bool? selected;

        [ObservableProperty]
        private bool enabled;

        [ObservableProperty]
        private string fieldInput;

        [ObservableProperty]
        private ObservableCollection<string> fieldInputSelection;

        // Dictionaries for translations
        public Dictionary<string, string> TitleTranslations { get; set; } = new();
        public Dictionary<bool, string> ShortCuts { get; set; } = new();
        public Dictionary<bool, string> Images { get; set; } = new();
        public Dictionary<string, string> GroupNameTranslations { get; set; } = new();
        public Dictionary<string, List<string>> FieldInputSelectionTranslations { get; set; } = new();

        /// <summary>
        /// Constructor to initialize the field with an ID.
        /// </summary>
        /// <param name="id">The unique identifier of the field.</param>
        public Field(string id)
        {
            this.id = id;
        }

        /// <summary>
        /// Checks if the current field is equal to another field based on their value.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>True if values match; otherwise false.</returns>
        public override bool Equals(object obj)
        {
            return obj is Field field && value == field.value;
        }

        /// <summary>
        /// Generates a hash code based on the field's value.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(value);
        }

        /// <summary>
        /// Creates a deep copy of the current field.
        /// </summary>
        /// <returns>A new instance of Field with the same data.</returns>
        public Field Copy()
        {
            var copy = new Field(this.Id)
            {
                Product = this.Product,
                Titel = this.Titel,
                Value = this.Value,
                OldShortCut = this.OldShortCut,
                Image = this.Image,
                AccessoriesGroup = this.AccessoriesGroup,
                GroupName = this.GroupName,
                FieldInput = this.FieldInput,
                FieldInputSelection = this.FieldInputSelection,
                Selected = this.Selected,
                Enabled = this.Enabled
            };

            // Deep copy translations
            foreach (var kvp in this.TitleTranslations)
            {
                copy.TitleTranslations[kvp.Key] = kvp.Value;
            }

            foreach (var kvp in this.GroupNameTranslations)
            {
                copy.GroupNameTranslations[kvp.Key] = kvp.Value;
            }

            foreach (var kvp in this.FieldInputSelectionTranslations)
            {
                copy.FieldInputSelectionTranslations[kvp.Key] = new List<string>(kvp.Value);
            }

            foreach (var kvp in this.ShortCuts)
            {
                copy.ShortCuts[kvp.Key] = kvp.Value;
            }

            foreach (var kvp in this.Images)
            {
                copy.Images[kvp.Key] = kvp.Value;
            }

            return copy;
        }

        /// <summary>
        /// Updates field values based on the selected language.
        /// </summary>
        /// <param name="sprache">The language code.</param>
        public void UpdateLanguage(string sprache)
        {
            if (TitleTranslations.TryGetValue(sprache, out var titelValue))
            {
                Titel = titelValue;
            }

            if (GroupNameTranslations.TryGetValue(sprache, out var groupValue))
            {
                GroupName = groupValue;
            }

            if (FieldInputSelectionTranslations.TryGetValue(sprache, out var fieldInputValue))
            {
                FieldInputSelection = fieldInputValue.ToObservableCollection<string>();
            }
        }

        public void UpdateShortCut(bool isOld)
        {
            if (ShortCuts.TryGetValue(isOld, out var shortCutValue))
            {
                OldShortCut = shortCutValue;
            }
        }

        public void UpdateImage(bool isOld)
        {
            if (Images.TryGetValue(isOld, out var imageValue))
            {
                Image = imageValue;
            }
        }
    }
}