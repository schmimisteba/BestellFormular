using BestellFormular.Models.AddressHead;
using BestellFormular.Models.Manager;
using BestellFormular.Models.Window;
using BestellFormular.Resources.Language;
using ClosedXML.Excel;
using CommunityToolkit.Maui.Core.Extensions;
using System.Linq.Expressions;

namespace BestellFormular.Models.Helper
{
    /// <summary>
    /// Class responsible for loading Excel data into dictionaries for address and field information.
    /// </summary>
    public class ExcelLoader
    {
        private readonly Dictionary<string, Field> _fieldDictionary = new();
        private readonly Dictionary<string, Address> _addressDictionary = new();

        public ExcelLoader(string idExcelPath, string adressesExcelPath)
        {
            LoadFieldData(idExcelPath);
            LoadAddressesData(adressesExcelPath);
        }

        /// <summary>
        /// Loads address data from an Excel file.
        /// </summary>
        private void LoadAddressesData(string path)
        {

            if (!File.Exists(path))
                ErrorManager.HandleErrorMessage(Resource.FileNotExist, new FileNotFoundException($"{path}. "));
            return;

            try
            {
                string? basePath = Path.GetDirectoryName(path);
                using var workbook = new XLWorkbook(path);
                var worksheet = workbook.Worksheet(1) ?? throw new Exception("No worksheet found in the Excel file.");

                foreach (var row in worksheet.RowsUsed().Skip(1))
                {
                    Address address = CreateAddressFromRow(row, basePath);
                    _addressDictionary[address.OverviewString] = address;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error loading Excel data", ex);
            }
        }

        private static Address CreateAddressFromRow(IXLRow row, string? basePath)
        {
            return new Address
            {
                Nr = row.Cell(1).GetString(),
                Debitor = row.Cell(2).GetString(),
                Name = row.Cell(3).GetString(),
                Street = row.Cell(4).GetString(),
                Zip = row.Cell(5).GetString(),
                City = row.Cell(6).GetString(),
                OverviewString = $"{row.Cell(3).GetString()}, {row.Cell(6).GetString()}"
            };
        }

        public List<Address> GetAllAddressesStartWithString(string prefix)
        {
            return _addressDictionary.Values.Where(a => a.Name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public Address? GetAddressByName(string name)
        {
            return _addressDictionary.TryGetValue(name, out var address) ? address : null;
        }

        /// <summary>
        /// Loads field data from an Excel file.
        /// </summary>
        private void LoadFieldData(string path)
        {

            if (!File.Exists(path))
            {
                ErrorManager.HandleErrorMessage(Resource.FileNotExist, new FileNotFoundException($"{path}. "));
                return;
            }


            try
            {
                string? basePath = Path.GetDirectoryName(path);
                using var workbook = new XLWorkbook(path);
                var worksheet = workbook.Worksheet(1) ?? throw new Exception("No worksheet found in the Excel file.");

                foreach (var row in worksheet.RowsUsed().Skip(1))
                {
                    var field = CreateFieldFromRow(row, basePath);
                    _fieldDictionary[field.Id] = field;
                }
            }
            catch (Exception ex)
            {
                //ErrorManager.HandleErrorMessage("LoadErrorMessage", ex: new Exception("Error loading Excel data", ex));
            }
        }

        private static Field CreateFieldFromRow(IXLRow row, string? basePath)
        {
            Field field = new Field(row.Cell(1).GetString())
            {
                Product = row.Cell(2).GetString(),
                Titel = row.Cell(3).GetString().Split('|')[0],
                Value = row.Cell(4).GetString(),
                OldShortCut = row.Cell(5).GetString().Split('|')[0],
                Image = Path.Join(basePath, row.Cell(6).GetString().Split('|')[0]),
                AccessoriesGroup = row.Cell(7).GetString(),
                GroupName = row.Cell(8).GetString().Split('|')[0],
                FieldInput = row.Cell(9).GetString(),
                FieldInputSelection = row.Cell(9).GetString().Split('|')[0].Split('/').Select(s => s.Trim()).ToObservableCollection(),
                FieldInputSelectionTranslations = CreateFieldInputDictionary(row.Cell(9).GetString().Split('|')),
                ShortCuts = CreateBoolFieldDictionary(row.Cell(5).GetString().Split('|')),
                Images = CreateImageFieldDictionary(row.Cell(6).GetString().Split('|'), basePath),
                TitleTranslations = CreateFieldDictionary(row.Cell(3).GetString().Split('|')),
                GroupNameTranslations = CreateFieldDictionary(row.Cell(8).GetString().Split('|')),
                Selected = !string.Equals(row.Cell(19).GetString()?.Trim(), "false", StringComparison.OrdinalIgnoreCase),
                Enabled = !string.Equals(row.Cell(20).GetString()?.Trim(), "false", StringComparison.OrdinalIgnoreCase)
            };

            return field;
        }

        private static Dictionary<bool, string> CreateImageFieldDictionary(string[] values, string basePath)
        {
            return new Dictionary<bool, string>
            {
                { false, Path.Join(basePath, values.ElementAtOrDefault(0)) ?? string.Empty },
                { true, Path.Join(basePath, values.ElementAtOrDefault(1)) ?? string.Empty }
            };
        }

        private static Dictionary<bool, string> CreateBoolFieldDictionary(string[] values)
        {
            return new Dictionary<bool, string>
            {
                { false, values.ElementAtOrDefault(0) ?? string.Empty },
                { true, values.ElementAtOrDefault(1) ?? string.Empty }
            };
        }

        private static Dictionary<string, string> CreateFieldDictionary(string[] values)
        {
            return new Dictionary<string, string>
            {
                { "de-CH", values.ElementAtOrDefault(0) ?? string.Empty },
                { "fr-CH", values.ElementAtOrDefault(1) ?? string.Empty },
                { "it-CH", values.ElementAtOrDefault(2) ?? string.Empty }
            };
        }

        private static Dictionary<string, List<string>> CreateFieldInputDictionary(string[] values)
        {
            return new Dictionary<string, List<string>>
            {
                { "de-CH", values.ElementAtOrDefault(0)?.Split('/').Select(s => s.Trim()).ToList() ?? new List<string>() },
                { "fr-CH", values.ElementAtOrDefault(1)?.Split('/').Select(s => s.Trim()).ToList() ?? new List<string>() },
                { "it-CH", values.ElementAtOrDefault(2)?.Split('/').Select(s => s.Trim()).ToList() ?? new List<string>() }
            };
        }

        public Field? GetFieldById(string id)
        {
            var f = _fieldDictionary.TryGetValue(id, out var field) ? field.Copy() : null;
            if (f != null)
            {
                return f;
            }

            ErrorManager.HandleErrorMessage(Resource.IdNotFound, new FileNotFoundException($"id: {id}. "));
            return new Field("id");
        }
    }
}