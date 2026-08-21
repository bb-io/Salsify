using Apps.Salsify.Handlers;
using Apps.Salsify.Handlers.List;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Salsify.Models.Requests.LookupTable;

public class UploadLookupTableRequest
{
    [Display("Content")]
    public FileReference Content { get; set; } = null!;

    [Display("Asset list name"), DataSource(typeof(AssetListNameDataHandler))]
    public string ListName { get; set; } = string.Empty;

    [Display("Source table asset ID", 
        Description = "Spreadsheet the translation is written into. Defaults to the asset the file was downloaded from")]
    [DataSource(typeof(AssetDataHandler))]
    public string? SourceAssetId { get; set; }
    
    [Display("Custom asset name")]
    public string? Name { get; set; }
}