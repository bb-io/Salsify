using Apps.Salsify.Models.Entities.Asset;
using Newtonsoft.Json;

namespace Apps.Salsify.Events.Webhooks.Models.Payloads;

public class AssetWebhookPayload
{
    [JsonProperty("digital_assets")]
    public List<AssetEntity> Assets { get; set; } = [];
}