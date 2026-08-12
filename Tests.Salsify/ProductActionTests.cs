using Apps.Salsify.Actions;
using Apps.Salsify.Models.Identifiers;
using Apps.Salsify.Models.Identifiers.Optional;
using Apps.Salsify.Models.Requests.Product;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Tests.Salsify.Base;

namespace Tests.Salsify;

[TestClass]
public class ProductActionTests : TestBaseMultipleConnections
{
    [TestMethod, TargetConnections]
    public async Task SearchProducts_ReturnsProducts(InvocationContext context)
    {
        // Arrange
        var actions = new ProductActions(context, FileManager);
        var input = new SearchProductsRequest
        {
            UpdatedAfter = DateTime.UtcNow - TimeSpan.FromDays(2),
            UpdatedBefore = DateTime.UtcNow + TimeSpan.FromHours(1),
            NameContains = "test"
        };

        // Act
        var result = await actions.SearchProducts(input);

        // Assert
        PrintResult(result);
        Assert.IsNotNull(result);
    }
    
    [TestMethod, TargetConnections]
    public async Task GetProduct_ReturnsProduct(InvocationContext context)
    {
        // Arrange
        var actions = new ProductActions(context, FileManager);
        var identifier = new ProductIdentifier { ProductId = "partcodeid" };

        // Act
        var result = await actions.GetProduct(identifier);

        // Assert
        PrintResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod, TargetConnections]
    public async Task DownloadProduct_IsSuccess(InvocationContext context)
    {
        // Arrange
        var actions = new ProductActions(context, FileManager);
        var identifier = new ProductIdentifier { ProductId = "partcodeid" };
        var downloadInput = new DownloadProductRequest
        {
            OnlyLocalizableProperties = false,
            Locale = "en-US",
        };

        // Act
        var result = await actions.DownloadProduct(identifier, downloadInput);

        // Assert
        Assert.IsNotNull(result.Content);
        TestContext.WriteLine(result.Content.Name);
    }

    [TestMethod, TargetConnections]
    public async Task UploadProduct_IsSuccess(InvocationContext context)
    {
        // Arrange
        var actions = new ProductActions(context, FileManager);
        var uploadInput = new UploadProductRequest
        {
            Content = new FileReference { Name = "test.html" },
            Locale = "fr-CA"
        };
        var productIdentifier = new ProductOptionalIdentifier { };

        // Act
        await actions.UploadProduct(uploadInput, productIdentifier);
    }

    [TestMethod, TargetConnections]
    public async Task CreateProduct_IsSuccess(InvocationContext context)
    {
        // Arrange
        var actions = new ProductActions(context, FileManager);
        string id = "test from tests bb2";
        var createInput = new CreateProductRequest
        {
            Id = id,
            Name = "test name 123"
        };

        // Act
        await actions.CreateProduct(createInput);

        // Assert
        var productIdentifier = new ProductIdentifier { ProductId = id };
        var createdProduct = await actions.GetProduct(productIdentifier);
        
        Assert.IsNotNull(createdProduct);
        PrintResult(createdProduct);
    }

    [TestMethod, TargetConnections]
    public async Task DeleteProduct_IsSuccess(InvocationContext context)
    {
        // Arrange
        var actions = new ProductActions(context, FileManager);
        var productIdentifier = new ProductIdentifier { ProductId = "test from tests bb2" };

        // Act
        await actions.DeleteProduct(productIdentifier);

        // Assert
        await Assert.ThrowsExceptionAsync<PluginApplicationException>(() => actions.GetProduct(productIdentifier));
    }

    [TestMethod, TargetConnections]
    public async Task UpdatePropertyValue_IsSuccess(InvocationContext context)
    {
        // Arrange
        var actions = new ProductActions(context, FileManager);
        string propertyId = "RichText_LOC";
        string propertyValue = "test updated richtext";
        
        var productIdentifier = new ProductIdentifier { ProductId = "partcodeid" };
        var propertyIdentifier = new PropertyIdentifier { PropertyId = propertyId };
        var updateInput = new UpdatePropertyValueRequest
        {
            PropertyValue = propertyValue,
            Locale = "fr-CA"
        };

        // Act
        await actions.UpdatePropertyValue(productIdentifier, propertyIdentifier, updateInput);
    }
}