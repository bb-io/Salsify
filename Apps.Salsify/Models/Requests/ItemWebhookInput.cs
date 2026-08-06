using Blackbird.Applications.Sdk.Common;

namespace Apps.Salsify.Models.Requests;

// Optional inputs the user can set on the webhook in the UI.
// Use them to filter which events actually trigger the bird (see the preflight check in the webhook list).
public class ItemWebhookInput
{
    [Display("Content ID", Description = "Only trigger when the event refers to this content item.")]
    public string? ContentId { get; set; }
}
