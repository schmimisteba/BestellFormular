using BestellFormular.Models.AddressHead;
using BestellFormular.Models.Window;
using BestellFormular.Models.Window.Product;
using ClosedXML.Excel;

namespace BestellFormular.Models.Helper
{
    public static class ExcelHelper
    {
        private const string DefaultFont = "Arial";
        private const int DefaultFontSize = 10;
        private const double DefaultMargin = 0.15;
        private const double DefaultRowHeight = 17.5;

        public static void ExportTocExcel(List<WindowElement> elements, AdressHead adresshead, string filePath)
        {
            using var workbook = new XLWorkbook();

            ApplyGlobalWorkbookStyle(workbook);

            CreateAdressHeadSheet(workbook, adresshead);
            CreateProductSheets(workbook, elements, adresshead);
            CreateMemorySheet(workbook, elements, adresshead);

            // Schütze alle Arbeitsblätter
            foreach (var sheet in workbook.Worksheets)
            {
                sheet.Protect();
            }

            workbook.SaveAs(filePath);
        }

        private static void ApplyGlobalWorkbookStyle(XLWorkbook workbook)
        {
            workbook.Style.Font.FontName = DefaultFont;
            workbook.Style.Font.FontSize = DefaultFontSize;
        }

        private static void ConfigureWorksheet(IXLWorksheet worksheet, bool isLandscape = false)
        {
            worksheet.PageSetup.PageOrientation = isLandscape ? XLPageOrientation.Landscape : XLPageOrientation.Portrait;
            worksheet.PageSetup.PaperSize = XLPaperSize.A4Paper;

            // Setze alle Ränder einheitlich
            var margins = worksheet.PageSetup.Margins;
            margins.SetLeft(DefaultMargin);
            margins.SetRight(DefaultMargin);
            margins.SetTop(DefaultMargin);
            margins.SetBottom(DefaultMargin);
            margins.SetHeader(DefaultMargin);
            margins.SetFooter(DefaultMargin);
        }

