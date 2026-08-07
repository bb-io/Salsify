using Apps.Salsify.Actions;
using Apps.Salsify.Models.Identifiers;
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

    [TestMethod, TargetConnections]
    public async Task GetProperty_ReturnsProperty(InvocationContext context)
    {
        // Arrange
        var actions = new PropertyActions(context);
        var input = new PropertyIdentifier { PropertyId = "s-cd3753af-1932-46bc-b8da-9ae44cbaeb86" };

        // Act
        var response = await actions.GetProperty(input);

        // Assert
        PrintResult(response);
        Assert.IsNotNull(response);
    }

    [TestMethod, TargetConnections]
    public async Task CreateProperty_ReturnsCreatedProperty(InvocationContext context)
    {
        // Arrange
        var actions = new PropertyActions(context);
        var input = new CreatePropertyRequest
        {
            PropertyId = "your-new-property",
            Type = "boolean"
        };

        // Act
        var response = await actions.CreateProperty(input);

        // Assert
        PrintResult(response);
        Assert.IsNotNull(response);
    }
}
