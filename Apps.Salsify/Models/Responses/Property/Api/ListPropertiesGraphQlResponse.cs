using Apps.Salsify.Models.Entities.Properties;
using Apps.Salsify.Models.Utility.GraphQl;
using Newtonsoft.Json;

namespace Apps.Salsify.Models.Responses.Property.Api;

public class ListPropertiesGraphQlResponse : IGraphQlPaged<PropertyGraphQlEntity>
{
    [JsonProperty("organization")]
    public OrganizationResponse Organization { get; set; } = null!;

    [JsonIgnore]
    public GraphQlPage<PropertyGraphQlEntity> Page => Organization.Properties;
}

public class OrganizationResponse
{
    [JsonProperty("properties")]
    public GraphQlPage<PropertyGraphQlEntity> Properties { get; set; } = null!;
}