        private static void CreateAdressHeadSheet(XLWorkbook workbook, AdressHead adresshead)
        {
            var worksheet = workbook.Worksheets.Add("Adresskopf");
            ConfigureWorksheet(worksheet, true);

            // Struktur für die drei Spaltengruppen
            var sections = new Dictionary<string, (int column, int row, string groupName)>
            {
                { "Buyer", (1, 4, "") },
                { "Processor", (3, 4, "") },
                { "Deliver", (5, 4, "") }
            };

            // Übergruppen Felder setzen (Buyer, Processor, Deliver)
            foreach (var property in adresshead.GetType().GetProperties())
            {
                if (property.Name.Contains("LikeBuyer")) continue;

                if (property.GetValue(adresshead) is Field field)
                {
                    foreach (var prefix in sections.Keys.ToList())
                    {
                        if (property.Name.StartsWith(prefix) && !property.Name.EndsWith("Nr"))
                        {
                            var (column, row, _) = sections[prefix];

                            // Header bei der ersten Eigenschaft dieses Typs setzen
                            if (row == 4)
                            {
                                worksheet.Range(3, column, 3, column + 1).Merge();
                                worksheet.Cell(3, column).SetValue(field.GroupName).Style.Font.Bold = true;
                                worksheet.Cell(3, column).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                                sections[prefix] = (column, row, field.GroupName);
                            }

                            // Felddaten hinzufügen
                            worksheet.Cell(row, column).SetValue(field.Titel).Style.Font.Bold = true;
                            worksheet.Cell(row, column + 1).SetValue(field.Value);

                            // Zeilenposition aktualisieren
                            sections[prefix] = (column, row + 1, sections[prefix].groupName);
                            break;
                        }
                    }
                }
            }

            // Maximale Zeilenhöhe bestimmen
            int startRow = sections.Values.Max(v => v.row) + 1;

            // Definition der zusätzlichen Abschnitte
            var additionalSections = new[]
            {
                (field: adresshead.Commission, column: 1, title: adresshead.Commission.GroupName),
                (field: adresshead.WishDate, column: 3, title: adresshead.WishDate.GroupName),
                (field: adresshead.InvoiceName, column: 5, title: adresshead.InvoiceName.GroupName)
            };

            // Drei neue Abschnitte für zusätzliche Felder einrichten
            foreach (var section in additionalSections)
            {
                worksheet.Range(startRow, section.column, startRow, section.column + 1).Merge();
                worksheet.Cell(startRow, section.column).SetValue(section.title).Style.Font.Bold = true;
                worksheet.Cell(startRow, section.column).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            }

            // Zeilenpositionen für die drei Spalten initialisieren
            int[] sectionRows = { startRow + 1, startRow + 1, startRow + 1 };

            // Felder in drei Spalten hinzufügen
            // Auftragsdaten (Spalte 1-2)
            var section1Fields = new[]
            {
                (title: adresshead.Commission.Titel, field: adresshead.Commission),
                (title: adresshead.OrderNumber.Titel, field: adresshead.OrderNumber),
                (title: adresshead.OfferNumber.Titel, field: adresshead.OfferNumber),
                (title: adresshead.Commission.Titel, field: adresshead.Commission),
                (title: adresshead.Date.Titel, field: adresshead.Date),
                (title: adresshead.Visum.Titel, field: adresshead.Visum),
                (title: adresshead.ObjectType.Titel, field: adresshead.ObjectType)
            };

            foreach (var (title, field) in section1Fields)
            {
                AddExtraField(worksheet, title, field, sectionRows[0]++, 1, 2);
            }


            // Spezieller Fall für Remark: Value unter dem Titel
            AddExtraFieldWithValueBelow(worksheet, adresshead.Remark.Titel, adresshead.Remark, ++sectionRows[0], 1, 2);
            sectionRows[0] += 2; // Zwei Zeilen für Titel und Value

            // Lieferdetails (Spalte 3-4)
            var section2Fields = new[]
            {
                (title: adresshead.WishDate.Titel, field: adresshead.WishDate),
                (title: adresshead.NameAvis.Titel, field: adresshead.NameAvis),
                (title: adresshead.PhoneAvis.Titel, field: adresshead.PhoneAvis),
                (title: adresshead.OnCall.Titel, field: adresshead.OnCall),
                (title: adresshead.WillBeCollected.Titel, field: adresshead.WillBeCollected),
                (title: adresshead.WithoutTrailer.Titel, field: adresshead.WithoutTrailer)
            };

            foreach (var (title, field) in section2Fields)
            {
                AddExtraField(worksheet, title, field, sectionRows[1]++, 3, 4);
            }

            // Rechnungsdetails (Spalte 5-6)
            var section3Fields = new[]
            {
                (title: adresshead.InvoiceName.Titel, field: adresshead.InvoiceName),
                (title: adresshead.InvoiceStreet.Titel, field: adresshead.InvoiceStreet),
                (title: adresshead.InvoicePlace.Titel, field: adresshead.InvoicePlace),
                (title: adresshead.InvoiceCoordinates.Titel, field: adresshead.InvoiceCoordinates)
            };

            foreach (var (title, field) in section3Fields)
            {
                AddExtraField(worksheet, title, field, sectionRows[2]++, 5, 6);
            }

            // Zeilenhöhen für alle Zeilen festlegen
            int maxRow = sectionRows.Max();
            for (int row = 3; row <= maxRow; row++)
            {
                worksheet.Row(row).Height = 17.5;
            }

            // Styling auf alle Bereiche anwenden
            ApplyRangeStyling(worksheet);

            // Kopfzeilen mit Fett und Rahmen stylen
            StyleHeaderRows(worksheet, startRow);

            // Fußzeile hinzufügen
            AddFooter(worksheet, adresshead);

            // Spalten zuerst basierend auf Inhalt anpassen
            worksheet.Columns().AdjustToContents();

            // Jetzt für A4-Seite optimieren
            OptimizeColumnsForA4(worksheet);

            AddAddressHeader(worksheet, adresshead, "Adresskopf");
        }

        private static void AddExtraField(IXLWorksheet worksheet, string title, Field field, int row, int titleCol, int valueCol)
        {
            if (field != null)
            {
                worksheet.Cell(row, titleCol).SetValue(title).Style.Font.Bold = true;
                worksheet.Cell(row, valueCol).SetValue(field.Value);
            }
        }

