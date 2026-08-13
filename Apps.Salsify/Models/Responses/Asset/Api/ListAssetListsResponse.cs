using Apps.Salsify.Models.Entities.Asset;
using Apps.Salsify.Models.Utility.Pagination;
using Newtonsoft.Json;

namespace Apps.Salsify.Models.Responses.Asset.Api;

public class ListAssetListsResponse : PaginatedResponse<AssetListEntity>
{
    [JsonProperty("lists")]
    public override List<AssetListEntity> Items { get; set; } = [];
}