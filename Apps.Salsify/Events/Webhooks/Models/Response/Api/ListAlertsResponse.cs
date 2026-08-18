using Apps.Salsify.Models.Entities.Alert;
using Apps.Salsify.Models.Utility.Pagination;
using Newtonsoft.Json;

namespace Apps.Salsify.Events.Webhooks.Models.Response.Api;

public class ListAlertsResponse : PaginatedResponse<AlertEntity>
{
    [JsonProperty("data")] 
    public override List<AlertEntity> Items { get; set; } = [];
}