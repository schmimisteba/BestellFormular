using CommunityToolkit.Mvvm.ComponentModel;

namespace BestellFormular.Models.AddressHead
{
    /// <summary>
    /// Represents an address with observable properties.
    /// </summary>
    public partial class Address : ObservableObject
    {
        /// <summary>
        /// Gets or sets the address number.
        /// </summary>
        [ObservableProperty]
        private string nr;

        /// <summary>
        /// Gets or sets the debtor information.
        /// </summary>
        [ObservableProperty]
        private string debitor;

        /// <summary>
        /// Gets or sets the name of the recipient.
        /// </summary>
        [ObservableProperty]
        private string name;

        /// <summary>
        /// Gets or sets the street name.
        /// </summary>
        [ObservableProperty]
        private string street;

        /// <summary>
        /// Gets or sets the postal code (ZIP).
        /// </summary>
        [ObservableProperty]
        private string zip;

        /// <summary>
        /// Gets or sets the city name.
        /// </summary>
        [ObservableProperty]
        private string city;

        /// <summary>
        /// Gets or sets the overview string for the address.
        /// </summary>
        [ObservableProperty]
        private string overviewString;
    }
}
