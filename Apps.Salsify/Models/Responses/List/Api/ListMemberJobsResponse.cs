using Newtonsoft.Json;

namespace Apps.Salsify.Models.Responses.List.Api;

public class ListMemberJobsResponse
{
    [JsonProperty("list_members_job_id")]
    public string ListMembersJobId { get; set; } = string.Empty;
}