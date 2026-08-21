using Apps.Salsify.Handlers.LookupTable;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;
using ClosedXML.Excel;

namespace Apps.Salsify.Models.Requests.LookupTable;

public class DownloadLookupTableRequest
{
    [Display("Column letters", Description = "Columns to translate, as they appear in Excel - A, B, AA. Case-insensitive")]
    [DataSource(typeof(LookupTableColumnDataHandler))]
    public List<string> ColumnLetters { get; set; } = [];
    
    [Display("First row", Description = "Row to start from. Defaults to 2, which skips a single header row")]
    public int? FirstRow { get; set; }

    public void Validate()
    {
        if (FirstRow < 1)
            throw new PluginMisconfigurationException("The 'First row' value can't be less than 1");
        
        List<string> wrongColumnLetters = ColumnLetters.Where(columnLetter => !XLHelper.IsValidColumn(columnLetter)).ToList();
        if (wrongColumnLetters.Count != 0)
            throw new PluginMisconfigurationException($"Some letters are not valid Excel column letters: {string.Join(", ", wrongColumnLetters)}");
    }
}