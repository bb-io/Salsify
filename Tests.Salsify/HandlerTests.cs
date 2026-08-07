using Apps.Salsify.Handlers;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Tests.Salsify.Base;

namespace Tests.Salsify;

[TestClass]
public class HandlerTests : TestBaseMultipleConnections
{
    [TestMethod, TargetConnections]
    public async Task PropertyDataHandler_ReturnsProperties(InvocationContext context)
    {
        // Arrange
        var handler = new PropertyDataHandler(context);
    
        // Act
        var result = await handler.GetDataAsync(new DataSourceContext { SearchString = "amazon" }, CancellationToken.None);
    
        // Assert
        PrintDataHandlerResult(result);
        Assert.IsNotNull(result);
    }
}
