using Newtonsoft.Json;

namespace Apps.Salsify.Models.Utility.Current;

public class CurrentResponse
{
    [JsonProperty("role_properties")]
    public List<RolePropertyResponse> RoleProperties { get; set; } = [];
}