        private static void AddExtraFieldWithValueBelow(IXLWorksheet worksheet, string title, Field field, int row, int titleCol, int valueCol)
        {
            if (field != null)
            {
                // Titel in der ersten Zeile
                worksheet.Cell(row, titleCol).SetValue(title).Style.Font.Bold = true;

                // Value in der nächsten Zeile unter dem Titel
                worksheet.Range(row + 1, titleCol, row + 1, valueCol).Merge();
                worksheet.Cell(row + 1, titleCol).SetValue(field.Value);
            }
        }

        /// <summary>
        /// Wendet einheitliches Styling auf alle Datenbereiche des Worksheets an
        /// </summary>
        /// <param name="worksheet">Das zu formatierende Arbeitsblatt</param>
        private static void ApplyRangeStyling(IXLWorksheet worksheet)
        {
            // Gemeinsame Styling-Funktionen
            Action<IXLRange> applyCommonStyling = range =>
            {
                range.Style.Border.InsideBorderColor = XLColor.Black;
                range.Style.Border.InsideBorder = XLBorderStyleValues.Hair;
                range.Style.Border.OutsideBorder = XLBorderStyleValues.None;
                range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            };

            // Formatierung der Hauptadressbereiche
            applyCommonStyling(worksheet.Range("A3:F9"));

            // Formatierung der zusätzlichen Informationsbereiche
            applyCommonStyling(worksheet.Range("A11:F18"));

            // Formatierung des Fußbereichs für Bemerkungen
            var footerRange = worksheet.Range("A21:F21");
            footerRange.Merge();
            footerRange.Style.Alignment.WrapText = true;
            footerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            footerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            footerRange.Style.Border.OutsideBorderColor = XLColor.Black;
            footerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Hair;

            // Zeilenhöhe für den Fußbereich anpassen
            worksheet.Row(21).Height = 80;
        }

        // New method to style header rows
        private static void StyleHeaderRows(IXLWorksheet worksheet, int startRow)
        {
            // Style first header row
            var firstHeaderRange = worksheet.Range(3, 1, 3, 6);
            firstHeaderRange.Style.Font.Bold = true;
            firstHeaderRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;

            // Style second header row
            var secondHeaderRange = worksheet.Range(startRow, 1, startRow, 6);
            secondHeaderRange.Style.Font.Bold = true;
            secondHeaderRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
        }

        // New method to optimize columns for A4 width
        private static void OptimizeColumnsForA4(IXLWorksheet worksheet)
        {
            // A4 width in points
            const double a4WidthPoints = 135;

            // Tabelle in der Mitte der Seite platzieren
            worksheet.PageSetup.CenterHorizontally = true;
            worksheet.PageSetup.PagesWide = 1; // Auf eine Seitenbreite anpassen

            // Calculate current total width
            double currentWidth = 0;
            for (int col = 1; col <= 6; col++)
            {
                currentWidth += worksheet.Column(col).Width;
            }

            // Calculate scaling factor to fit A4
            double scaleFactor = (a4WidthPoints) / currentWidth;

            // Get title columns and value columns
            var titleColumns = new[] { 1, 3, 5 };
            var valueColumns = new[] { 2, 4, 6 };

            // Preserve title column widths based on content (with minimum width)
            foreach (int col in titleColumns)
            {
                double contentWidth = worksheet.Column(col).Width;
                double minWidth = 10; // Minimum width for title columns
                worksheet.Column(col).Width = Math.Max(contentWidth, minWidth);
            }

            // Recalculate remaining space for value columns
            double titleColumnsWidth = titleColumns.Sum(col => worksheet.Column(col).Width);
            double remainingWidth = (a4WidthPoints) - titleColumnsWidth;
            double valueColumnWidth = remainingWidth / valueColumns.Length;

            // Set equal width for all value columns
            foreach (int col in valueColumns)
            {
                worksheet.Column(col).Width = valueColumnWidth;
            }

            // Add a little extra space to the rightmost column to ensure full width is used
            worksheet.Column(6).Width += 2;
        }

