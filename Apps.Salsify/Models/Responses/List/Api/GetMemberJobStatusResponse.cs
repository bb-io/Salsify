using Newtonsoft.Json;

namespace Apps.Salsify.Models.Responses.List.Api;

public class GetMemberJobStatusResponse
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("status")]
    public string Status { get; set; } = string.Empty;
}