using System;
using System.Globalization;
using System.Resources;

namespace BestellFormular.Resources.Language
{
    public static class AppResources
    {
        private static ResourceManager _resourceManager;

        static AppResources()
        {
            try
            {
                _resourceManager = new ResourceManager("BestellFormular.Resources.Language.AppResources",
                                                     typeof(AppResources).Assembly);
            }
            catch (Exception ex)
            {
                // Hier sollte ein Logger verwendet werden
                Console.WriteLine($"Fehler beim Initialisieren des ResourceManager: {ex.Message}");
            }
        }

        public static string GetString(string key, string languageCode)
        {
            if (_resourceManager == null)
                return key;

            CultureInfo culture;
            switch (languageCode)
            {
                case "DE":
                    culture = new CultureInfo("de-DE");
                    break;
                case "FR":
                    culture = new CultureInfo("fr-FR");
                    break;
                case "IT":
                    culture = new CultureInfo("it-IT");
                    break;
                default:
                    culture = new CultureInfo("de-DE");
                    break;
            }

            try
            {
                return _resourceManager.GetString(key, culture) ?? key;
            }
            catch (Exception ex)
            {
                // Hier sollte ein Logger verwendet werden
                Console.WriteLine($"Fehler beim Abrufen der Ressource '{key}': {ex.Message}");
                return key; // Fallback zum Schlüsselnamen
            }
        }
    }
}