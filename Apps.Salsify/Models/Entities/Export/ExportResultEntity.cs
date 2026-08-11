using Newtonsoft.Json;

namespace Apps.Salsify.Models.Entities.Export;

public class ExportResultEntity
{
    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; } = string.Empty;

    [JsonProperty("end_time")]
    public DateTime? EndTime { get; set; }

    [JsonProperty("progress")]
    public int Progress { get; set; }

    [JsonProperty("url")]
    public string? Url { get; set; }

    [JsonProperty("failure_reason")]
    public string? FailureReason { get; set; }

    public bool ExportEnded => EndTime.HasValue;
    
    public bool ExportEndedSuccessfully => ExportEnded && Status == "completed" && FailureReason == null && Progress == 100;
}