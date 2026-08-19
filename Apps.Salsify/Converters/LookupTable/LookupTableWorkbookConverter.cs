using Apps.Salsify.Constants;
using Apps.Salsify.Converters.LookupTable.Models;
using ClosedXML.Excel;
using HtmlAgilityPack;

namespace Apps.Salsify.Converters.LookupTable;

public static class LookupTableWorkbookConverter
{
    public static List<LookupTableCell> ParseCells(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var cells = new List<LookupTableCell>();
        var nodes = doc.DocumentNode.Descendants().Where(x => x.Attributes.Contains(HtmlConstants.KeyAttribute));

        foreach (var node in nodes)
        {
            string address = node.GetAttributeValue(HtmlConstants.KeyAttribute, string.Empty);
            string sheet = node.GetAttributeValue(HtmlConstants.SheetAttribute, string.Empty);
            
            if (string.IsNullOrEmpty(address) || string.IsNullOrEmpty(sheet)) 
                continue;

            string text = HtmlEntity.DeEntitize(node.InnerText) ?? string.Empty;
            if (string.IsNullOrWhiteSpace(text)) 
                continue;

            cells.Add(new LookupTableCell(sheet, address, text));
        }

        return cells;
    }
    
    public static LookupTableWriteResult WriteCells(XLWorkbook workbook, IEnumerable<LookupTableCell> cells)
    {
        int written = 0;
        var skipped = new List<string>();

        foreach (var sheetGroup in cells.GroupBy(x => x.Sheet, StringComparer.OrdinalIgnoreCase))
        {
            var sheet = workbook.Worksheets.FirstOrDefault(x => string.Equals(x.Name, sheetGroup.Key, StringComparison.OrdinalIgnoreCase));

            if (sheet is null)
            {
                skipped.AddRange(sheetGroup.Select(x => x.Describe()));
                continue;
            }

            foreach (var item in sheetGroup)
            {
                if (!XLHelper.IsValidA1Address(item.Address))
                {
                    skipped.Add(item.Describe());
                    continue;
                }

                var cell = sheet.Cell(item.Address);
                if (cell.HasFormula)
                {
                    skipped.Add(item.Describe());
                    continue;
                }

                cell.SetValue(item.Text);
                written++;
            }
        }

        return new(written, skipped);
    }
}