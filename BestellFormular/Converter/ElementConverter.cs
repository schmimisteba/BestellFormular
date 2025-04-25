using BestellFormular.Models.AddressHead;
using System.Collections.ObjectModel;
using System.Globalization;

namespace BestellFormular.Converter
{
    // Converts a string into a formatted text where the first character has a larger font size
    public class FormattedTextConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null) return null;

            string text = value?.ToString()?.Trim();
            if (string.IsNullOrEmpty(text)) return null;

            var formattedString = new FormattedString();

            // Adds a leading space
            formattedString.Spans.Add(new Span
            {
                Text = " ",
                FontSize = 16
            });

            // Adds the first character with larger font size
            formattedString.Spans.Add(new Span
            {
                Text = text[0].ToString(),
                FontSize = 16
            });

            // Adds the rest of the text with a smaller font size
            if (text.Length > 1)
            {
                formattedString.Spans.Add(new Span
                {
                    Text = text.Substring(1),
                    FontSize = 10
                });
            }

            return formattedString;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return null; // One-way binding only
        }
    }

    public class FirstCharConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            string text = value.ToString();
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            // Return only the first character
            return text[0].ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null; // One-way binding only
        }
    }

    public class RestOfStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            string text = value.ToString();
            if (string.IsNullOrEmpty(text) || text.Length <= 1)
                return string.Empty;

            // Return everything after the first character
            return text.Substring(1);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null; // One-way binding only
        }
    }

    // Adds an asterisk (*) to indicate a required field
    public class RequiredFieldConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null) return null;

            string text = value.ToString().Trim();
            if (string.IsNullOrEmpty(text)) return null;

            var formattedString = new FormattedString();

            formattedString.Spans.Add(new Span
            {
                Text = text,
                FontSize = 16
            });

            formattedString.Spans.Add(new Span
            {
                Text = " *",
                FontSize = 16,
                FontAttributes = FontAttributes.Bold
            });

            return formattedString;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException(); // One-way binding only
        }
    }

    // Checks if a value is not null and returns true, otherwise false
    public class NullToBooleanConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Returns false if the value is an empty string, otherwise true
    public class IsEmptyToFalseConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return !string.IsNullOrEmpty(value?.ToString());
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Returns false if the value is an empty ObservableCollection, otherwise true
    public class IsEmptyObservableCollectionToFalseConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var addresse = value as ObservableCollection<Address>;
            return addresse != null && addresse.Count > 0;
        }
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BeforeSlashNConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                var lines = str.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                for (int i = 0; i < lines.Length; i++)
                {
                    int index = lines[i].IndexOf(" Pos");
                    if (index != -1)
                    {
                        lines[i] = lines[i].Substring(0, index).Trim();
                    }
                }
                return string.Join("\n", lines);
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class AfterSlashNConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                var lines = str.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                for (int i = 0; i < lines.Length; i++)
                {
                    int index = lines[i].IndexOf(" Pos");
                    if (index != -1)
                    {
                        lines[i] = lines[i].Substring(index).Trim();
                    }
                    else
                    {
                        lines[i] = string.Empty;
                    }
                }
                return string.Join("\n", lines);
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DisplayWithoutBracketsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string text)
            {
                // Entfernt den Text in eckigen Klammern und alle Leerzeichen am Ende
                return System.Text.RegularExpressions.Regex.Replace(text, @"\s*\[[^\]]*\]$", "").Trim();
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Bei ConvertBack wird der Originalwert benötigt, was hier nicht möglich ist
            // da die Information über den Code verloren gegangen ist
            return value;
        }
    }
}