namespace BestellFormular.Models.Helper
{
    /// <summary>
    /// Provides extension methods for string manipulation.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Extracts a substring between two specified strings.
        /// </summary>
        /// <param name="str">The source string.</param>
        /// <param name="start">The starting delimiter.</param>
        /// <param name="end">The ending delimiter.</param>
        /// <returns>
        /// The substring between the start and end delimiters, or the original string
        /// if the delimiters are not found in the correct order.
        /// </returns>
        public static string GetBetween(this string str, string start, string end)
        {
            // Return an empty string if the source string is null
            if (str is null)
            {
                return string.Empty;
            }

            // Find the start and end indices of the delimiters
            int startIndex = str.IndexOf(start, StringComparison.Ordinal);
            int endIndex = str.IndexOf(end, startIndex + start.Length, StringComparison.Ordinal);

            // Ensure both delimiters exist and are in the correct order
            if (startIndex >= 0 && endIndex > startIndex)
            {
                return str.Substring(startIndex + start.Length, endIndex - startIndex - start.Length);
            }

            // Return the original string if no valid match is found
            return str;
        }
    }
}
