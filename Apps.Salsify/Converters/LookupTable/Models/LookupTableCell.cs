namespace Apps.Salsify.Converters.LookupTable.Models;

public record LookupTableCell(string Sheet, string Address, string Text)
{
    public string Describe() => $"{Sheet}!{Address}";
}