using Apps.Salsify.Models.Entities.Asset;
using Apps.Salsify.Models.Utility.Pagination;
using Newtonsoft.Json;

namespace Apps.Salsify.Models.Responses.Asset.Api;

public class ListAssetsResponse : PaginatedResponse<AssetEntity>
{
    [JsonProperty("data")] 
    public override List<AssetEntity> Items { get; set; } = [];
}