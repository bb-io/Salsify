using Apps.Salsify.Api;
using Apps.Salsify.Constants.GraphQl;
using Apps.Salsify.Events.Polling.Models.Memory;
using Apps.Salsify.Events.Polling.Models.Request.Property;
using Apps.Salsify.Events.Polling.Models.Response.Property;
using Apps.Salsify.Events.Polling.Models.Response.Property.Api;
using Apps.Salsify.Helpers.Event;
using Apps.Salsify.Models.Entities.Properties;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;

namespace Apps.Salsify.Events.Polling;

[PollingEventList("Properties")]
public class PropertyPollingList(InvocationContext invocationContext) : SalsifyInvocable(invocationContext)
{
    [MultipleEvents]
    [PollingEvent("On property created or updated", Description = "Triggered when a property is created or its definition is updated")]
    public async Task<PollingEventResponse<DateMemory, List<PropertyPollingResponse>>> OnPropertyCreatedOrUpdated(
        PollingEventRequest<DateMemory> pollingRequest,
        [PollingEventParameter] OnPropertyCreatedOrUpdatedRequest input)
    {
        // Salsify timestamps are second-precision so a sub-second cursor can skip changes made later in the same second
        // One second of overlap risks a duplicate instead of a gap
        var pollingStartTime = DateTime.UtcNow.AddSeconds(-1);
        
        if (pollingRequest.Memory?.LastPollingTime is null)
            return PollingResult.DoNotFly<List<PropertyPollingResponse>>(pollingStartTime);

        var request = new GraphQlRequest("PropertyIndexPolling", GraphQlQueries.PropertyIndexPolling, new
        {
            localizable = input.OnlyLocalizable is true ? "LOCALIZABLE" : "ALL"
        });
        var properties = await GraphQlClient.Paginate<ListPollingPropertiesGraphQlResponse, PropertyPollingEntity>(request);
        
        var filtered = properties.Where(x => x.UpdatedAt > pollingRequest.Memory.LastPollingTime);
        var result = filtered.Select(x => new PropertyPollingResponse(x)).ToList();
        
        return result.Count > 0
            ? PollingResult.Fly(result, pollingStartTime)
            : PollingResult.DoNotFly<List<PropertyPollingResponse>>(pollingStartTime);
    }
}