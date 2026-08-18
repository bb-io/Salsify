using Newtonsoft.Json;

namespace Apps.Salsify.Models.Entities.Alert;

public class AlertEntity
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("webhook_url")]
    public string WebhookUrl { get; set; } = string.Empty;
}