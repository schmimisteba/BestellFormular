using CommunityToolkit.Mvvm.ComponentModel;
using BestellFormular.Models.Helper;
using BestellFormular.Models.Window;

namespace BestellFormular.Models
{
    /// <summary>
    /// Controller that manages the buttons and their associated fields in the application.
    /// </summary>
    public partial class ButtonsController : ObservableObject
    {
        // Dictionary to store button fields by their ID.
        private readonly Dictionary<string, Field> _buttonDictionary = new();
        private readonly ExcelLoader _excelLoader;

        // Observable properties for the buttons and products.
        [ObservableProperty]
        private Field addButton;

        [ObservableProperty]
        private Field imgButton;

        [ObservableProperty]
        private Field adressButton;

        [ObservableProperty]
        private Field deleteButton;

        [ObservableProperty]
        private Field copyButton;

        [ObservableProperty]
        private Field sketchButton;

        [ObservableProperty]
        private Field loadButton;

        [ObservableProperty]
        private Field newButton;

        [ObservableProperty]
        private Field saveButton;

        [ObservableProperty]
        private Field saveAsButton;

        [ObservableProperty]
        private Field sendButton;

        [ObservableProperty]
        private Field openFileButton;

        [ObservableProperty]
        private Field fileMenuButton;

        [ObservableProperty]
        private Field productButton;

        [ObservableProperty]
        private Field expertModus;

        [ObservableProperty]
        private Field products;

        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonsController"/> class.
        /// </summary>
        /// <param name="excelHelper">The helper to load fields from Excel.</param>
        public ButtonsController(ExcelLoader excelLoader)
        {
            this._excelLoader = excelLoader;
            InitializeFields(); // Load buttons after initialization
        }

        /// <summary>
        /// Loads all button fields using their respective IDs and stores them in the button dictionary.
        /// </summary>
        private void InitializeFields()
        {
            AddButton = InitializeFields("ID8000");  // Load add button
            ImgButton = InitializeFields("ID8001");  // Load image button
            AdressButton = InitializeFields("ID8002");  // Load address button
            DeleteButton = InitializeFields("ID8004");  // Load delete button
            CopyButton = InitializeFields("ID8005");  // Load copy button
            SketchButton = InitializeFields("ID8006");  // Load sketch button
            Products = InitializeFields("ID0022");  // Load products button
            NewButton = InitializeFields("ID8014");  // Load new button
            LoadButton = InitializeFields("ID8008");
            SaveButton = InitializeFields("ID8009");
            SaveAsButton = InitializeFields("ID8010");
            SendButton = InitializeFields("ID8011");
            OpenFileButton = InitializeFields("ID8012");
            FileMenuButton = InitializeFields("ID8013");
            ProductButton = InitializeFields("ID8015");
            ExpertModus = InitializeFields("ID8016");
        }

        /// <summary>
        /// Loads a field from Excel using its ID and adds it to the dictionary.
        /// </summary>
        /// <param name="id">The ID of the button field to load.</param>
        /// <returns>The loaded button field.</returns>
        private Field InitializeFields(string id)
        {
            // Load the field by its ID from Excel and add it to the dictionary.
            _buttonDictionary[id] = _excelLoader.GetFieldById(id);
            return _buttonDictionary[id];  // Return the loaded field
        }

        /// <summary>
        /// Updates the language for all button fields.
        /// </summary>
        /// <param name="language">The new language to set for the buttons.</param>
        public void UpdateLanguage(string language)
        {
            // Iterate through all the button fields and update their language.
            foreach (var button in _buttonDictionary.Values)
            {
                button.UpdateLanguage(language);
            }
        }
    }
}