        private static void ApplyCellStyle(IXLCell cell, XLAlignmentHorizontalValues horizontalAlignment = XLAlignmentHorizontalValues.Center, bool isHeader = false)
        {
            var style = cell.Style;
            cell.WorksheetRow().Height = 17.5;
            style.Alignment.Horizontal = horizontalAlignment;
            style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            if (isHeader)
            {
                style.Font.Bold = true;
                style.Border.BottomBorder = XLBorderStyleValues.Thin;
            }
        }

        private static void CreateProductSheets(XLWorkbook workbook, List<WindowElement> elements, AdressHead adressHead)
        {
            var aluminumWindowSills = elements.Where(e => e.AluminumWindowSill?.Selected == true)
                                              .Select(e =>
                                              {
                                                  e.AluminumWindowSill.Count.Value = e.Count.ToString();
                                                  return e.AluminumWindowSill;
                                              })
                                              .ToList();
            aluminumWindowSills = ProductBase.FilterElements(aluminumWindowSills);

            var supportAngles = elements.Where(e => e.SupportAngleWithSlope?.Selected == true)
                                        .Select(e =>
                                        {
                                            e.SupportAngleWithSlope.Count.Value = e.Count.ToString();
                                            return e.SupportAngleWithSlope;
                                        })
                                        .ToList();

            supportAngles = ProductBase.FilterElements(supportAngles);
            SupportAngleWithSlope.CountToElement(supportAngles);


            var supportWedgesWithSlope = elements.Where(e => e.SupportWedgesWithSlope?.Selected == true)
                                                 .Select(e =>
                                                 {
                                                     e.SupportWedgesWithSlope.Count.Value = e.Count.ToString();
                                                     return e.SupportWedgesWithSlope;
                                                 })
                                                 .ToList();
            supportWedgesWithSlope = ProductBase.FilterElements(supportWedgesWithSlope);
            SupportWedgesWithSlope.CountToElement(supportWedgesWithSlope);

            var apronElements = elements.Where(e => e.ApronElement?.Selected == true)
                           .Select(e =>
                           {
                               e.ApronElement.Count.Value = e.Count.ToString();
                               return e.ApronElement;
                           })
                           .ToList();

            apronElements = ProductBase.FilterElements(apronElements);

            var ravisols = elements.Where(e => e.Ravisol?.Selected == true)
                                   .Select(e =>
                                   {
                                       e.Ravisol.Count.Value = e.Count.ToString();
                                       return e.Ravisol;
                                   })
                                   .ToList();
            ravisols = ProductBase.FilterElements(ravisols);
            Ravisol.CountToElement(ravisols);

            var stuisol = elements.Where(e => e.Stuisol?.Selected == true)
                                  .Select(e =>
                                  {
                                      e.Stuisol.Count.Value = e.Count.ToString();
                                      return e.Stuisol;
                                  })
                                  .ToList();
            stuisol = ProductBase.FilterElements(stuisol);
            Stuisol.CountToElement(stuisol);

            if (aluminumWindowSills.Any())
                CreateProductSheet(workbook, "Fensterbänke aus Alu", aluminumWindowSills, adressHead);

            if (supportAngles.Any())
                CreateProductSheet(workbook, "Auflagewinkel mit Gefälle", supportAngles, adressHead);

            if (supportWedgesWithSlope.Any())
                CreateProductSheet(workbook, "Auflagekeile mit Gefälle", supportWedgesWithSlope, adressHead);

            if (apronElements.Any())
                CreateProductSheet(workbook, "Schürzenelemente Do-Tab", apronElements, adressHead);

            if (ravisols.Any())
                CreateProductSheet(workbook, "Rav-Isol", ravisols, adressHead);

            if (stuisol.Any())
                CreateProductSheet(workbook, "Stu-Isol", stuisol, adressHead);
        }

