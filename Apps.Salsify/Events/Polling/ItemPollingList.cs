using Apps.Salsify.Models.Polling;
using Apps.Salsify.Models.Responses;
using Blackbird.Applications.SDK.Blueprints;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;

namespace Apps.Salsify.Events.Polling;

[PollingEventList("Items")]
public class ItemPollingList(InvocationContext invocationContext) : SalsifyInvocable(invocationContext)
{
    [BlueprintEventDefinition(BlueprintEvent.ContentCreatedOrUpdatedMultiple)]
    [PollingEvent("On items created or updated",
        Description = "Triggered on an interval and outputs the knowledge items created or updated since the previous poll.")]
    public async Task<PollingEventResponse<PollingMemory, ItemsEventResponse>> OnArticlesCreatedOrUpdated(
        PollingEventRequest<PollingMemory> request)
    {
        if (request.Memory?.LastPollingTime is null)
            return Baseline<ItemsEventResponse>();

        // Your real implementation to fetch items from your data source goes here.
        var items = new List<ItemResponse>();
        return new PollingEventResponse<PollingMemory, ItemsEventResponse>
        {
            FlyBird = items.Count > 0,
            Memory = new PollingMemory { LastPollingTime = DateTime.UtcNow },
            Result = items.Count > 0
                ? new ItemsEventResponse { Items = items, TotalCount = items.Count }
                : null
        };
    }
    
    private static PollingEventResponse<PollingMemory, TResult> Baseline<TResult>() where TResult : class =>
        new()
        {
            FlyBird = false,
            Memory = new PollingMemory { LastPollingTime = DateTime.UtcNow },
            Result = null
        };
}