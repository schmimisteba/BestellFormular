namespace BestellFormular.Models.Manager
{
    public class LanguageHandler
    {
        public static string GetLanguageFromCulture()
        {
            // Hole die aktuelle Kultur des Threads
            string culture = Thread.CurrentThread.CurrentCulture.Name.ToLower();

            // Wenn die Kultur mit "fr" beginnt, ändere sie auf "fr-CH"
            if (culture.StartsWith("fr"))
            {
                return "fr-CH";
            }
            // Wenn die Kultur mit "de" beginnt, ändere sie auf "de-CH"
            else if (culture.StartsWith("de"))
            {
                return "de-CH";
            }
            // Wenn die Kultur mit "it" beginnt, ändere sie auf "it-CH"
            else if (culture.StartsWith("it"))
            {
                return "it-CH";
            }
            // Falls keine der oben genannten Kulturen zutrifft, setze sie auf "de-CH"
            else
            {
                return "de-CH";
            }
        }
    }
}