        private static void CreateProductSheet<T>(XLWorkbook workbook, string sheetName, List<T> products, AdressHead adressHead) where T : ProductBase
        {
            // Arbeitsblatt erstellen und konfigurieren
            var worksheet = workbook.Worksheets.Add(sheetName);
            ConfigureWorksheet(worksheet, isLandscape: true);

            // Felder ermitteln und sortieren (Pos und Count zuerst)
            var fields = typeof(T)
                .GetProperties()
                .Where(p => p.PropertyType == typeof(Field))
                .OrderBy(p => p.Name == "Pos" ? 0 : p.Name == "Count" ? 1 : 2)
                .ToList();

            // Spalten mit Daten ermitteln
            var ignoredIds = new HashSet<string> { "ID0223", "ID0234", "ID0449", "ID0463" };

            var columnsWithData = fields
                .Select((field, index) => new { field, index })
                .Where(x => products.Any(product =>
                {
                    var field = x.field.GetValue(product) as Field;
                    if (field == null || ignoredIds.Contains(field.Id)) return false;

                    return (!string.IsNullOrWhiteSpace(field.Value?.ToString()) && field.Value.ToString() != "Nein")
                           || field.Titel == "Bemerkungen";
                }))
                .Select(x => x.index)
                .ToList();

            // Datenbereich formatieren
            var dataRange = worksheet.Range(3, 1, 3 + products.Count, columnsWithData.Count);
            dataRange.Style.Border.InsideBorderColor = XLColor.Black;
            dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Hair;
            dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.None;
            dataRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            dataRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            // Konfiguration für Spaltenbreiten
            double defaultColumnWidth = 8; // Standardbreite für normale Spalten
            double totalWidth = 135;        // Gesamtbreite der Tabelle
            int remarkCol = 0;              // Index der Bemerkungsspalte

            // Überschriften setzen
            for (int i = 0; i < columnsWithData.Count; i++)
            {
                int col = columnsWithData[i];
                var headerField = fields[col].GetValue(products.First()) as Field;
                var headerTitle = !string.IsNullOrEmpty(headerField?.OldShortCut)
                    ? headerField.OldShortCut
                    : headerField?.Titel ?? "";

                var headerCell = worksheet.Cell(3, i + 1);
                headerCell.Value = headerTitle;

                if (headerTitle == "Bemerkungen")
                {
                    remarkCol = i + 1;
                }

                headerCell.Style.Font.Bold = true;
                headerCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                headerCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                headerCell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            }

            // Datenzellen setzen
            for (int row = 0; row < products.Count; row++)
            {
                worksheet.Row(row + 4).Height = DefaultRowHeight;

                for (int i = 0; i < columnsWithData.Count; i++)
                {
                    int col = columnsWithData[i];
                    var field = fields[col].GetValue(products[row]) as Field;
                    var cell = worksheet.Cell(row + 4, i + 1);

                    var outValue = StringExtensions.GetBetween(field?.Value, "[", "]") ?? field?.Value;
                    cell.Value = outValue ?? "";
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                }
            }

            // Fußzeile hinzufügen
            AddFooter(worksheet, adressHead);

            // Spaltenbreiten anpassen
            if (remarkCol > 0)
            {
                // Alle Spalten prüfen und Breite anpassen
                for (int i = 1; i <= columnsWithData.Count; i++)
                {
                    if (i != remarkCol)
                    {
                        worksheet.Column(i).AdjustToContents();

                        // Nur Standardbreite setzen, wenn Inhalt nicht größer ist
                        double contentWidth = worksheet.Column(i).Width;
                        if (contentWidth <= defaultColumnWidth)
                        {
                            worksheet.Column(i).Width = defaultColumnWidth;
                        }
                    }
                }

                // Berechne verbleibenden Platz für Bemerkungsspalte
                double usedWidth = (columnsWithData.Count - 1) * defaultColumnWidth;
                double remainingWidth = totalWidth - usedWidth;

                // Minimale Breite für Bemerkungsspalte sicherstellen
                worksheet.Column(remarkCol).Width = Math.Max(remainingWidth, defaultColumnWidth);
                worksheet.Column(remarkCol).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            }
            else
            {
                // Falls keine Bemerkungsspalte vorhanden, alle Spalten anpassen
                worksheet.Columns().AdjustToContents();
            }

            // Seiteneinstellungen
            worksheet.PageSetup.CenterHorizontally = true;
            worksheet.PageSetup.PagesWide = 1; // Auf eine Seitenbreite anpassen

            // Adressheader hinzufügen
            AddAddressHeader(worksheet, adressHead, sheetName);
        }

