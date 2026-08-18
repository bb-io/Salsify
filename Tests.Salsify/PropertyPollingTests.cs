using Apps.Salsify.Events.Polling;
using Apps.Salsify.Events.Polling.Models;
using Apps.Salsify.Events.Polling.Models.Request.Property;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using Tests.Salsify.Base;

namespace Tests.Salsify;

[TestClass]
public class PropertyPollingTests : TestBaseMultipleConnections
{
    [TestMethod, TargetConnections]
    public async Task OnPropertyCreatedOrUpdated_ReturnsProperties(InvocationContext context)
    {
        // Arrange
        var polling = new PropertyPollingList(context);
        var memory = new PollingMemory { LastPollingTime = DateTime.UtcNow - TimeSpan.FromMinutes(1) };
        var request = new PollingEventRequest<PollingMemory> { Memory = memory };
        var input = new OnPropertyCreatedOrUpdatedRequest { };

        // Act
        var result = await polling.OnPropertyCreatedOrUpdated(request, input);

        // Assert
        PrintResult(result);
        Assert.IsNotNull(result);
    }
}