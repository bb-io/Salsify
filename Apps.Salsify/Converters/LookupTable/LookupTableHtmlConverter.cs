using System.Net;
using Apps.Salsify.Constants;
using Apps.Salsify.Extensions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using ClosedXML.Excel;
using HtmlAgilityPack;

namespace Apps.Salsify.Converters.LookupTable;

public static class LookupTableHtmlConverter
{
    public static HtmlDocument GenerateHtml(
        XLWorkbook workbook,
        string sheetName,
        List<string> columnLetters,
        int startRow)
    {
        var sheet = workbook.ResolveSheet(sheetName);
        
        var doc = new HtmlDocument();
        doc.LoadHtml("<html><head><meta charset=\"utf-8\"></head><body></body></html>");
        var body = doc.DocumentNode.SelectSingleNode("//body")!;
        
        int lastRow = sheet.LastRowUsed()?.RowNumber() ?? startRow;
        var normalizedColumnLetters = columnLetters.Select(x => x.Trim().ToUpperInvariant()).Distinct(StringComparer.Ordinal);
        
        foreach (string letter in normalizedColumnLetters)
        {
            int columnNumber = letter.ResolveColumn();

            for (int rowNumber = startRow; rowNumber <= lastRow; rowNumber++)
            {
                var cell = sheet.Cell(rowNumber, columnNumber);
                if (!cell.IsTranslatable()) 
                    continue;

                body.AppendChild(BuildCellNode(doc, sheet.Name, cell));
            }
        }

        return doc;
    }
    
    private static HtmlNode BuildCellNode(HtmlDocument doc, string sheetName, IXLCell cell)
    {
        string cellAddress = cell.Address.ToString() ?? throw new PluginApplicationException("Cell address is empty");
        
        var node = doc.CreateElement("div");
        node.SetAttributeValue(HtmlConstants.SheetAttribute, sheetName);
        node.SetAttributeValue(HtmlConstants.KeyAttribute, cellAddress);
        node.InnerHtml = WebUtility.HtmlEncode(cell.GetString());

        return node;
    }
}