using Newtonsoft.Json;

namespace Apps.Appname.Models.Requests;

// Represents the JSON body that the third-party system POSTs to Blackbird when the event fires.
// Model only the fields you actually need from the payload.
public class ItemWebhookPayload
{
    [JsonProperty("id")] public string Id { get; set; } = string.Empty;
}
