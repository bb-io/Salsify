using Apps.Salsify.Models.Entities.Properties.Enumerated;
using Apps.Salsify.Models.Utility.GraphQl;
using Newtonsoft.Json;

namespace Apps.Salsify.Models.Responses.Property.Api;

public class ListPropertyValuesResponse : IGraphQlPaged<EnumeratedValueEntity>
{
    [JsonProperty("organization")] 
    public EnumeratedValuesOrganizationResponse Organization { get; set; } = new();

    [JsonIgnore] 
    public GraphQlPage<EnumeratedValueEntity> Page => Organization.Property.EnumeratedValues;
}

public class EnumeratedValuesOrganizationResponse
{
    [JsonProperty("property")] 
    public EnumeratedValuesPropertyResponse Property { get; set; } = new();
}

public class EnumeratedValuesPropertyResponse
{
    [JsonProperty("enumeratedValues")] 
    public GraphQlPage<EnumeratedValueEntity> EnumeratedValues { get; set; } = new();
}