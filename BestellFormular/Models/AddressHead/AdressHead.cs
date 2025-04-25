using BestellFormular.Models.Helper;
using BestellFormular.Models.Manager;
using BestellFormular.Models.Window;
using BestellFormular.Resources.Language;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace BestellFormular.Models.AddressHead
{
    /// <summary>
    /// Represents the header of an address, containing various fields related to buyers, processors, deliveries, and invoice information.
    /// </summary>
    public partial class AdressHead : ObservableObject
    {
        private readonly Dictionary<string, Field> _adressHeadDictionary = new();
        private ExcelLoader _excelLoader;

        // Fields representing buyer, processor, delivery, and invoice information
        [ObservableProperty] private Field address;
        [ObservableProperty] private Field miscellaneous;

        [ObservableProperty] private Field buyerName;
        [ObservableProperty] private Field buyerStreet;
        [ObservableProperty] private Field buyerPlace;
        [ObservableProperty] private Field buyerPropertyHolder;
        [ObservableProperty] private Field buyerPhone;
        [ObservableProperty] private Field buyerEmail;
        public Field BuyerDebitorNr { get; set; }
        public Field BuyerContactNr { get; set; }

        [ObservableProperty] private Field processorName;
        [ObservableProperty] private Field processorStreet;
        [ObservableProperty] private Field processorPlace;
        [ObservableProperty] private Field processorPropertyHolder;
        [ObservableProperty] private Field processorPhone;
        [ObservableProperty] private Field processorEmail;
        [ObservableProperty] private Field processorLikeBuyer;
        public Field ProcessorDebitorNr { get; set; }
        public Field ProcessorContactNr { get; set; }

        [ObservableProperty] private Field deliveryName;
        [ObservableProperty] private Field deliveryStreet;
        [ObservableProperty] private Field deliveryPlace;
        [ObservableProperty] private Field deliveryLikeBuyer;
        public Field DeliverDebitorNr { get; set; }
        public Field DeliverContactNr { get; set; }

        [ObservableProperty] private Field commission;
        [ObservableProperty] private Field orderNumber;
        [ObservableProperty] private Field offerNumber;
        [ObservableProperty] private Field date;
        [ObservableProperty] private Field visum;
        [ObservableProperty] private Field objectType;

        [ObservableProperty] private Field wishDate;
        [ObservableProperty] private Field nameAvis;
        [ObservableProperty] private Field phoneAvis;
        [ObservableProperty] private Field onCall;
        [ObservableProperty] private Field willBeCollected;
        [ObservableProperty] private Field withoutTrailer;

        [ObservableProperty] private Field invoiceLikeBuyer;
        [ObservableProperty] private Field invoiceName;
        [ObservableProperty] private Field invoiceStreet;
        [ObservableProperty] private Field invoicePlace;
        [ObservableProperty] private Field invoiceCoordinates;

        [ObservableProperty] private Field remark;

        [ObservableProperty] private ObservableCollection<Address> addresses = new ObservableCollection<Address>();
        [ObservableProperty] private Address selectedAddress;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdressHead"/> class, setting up fields using the provided <see cref="excelLoader"/>.
        /// </summary>
        /// <param name="excelLoader">The helper that provides field data from Excel.</param>
        public AdressHead(ExcelLoader excelLoader)
        {
            _excelLoader = excelLoader;

            // Initialize all fields using the excelLoader
            InitializeTitelFields(excelLoader);
            InitializeBuyerFields(excelLoader);
            InitializeProcessorFields(excelLoader);
            InitializeDeliveryFields(excelLoader);
            InitializeOrderFields(excelLoader);
            InitializeInvoiceFields(excelLoader);
            InitializeOtherFields(excelLoader);
        }

        /// <summary>
        /// Loads a field from Excel using its ID and adds it to the dictionary.
        /// </summary>
        /// <param name="id">The ID of the button field to load.</param>
        /// <returns>The loaded button field.</returns>
        private Field InitializeField(string id)
        {
            // Load the field by its ID from Excel and add it to the dictionary.
            _adressHeadDictionary[id] = _excelLoader.GetFieldById(id);
            return _adressHeadDictionary[id];  // Return the loaded field
        }

        /// <summary>
        /// Initialize the titel fields.
        /// </summary>
        /// <param name="excelLoader"></param>
        private void InitializeTitelFields(ExcelLoader excelLoader)
        {
            Address = InitializeField("ID8055");
            Miscellaneous = InitializeField("ID8056");
        }

        /// <summary>
        /// Initializes the buyer-related fields.
        /// </summary>
        /// <param name="excelLoader">The helper that provides field data from Excel.</param>
        private void InitializeBuyerFields(ExcelLoader excelLoader)
        {
            BuyerName = InitializeField("ID8020");
            BuyerName.PropertyChanged += (s, e) => OnSearchChanged(e, "BuyerName", BuyerName);
            BuyerStreet = InitializeField("ID8021");
            BuyerPlace = InitializeField("ID8022");
            BuyerPropertyHolder = InitializeField("ID8023");
            BuyerPhone = InitializeField("ID8024");
            BuyerEmail = InitializeField("ID8025");

            BuyerDebitorNr = InitializeField("ID8057");
            BuyerContactNr = InitializeField("ID8058");
        }

        /// <summary>
        /// Initializes the processor-related fields and sets up event handlers.
        /// </summary>
        /// <param name="excelLoader">The helper that provides field data from Excel.</param>
        private void InitializeProcessorFields(ExcelLoader excelLoader)
        {
            ProcessorName = InitializeField("ID8026");
            ProcessorName.PropertyChanged += (s, e) => OnSearchChanged(e, "ProcessorName", ProcessorName);
            ProcessorStreet = InitializeField("ID8027");
            ProcessorPlace = InitializeField("ID8028");
            ProcessorPropertyHolder = InitializeField("ID8029");
            ProcessorPhone = InitializeField("ID8030");
            ProcessorEmail = InitializeField("ID8031");

            ProcessorLikeBuyer = InitializeField("ID8032");
            ProcessorLikeBuyer.PropertyChanged += (s, e) => SyncFields(e, ProcessorLikeBuyer, new[] { ProcessorName, ProcessorStreet, ProcessorPlace, ProcessorDebitorNr, ProcessorContactNr, ProcessorPropertyHolder, ProcessorPhone, ProcessorEmail });

            ProcessorDebitorNr = InitializeField("ID8059");
            ProcessorContactNr = InitializeField("ID8060");


        }

        /// <summary>
        /// Initializes the delivery-related fields and sets up event handlers.
        /// </summary>
        /// <param name="excelLoader">The helper that provides field data from Excel.</param>
        private void InitializeDeliveryFields(ExcelLoader excelLoader)
        {
            DeliveryName = InitializeField("ID8033");
            DeliveryName.PropertyChanged += (s, e) => OnSearchChanged(e, "DeliveryName", DeliveryName);
            DeliveryStreet = InitializeField("ID8034");
            DeliveryPlace = InitializeField("ID8035");
            DeliveryLikeBuyer = InitializeField("ID8036");

            DeliveryLikeBuyer.PropertyChanged += (s, e) => SyncFields(e, DeliveryLikeBuyer, new[] { DeliveryName, DeliveryStreet, DeliveryPlace, DeliverDebitorNr, DeliverContactNr });

            DeliverDebitorNr = InitializeField("ID8061");
            DeliverContactNr = InitializeField("ID8062");

        }

        /// <summary>
        /// Initializes the order-related fields.
        /// </summary>
        /// <param name="excelLoader">The helper that provides field data from Excel.</param>
        private void InitializeOrderFields(ExcelLoader excelLoader)
        {
            Commission = InitializeField("ID8040");
            OrderNumber = InitializeField("ID8041");
            OfferNumber = InitializeField("ID8042");
            Date = InitializeField("ID8043");
            Date.PropertyChanged += (s, e) => DateChanged(e, Date);
            Visum = InitializeField("ID8044");
            ObjectType = InitializeField("ID8045");
        }

        /// <summary>
        /// Handles changes to the date field and ensures it is formatted correctly.
        /// </summary>
        /// <param name="e">The event arguments containing the name of the changed property.</param>
        /// <param name="dateField">The field representing the date.</param>
        private void DateChanged(PropertyChangedEventArgs e, Field dateField)
        {
            if (e.PropertyName == nameof(Field.Value)) // Check if the Date field has changed
            {
                if (!string.IsNullOrEmpty(dateField?.Value) && DateTime.TryParse(dateField.Value, out var parsedDate))
                {
                    dateField.Value = parsedDate.ToString("dd.MM.yyyy"); // German-style date format
                }
                else
                {
                    // If the date is invalid, set the Date field to "Unbekannt" or something similar
                    dateField.Value = "Unbekannt";
                }
            }
        }

        /// <summary>
        /// Initializes the invoice-related fields.
        /// </summary>
        /// <param name="excelLoader">The helper that provides field data from Excel.</param>
        private void InitializeInvoiceFields(ExcelLoader excelLoader)
        {
            InvoiceLikeBuyer = InitializeField("ID8049");
            InvoiceLikeBuyer.PropertyChanged += (s, e) => SyncFields(e, InvoiceLikeBuyer, new[] { InvoiceName, InvoiceStreet, InvoicePlace });

            InvoiceName = InitializeField("ID8050");
            InvoiceStreet = InitializeField("ID8051");
            InvoicePlace = InitializeField("ID8052");
            InvoiceCoordinates = InitializeField("ID8053");
        }

        /// <summary>
        /// Initializes other fields that do not fall under buyer, processor, delivery, or invoice categories.
        /// </summary>
        /// <param name="excelLoader">The helper that provides field data from Excel.</param>
        private void InitializeOtherFields(ExcelLoader excelLoader)
        {
            WishDate = InitializeField("ID8046");
            WishDate.PropertyChanged += (s, e) => DateChanged(e, WishDate);
            NameAvis = InitializeField("ID8047");
            PhoneAvis = InitializeField("ID8048");

            OnCall = InitializeField("ID8037");
            WillBeCollected = InitializeField("ID8038");
            WithoutTrailer = InitializeField("ID8039");

            Remark = InitializeField("ID8054");
        }

        /// <summary>
        /// Synchronisiert Felder zwischen Käufer und Zielfeldern, wenn "LikeBuyer" auf "True" gesetzt ist.
        /// </summary>
        /// <param name="e">Die PropertyChanged-Ereignisargumente.</param>
        /// <param name="likeBuyerField">Das Feld, das angibt, ob synchronisiert werden soll.</param>
        /// <param name="targetFields">Die zu synchronisierenden Zielfelder.</param>
        private void SyncFields(System.ComponentModel.PropertyChangedEventArgs e, Field likeBuyerField, Field[] targetFields)
        {
            // Nur synchronisieren, wenn die Value-Eigenschaft geändert wurde und LikeBuyer true ist
            if (e.PropertyName == nameof(Field.Value) && likeBuyerField.Value == "True")
            {
                // Quellenfelder definieren
                var sourceFields = new[] { BuyerName, BuyerStreet, BuyerPlace, BuyerDebitorNr, BuyerContactNr, BuyerPropertyHolder, BuyerPhone, BuyerEmail };

                // Felder synchronisieren, mit Prüfung auf Array-Längen
                int fieldsToSync = Math.Min(sourceFields.Length, targetFields.Length);
                for (int i = 0; i < fieldsToSync; i++)
                {
                    targetFields[i].Value = sourceFields[i].Value;
                }

                // Auswahl zurücksetzen
                SetSelection(false);
            }
        }

        /// <summary>
        /// Behandelt Änderungen in Suchfeldern und aktualisiert entsprechend die Adressen.
        /// </summary>
        /// <param name="e">Die PropertyChanged-Ereignisargumente.</param>
        /// <param name="fieldName">Der Name des geänderten Feldes.</param>
        /// <param name="field">Das geänderte Feld.</param>
        private void OnSearchChanged(System.ComponentModel.PropertyChangedEventArgs e, string fieldName, Field field)
        {
            if (e.PropertyName != nameof(Field.Value)) return;

            // Auswahl zurücksetzen
            SetSelection(false);

            // Bei leerem Wert Adressen zurücksetzen
            if (string.IsNullOrEmpty(field?.Value))
            {
                Addresses = new ObservableCollection<Address>();
                field.Selected = false;
                return;
            }

            // Feldspezifische Auswahl setzen
            switch (fieldName)
            {
                case "BuyerName":
                    BuyerName.Selected = true;
                    break;
                case "ProcessorName":
                    ProcessorName.Selected = true;
                    break;
                case "DeliveryName":
                    DeliveryName.Selected = true;
                    break;
            }

            // Adressen aktualisieren
            Addresses = _excelLoader
                .GetAllAddressesStartWithString(field.Value)
                .Take(5)
                .ToObservableCollection<Address>();

            field.Selected = Addresses.Count == 0 ? false : true;
        }

        /// <summary>
        /// Behandelt Änderungen der ausgewählten Adresse und aktualisiert die entsprechenden Felder.
        /// </summary>
        /// <param name="oldValue">Die alte Adresse.</param>
        /// <param name="newValue">Die neue Adresse.</param>
        partial void OnSelectedAddressChanged(Address? oldValue, Address newValue)
        {
            if (newValue == null) return;

            if (BuyerName.Selected == true)
            {
                UpdateAddressFields(BuyerName, BuyerStreet, BuyerPlace, BuyerDebitorNr, null, newValue);
            }
            else if (ProcessorName.Selected == true)
            {
                UpdateAddressFields(ProcessorName, ProcessorStreet, ProcessorPlace, null, ProcessorContactNr, newValue);
            }
            else if (DeliveryName.Selected == true)
            {
                UpdateAddressFields(DeliveryName, DeliveryStreet, DeliveryPlace, DeliverDebitorNr, null, newValue);
            }

            SelectedAddress = null;
        }

        /// <summary>
        /// Aktualisiert die Adressfelder mit den Werten der ausgewählten Adresse.
        /// </summary>
        /// <param name="nameField">Das Namensfeld.</param>
        /// <param name="streetField">Das Straßenfeld.</param>
        /// <param name="placeField">Das Ortsfeld.</param>
        /// <param name="debitorField">Das Debitorfeld (kann null sein).</param>
        /// <param name="contactField">Das Kontaktfeld (kann null sein).</param>
        /// <param name="address">Die ausgewählte Adresse.</param>
        private void UpdateAddressFields(Field nameField, Field streetField, Field placeField, Field? debitorField, Field? contactField, Address address)
        {
            nameField.Value = address.Name;
            streetField.Value = address.Street;
            placeField.Value = address.Zip + " " + address.City;

            if (debitorField != null)
            {
                debitorField.Value = address.Debitor;
            }

            if (contactField != null)
            {
                contactField.Value = address.Nr;
            }

            nameField.Selected = false;
        }

        /// <summary>
        /// Setzt die Auswahl aller Namensfelder.
        /// </summary>
        /// <param name="selected">Ob die Felder ausgewählt sein sollen.</param>
        public void SetSelection(bool selected)
        {
            bool value = selected ? true : false;
            BuyerName.Selected = value;
            ProcessorName.Selected = value;
            DeliveryName.Selected = value;
        }

        /// <summary>
        /// Validates if all required fields in the address head are filled.
        /// If any required field is missing, an error message is displayed.
        /// </summary>
        /// <param name="language">The language code for error messages.</param>
        /// <returns>True if all required fields are filled, otherwise false.</returns>
        public bool IsAddressHeadValid(string language)
        {
            var missingFields = new List<string>();

            // Check each required field and add missing ones to the list
            if (string.IsNullOrEmpty(BuyerName.Value)) missingFields.Add(BuyerName.Titel);
            if (string.IsNullOrEmpty(BuyerStreet.Value)) missingFields.Add(BuyerStreet.Titel);
            if (string.IsNullOrEmpty(BuyerPlace.Value)) missingFields.Add(BuyerPlace.Titel);
            if (string.IsNullOrEmpty(BuyerPropertyHolder.Value)) missingFields.Add(BuyerPropertyHolder.Titel);
            if (string.IsNullOrEmpty(ProcessorName.Value)) missingFields.Add(ProcessorName.Titel);
            if (string.IsNullOrEmpty(ProcessorStreet.Value)) missingFields.Add(ProcessorStreet.Titel);
            if (string.IsNullOrEmpty(ProcessorPlace.Value)) missingFields.Add(ProcessorPlace.Titel);
            if (string.IsNullOrEmpty(ProcessorPropertyHolder.Value)) missingFields.Add(ProcessorPropertyHolder.Titel);
            if (string.IsNullOrEmpty(InvoiceName.Value)) missingFields.Add(InvoiceName.Titel);
            if (string.IsNullOrEmpty(InvoiceStreet.Value)) missingFields.Add(InvoiceStreet.Titel);
            if (string.IsNullOrEmpty(InvoicePlace.Value)) missingFields.Add(InvoicePlace.Titel);
            if (string.IsNullOrEmpty(Commission.Value)) missingFields.Add(Commission.Titel);
            if (string.IsNullOrEmpty(Date.Value)) missingFields.Add(Date.Titel);
            if (string.IsNullOrEmpty(Visum.Value)) missingFields.Add(Visum.Titel);
            if (string.IsNullOrEmpty(ObjectType.Value)) missingFields.Add(ObjectType.Titel);
            if (string.IsNullOrEmpty(WishDate.Value)) missingFields.Add(WishDate.Titel);
            if (string.IsNullOrEmpty(DeliveryName.Value)) missingFields.Add(DeliveryName.Titel);
            if (string.IsNullOrEmpty(DeliveryStreet.Value)) missingFields.Add(DeliveryStreet.Titel);
            if (string.IsNullOrEmpty(DeliveryPlace.Value)) missingFields.Add(DeliveryPlace.Titel);

            // If there are missing fields, display an error message
            if (missingFields.Any())
            {
                string errorMessage = string.Join(", ", missingFields);
                ErrorManager.HandleErrorMessage(Resource.AdressNotCompleteMessage, new Exception(errorMessage));
                return false;
            }
            return true;
        }

        /// <summary>
        /// Updates the language for all button fields.
        /// </summary>
        /// <param name="language">The new language to set for the buttons.</param>
        public void UpdateLanguage(string language)
        {
            // Iterate through all the button fields and update their language.
            foreach (var button in _adressHeadDictionary.Values)
            {
                button.UpdateLanguage(language);
            }
        }
    }
}
