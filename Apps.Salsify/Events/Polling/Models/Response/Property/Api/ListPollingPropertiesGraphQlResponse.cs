using Apps.Salsify.Models.Entities.Properties;
using Apps.Salsify.Models.Utility.GraphQl;
using Newtonsoft.Json;

namespace Apps.Salsify.Events.Polling.Models.Response.Property.Api;

public class ListPollingPropertiesGraphQlResponse : IGraphQlPaged<PropertyPollingEntity>
{
    [JsonProperty("organization")]
    public OrganizationResponse Organization { get; set; } = null!;

    [JsonIgnore]
    public GraphQlPage<PropertyPollingEntity> Page => Organization.Properties;
}

public class OrganizationResponse
{
    [JsonProperty("properties")]
    public GraphQlPage<PropertyPollingEntity> Properties { get; set; } = null!;
}