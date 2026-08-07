using Apps.Salsify.Models.Entities.Product;
using Apps.Salsify.Models.Utility.Pagination;
using Newtonsoft.Json;

namespace Apps.Salsify.Models.Responses.Product.Api;

public class ListProductsResponse : PaginatedResponse<ProductEntity>
{
    [JsonProperty("data")]
    public override List<ProductEntity> Items { get; set; } = [];
}