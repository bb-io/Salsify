using Apps.Salsify.Actions;
using Apps.Salsify.Models.Identifiers;
using Apps.Salsify.Models.Identifiers.Optional;
using Apps.Salsify.Models.Requests.Property;
using Blackbird.Applications.Sdk.Common.Files;
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
        var actions = new PropertyActions(context, FileManager);
        var input = new SearchPropertiesRequest
        {
            NameContains = "",
            OnlyLocalizable = true,
            Type = "link"
        };

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
        var actions = new PropertyActions(context, FileManager);
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
        var actions = new PropertyActions(context, FileManager);
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
    
    [TestMethod, TargetConnections]
    public async Task UpdateProperty_IsSuccess(InvocationContext context)
    {
        // Arrange
        var actions = new PropertyActions(context, FileManager);
        
        string newPropertyName = "your-updated-property1";
        var propertyIdentifier = new PropertyIdentifier { PropertyId = "your-new-property" };
        var input = new UpdatePropertyRequest
        {
            Name = newPropertyName
        };

        // Act
        await actions.UpdateProperty(propertyIdentifier, input);

        // Assert
        var updatedProperty = await actions.GetProperty(propertyIdentifier);
        Assert.AreEqual(updatedProperty.Name, newPropertyName);
        PrintResult(updatedProperty);
    }
    
    [TestMethod, TargetConnections]
    public async Task DownloadPicklistValues_IsSuccess(InvocationContext context)
    {
        // Arrange
        var actions = new PropertyActions(context, FileManager);
        var picklistIdentifier = new PicklistIdentifier { PicklistId = "sinkType_LOC" };
        var localeIdentifier = new LocaleOptionalIdentifier { Locale = "en-US" };

        // Act
        var result = await actions.DownloadPicklistValues(picklistIdentifier, localeIdentifier);
        
        // Assert
        TestContext.WriteLine(result.Content.Name);
        Assert.IsNotNull(result);
    }
    
    [TestMethod, TargetConnections]
    public async Task UploadPicklistValues_IsSuccess(InvocationContext context)
    {
        // Arrange
        var actions = new PropertyActions(context, FileManager);
        var uploadInput = new UploadPicklistValuesRequest { Content = new FileReference { Name = "test_loc.html" } };
        var localeIdentifier = new LocaleIdentifier { Locale = "en-CA" };
        var picklistIdentifier = new PicklistOptionalIdentifier { };

        // Act
        await actions.UploadPicklistValues(uploadInput, localeIdentifier, picklistIdentifier);
    }
}
