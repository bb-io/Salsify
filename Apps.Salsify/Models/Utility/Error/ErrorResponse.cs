using Newtonsoft.Json;

namespace Apps.Salsify.Models.Utility.Error;

public class ErrorResponse
{
    [JsonProperty("error")]
    public string? Error { get; set; }

    [JsonProperty("errors")]
    public List<string>? Errors { get; set; }
}