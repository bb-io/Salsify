using Apps.Salsify.Models.Entities.Properties;
using Apps.Salsify.Models.Utility.Pagination;
using Newtonsoft.Json;

namespace Apps.Salsify.Models.Responses.Property.Api;

public class ListPropertiesResponse : PaginatedResponse<PropertyListEntity>
{
    [JsonProperty("properties")] 
    public override List<PropertyListEntity> Items { get; set; } = [];
}