        private static void AddFooter(IXLWorksheet worksheet, AdressHead adressHead)
        {
            var footerLeftText = worksheet.PageSetup.Footer.Left.AddText(BestellFormularController.Instance.GetVersion());
            footerLeftText.FontName = "Arial";
            footerLeftText.FontSize = 9;

            string formattedDate = "";
            if (!string.IsNullOrEmpty(adressHead.Date?.Value) && DateTime.TryParse(adressHead.Date.Value, out var parsedDate))
            {
                formattedDate = parsedDate.ToString("dd.MM.yyyy"); // German-style date format
            }
            else
            {
                formattedDate = "Invalid date"; // Fallback if date is missing or not valid
            }

            var footerRightText = worksheet.PageSetup.Footer.Right.AddText($"{adressHead.Visum.Value} {formattedDate} - Seite &P von &N");
            footerRightText.FontName = "Arial";
            footerRightText.FontSize = 9;
        }


        private static void AddAddressHeader(IXLWorksheet worksheet, AdressHead adressHead, string sheetName)
        {
            // TODO
            var headerRightText = worksheet.PageSetup.Header.Right.AddText("Dosteba");
            headerRightText.FontName = "Univers 45 Light Bold";
            headerRightText.FontSize = 36;
            headerRightText.FontColor = XLColor.FromHtml("#4E95D5");

            worksheet.Cell(1, 1).SetValue(sheetName + " " + adressHead.BuyerName.Value + " " + adressHead.Commission.Value);
            worksheet.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            worksheet.Cell(1, 1).Style.Font.FontName = "Univers 45 Light Bold";
            worksheet.Cell(1, 1).Style.Font.FontSize = 12;

            worksheet.PageSetup.Margins.SetTop(0.50);
            worksheet.PageSetup.Margins.SetHeader(0.14);
        }

        private static void CreateMemorySheet(XLWorkbook workbook, List<WindowElement> elements, AdressHead adresshead)
        {
            var worksheet = workbook.Worksheets.Add("Memory");
            worksheet.Visibility = XLWorksheetVisibility.Hidden;
            var memoryRecords = new List<Dictionary<string, string>>();

            // Adresskopf-Daten
            var adressRecord = new Dictionary<string, string>
            {
                ["RecordType"] = "AdressHead",
                ["WindowGroupId"] = "0"
            };
            foreach (var prop in adresshead.GetType().GetProperties())
            {
                if (prop.GetValue(adresshead) is Field field)
                {
                    adressRecord[field.Id] = field.Value;
                }
            }
            memoryRecords.Add(adressRecord);

            // Fenster-Gruppen
            var groupId = 1;
            foreach (var window in elements)
            {
                if (window.AluminumWindowSill?.Selected == true)
                    AddProductToMemory("AluminumWindowSill", window.AluminumWindowSill, groupId, window.Count, window.Name, memoryRecords);

                if (window.SupportAngleWithSlope?.Selected == true)
                    AddProductToMemory("SupportAngleWithSlope", window.SupportAngleWithSlope, groupId, window.Count, window.Name, memoryRecords);

                if (window.SupportWedgesWithSlope?.Selected == true)
                    AddProductToMemory("SupportWedgesWithSlope", window.SupportWedgesWithSlope, groupId, window.Count, window.Name, memoryRecords);

                if (window.ApronElement?.Selected == true)
                    AddProductToMemory("ApronElement", window.ApronElement, groupId, window.Count, window.Name, memoryRecords);

                if (window.Ravisol?.Selected == true)
                    AddProductToMemory("Ravisol", window.Ravisol, groupId, window.Count, window.Name, memoryRecords);

                if (window.Stuisol?.Selected == true)
                    AddProductToMemory("Stuisol", window.Stuisol, groupId, window.Count, window.Name, memoryRecords);

                AddProductToMemory("GeneralMass", window.GeneralMass, groupId, window.Count, window.Name, memoryRecords);

                groupId++;
            }

            // Schreibe Records in Excel
            var columns = memoryRecords.SelectMany(r => r.Keys).Distinct().ToList();

            // Headers
            for (int i = 0; i < columns.Count; i++)
            {
                var cell = worksheet.Cell(1, i + 1);
                cell.SetValue(columns[i]);
                ApplyCellStyle(cell, isHeader: true);
            }

            // Data
            for (int row = 0; row < memoryRecords.Count; row++)
            {
                for (int col = 0; col < columns.Count; col++)
                {
                    var cell = worksheet.Cell(row + 2, col + 1);
                    cell.SetValue(memoryRecords[row].GetValueOrDefault(columns[col], ""));
                    ApplyCellStyle(cell);
                }
            }

            worksheet.Columns().AdjustToContents();
        }

