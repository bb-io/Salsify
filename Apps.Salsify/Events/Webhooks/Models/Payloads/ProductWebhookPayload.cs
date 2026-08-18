using Apps.Salsify.Models.Entities.Product;
using Newtonsoft.Json;

namespace Apps.Salsify.Events.Webhooks.Models.Payloads;

public class ProductWebhookPayload
{
    [JsonProperty("products")]
    public List<ProductEntity> Products { get; set; } = [];
}