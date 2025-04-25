using BestellFormular.Models.AddressHead;
using CommunityToolkit.Maui.Storage;

namespace BestellFormular.Models.Helper
{
    /// <summary>
    /// Utility class for handling file-related operations such as saving and picking files.
    /// </summary>
    public static class FileHelper
    {
        /// <summary>
        /// Saves an empty file with a name derived from the provided address head.
        /// </summary>
        /// <param name="adressHead">The address head containing file metadata.</param>
        /// <returns>A Task that returns a FileSaverResult containing file save details.</returns>
        public static async Task<FileSaverResult> GetFilePathFromFileSaver(AdressHead adressHead)
        {
            var defaultFileName = GetFileNameFromAdressHead(adressHead);
            return await FileSaver.SaveAsync(defaultFileName, new MemoryStream());
        }

        /// <summary>
        /// Opens a file picker dialog for the user to select a file.
        /// </summary>
        /// <returns>A Task that returns a FileResult containing file selection details.</returns>
        public static async Task<FileResult?> GetFilePathFromFilePicker()
        {
            return await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Datei auswählen"
            });
        }

        /// <summary>
        /// Generates a file name based on the properties of the provided address head.
        /// </summary>
        /// <param name="adressHead">The address head containing file metadata.</param>
        /// <returns>A formatted file name string.</returns>
        public static string GetFileNameFromAdressHead(AdressHead adressHead)
        {
            // Ensure the date part is not null or empty
            string datePart = !string.IsNullOrEmpty(adressHead.Date?.Value)
                ? adressHead.Date.Value
                : "no_date";

            return $"{adressHead.Visum?.Value}_{adressHead.Commission?.Value}_{adressHead.BuyerName?.Value}_{datePart}.xlsx";
        }
    }
}