        private static void AddProductToMemory(string productType, ProductBase product, int groupId, int count, string windowName, List<Dictionary<string, string>> records)
        {
            var record = new Dictionary<string, string>
            {
                ["RecordType"] = "Window",
                ["ProductType"] = productType,
                ["WindowGroupId"] = groupId.ToString(),
                ["Count"] = count.ToString(),
                ["WindowName"] = windowName
            };

            foreach (var prop in product.GetType().GetProperties())
            {
                var value = prop.GetValue(product);

                if (value is Field field)
                {
                    record[field.Id] = field.Value;
                    record[field.Id + "_Selected"] = field.Selected.HasValue ? (field.Selected.Value ? "1" : "0") : "0";
                }
                else if (prop.Name == "Column" && value is int number)
                {
                    record["Column"] = number.ToString();
                }
                else if (prop.Name == "Selected" && value is bool selected)
                {
                    record["Selected"] = selected.ToString();
                }
            }


            records.Add(record);
        }

        public static (List<WindowElement> windows, AdressHead adressHead) ImportFromExcel(string filePath, ExcelLoader excelLoader)
        {
            using var workbook = new XLWorkbook(filePath);
            var memorySheet = workbook.Worksheet("Memory");

            return ImportFromMemorySheet(memorySheet, excelLoader);
        }

        private static (List<WindowElement> windows, AdressHead adressHead) ImportFromMemorySheet(IXLWorksheet sheet, ExcelLoader excelLoader)
        {
            var records = ReadSheetToRecords(sheet);
            var adressHead = ImportAdressHead(records, excelLoader);
            var windows = ImportWindows(records, excelLoader);
            return (windows, adressHead);
        }

        private static List<Dictionary<string, string>> ReadSheetToRecords(IXLWorksheet sheet)
        {
            var records = new List<Dictionary<string, string>>();
            var headers = sheet.Row(1).CellsUsed().Select(c => c.Value.ToString()).ToList();

            foreach (var row in sheet.RowsUsed().Skip(1))
            {
                var record = new Dictionary<string, string>();
                for (int i = 0; i < headers.Count; i++)
                {
                    record[headers[i]] = row.Cell(i + 1).Value.ToString();
                }
                records.Add(record);
            }

            return records;
        }

        private static AdressHead ImportAdressHead(List<Dictionary<string, string>> records, ExcelLoader excelLoader)
        {
            var adressHeadRecord = records.First(r => r["RecordType"] == "AdressHead");
            var adressHead = new AdressHead(excelLoader);

            foreach (var prop in adressHead.GetType().GetProperties())
            {
                if (prop.GetValue(adressHead) is Field field && adressHeadRecord.ContainsKey(field.Id))
                {
                    field.Value = adressHeadRecord[field.Id];
                }
            }

            return adressHead;
        }

