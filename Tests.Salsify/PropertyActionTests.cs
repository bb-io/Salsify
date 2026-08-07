using Apps.Salsify.Actions;
using Apps.Salsify.Models.Requests.Property;
using Blackbird.Applications.Sdk.Common.Invocation;
using Tests.Salsify.Base;

namespace Tests.Salsify;

[TestClass]
public class PropertyActionTests : TestBaseMultipleConnections
{
    [TestMethod, TargetConnections]
    public async Task SearchProperties_ReturnsProperties(InvocationContext context)
    {
        // Arrange
        var actions = new PropertyActions(context);
        var input = new SearchPropertiesRequest { NameContains = "Product" };

        // Act
        var response = await actions.SearchProperties(input);

        // Assert
        PrintResult(response);
        Assert.IsNotNull(response);
    }
}
