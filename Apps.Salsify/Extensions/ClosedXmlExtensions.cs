using Blackbird.Applications.Sdk.Common.Exceptions;
using ClosedXML.Excel;

namespace Apps.Salsify.Extensions;

public static class ClosedXmlExtensions
{
    public static IXLWorksheet ResolveSheet(this XLWorkbook workbook, string sheetName)
    {
        if (string.IsNullOrWhiteSpace(sheetName))
            throw new PluginMisconfigurationException("Sheet name is required");

        var sheet = workbook.Worksheets.FirstOrDefault(x => string.Equals(x.Name, sheetName.Trim(), StringComparison.OrdinalIgnoreCase));
        if (sheet is not null) 
            return sheet;
        
        string availableSheetsPart = string.Join(", ", workbook.Worksheets.Select(x => x.Name));
        throw new PluginMisconfigurationException(
            $"Sheet '{sheetName}' was not found in the file. Available sheets: {availableSheetsPart}");
    }

    public static int ResolveColumn(this string letter)
    {
        return XLHelper.IsValidColumn(letter)
            ? XLHelper.GetColumnNumberFromLetter(letter)
            : throw new PluginMisconfigurationException($"'{letter}' is not a valid Excel column letter");
    }
    
    // Formulas are skipped deliberately (writing a translated value instead of formula breaks the sheet)
    // Numbers, dates and blanks are not translatable
    public static bool IsTranslatable(this IXLCell cell)
    {
        return !cell.HasFormula && cell.DataType == XLDataType.Text && !string.IsNullOrWhiteSpace(cell.GetString());
    }
}