        private static List<WindowElement> ImportWindows(List<Dictionary<string, string>> records, ExcelLoader excelLoader)
        {
            return records
            .Where(r => r["RecordType"] == "Window")
            .GroupBy(r => r["WindowGroupId"])
            .Select(group =>
            {
                // Create WindowElement with base data
                var window = new WindowElement("Unbenanntes Fenster", excelLoader)
                {
                    Count = int.TryParse(group.First().GetValueOrDefault("Count"), out int count) ? count : 1,
                    Name = group.First().GetValueOrDefault("WindowName", "Unbenanntes Fenster"),
                    IsInitializing = true // Set flag to prevent column updates during initialization
                };

                // List of already created product types
                var existingProductTypes = new HashSet<string>();

                // Create products for each window in the group
                foreach (var record in group)
                {
                    var productType = record.GetValueOrDefault("ProductType");
                    existingProductTypes.Add(productType);
                    switch (productType)
                    {
                        case "ApronElement":
                            window.ApronElement = CreateProduct<ApronElement>(record, window, excelLoader);
                            break;
                        case "AluminumWindowSill":
                            window.AluminumWindowSill = CreateProduct<AluminumWindowSill>(record, window, excelLoader);
                            break;
                        case "Ravisol":
                            window.Ravisol = CreateProduct<Ravisol>(record, window, excelLoader);
                            break;
                        case "Stuisol":
                            window.Stuisol = CreateProduct<Stuisol>(record, window, excelLoader);
                            break;
                        case "SupportAngleWithSlope":
                            window.SupportAngleWithSlope = CreateProduct<SupportAngleWithSlope>(record, window, excelLoader);
                            break;
                        case "SupportWedgesWithSlope":
                            window.SupportWedgesWithSlope = CreateProduct<SupportWedgesWithSlope>(record, window, excelLoader);
                            break;
                        case "GeneralMass":
                            window.GeneralMass = CreateProduct<GeneralMass>(record, window, excelLoader);
                            break;
                    }
                }

                // Add standard products if they're missing
                if (!existingProductTypes.Contains("ApronElement"))
                    window.ApronElement = CreateProduct<ApronElement>(new Dictionary<string, string>(), window, excelLoader);
                if (!existingProductTypes.Contains("AluminumWindowSill"))
                    window.AluminumWindowSill = CreateProduct<AluminumWindowSill>(new Dictionary<string, string>(), window, excelLoader);
                if (!existingProductTypes.Contains("Ravisol"))
                    window.Ravisol = CreateProduct<Ravisol>(new Dictionary<string, string>(), window, excelLoader);
                if (!existingProductTypes.Contains("Stuisol"))
                    window.Stuisol = CreateProduct<Stuisol>(new Dictionary<string, string>(), window, excelLoader);
                if (!existingProductTypes.Contains("SupportAngleWithSlope"))
                    window.SupportAngleWithSlope = CreateProduct<SupportAngleWithSlope>(new Dictionary<string, string>(), window, excelLoader);
                if (!existingProductTypes.Contains("SupportWedgesWithSlope"))
                    window.SupportWedgesWithSlope = CreateProduct<SupportWedgesWithSlope>(new Dictionary<string, string>(), window, excelLoader);
                if (!existingProductTypes.Contains("GeneralMass"))
                    window.GeneralMass = CreateProduct<GeneralMass>(new Dictionary<string, string>(), window, excelLoader);


                // End initialization and update columns once
                window.IsInitializing = false;
                window.UpdateColumns(); // Now run update columns once at the end
                window.RegisterEvents();
                window.SetProductPositionString();

                return window;
            })
            .ToList();
        }

        private static T CreateProduct<T>(Dictionary<string, string> record, WindowElement window, ExcelLoader excelLoader) where T : ProductBase
        {

            var constructor = typeof(T).GetConstructor(new[] {
                typeof(WindowElement),  // WindowElement
                typeof(ExcelLoader)     // ExcelHelper
            });

            if (constructor == null)
            {
                throw new InvalidOperationException($"Constructor not found for type {typeof(T).FullName}");
            }

            // Create the product instance
            var product = (T)constructor.Invoke(new object[] { window, excelLoader });


            // Setze die Werte aus dem Record
            foreach (var prop in typeof(T).GetProperties())
            {
                if (prop.GetValue(product) is Field field && record.ContainsKey(field.Id))
                {
                    field.Value = record[field.Id]; 
                    if (record.ContainsKey(field.Id + "_Selected"))
                    {
                        field.Selected = record[field.Id + "_Selected"] == "1";
                    }
                }
            }

            if (record.TryGetValue("Column", out string columnValue) && int.TryParse(columnValue, out int column))
            {
                product.Column = column;
            }

            if (record.TryGetValue("Selected", out string selectedValue) && bool.TryParse(selectedValue, out bool selected))
            {
                product.Selected = selected;
            }

            return product;
        }
    }
}
