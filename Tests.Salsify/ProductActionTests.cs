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
            //UpdatedAfter = DateTime.UtcNow - TimeSpan.FromDays(2),
            //UpdatedBefore = DateTime.UtcNow + TimeSpan.FromHours(1),
            //ListId = "1237442",
            //PropertyNames =  ["Material_LOC"],
            //PropertyValues = ["ABS"],
            CustomQuery = "'Part Code (ID)':'partcodeid'"
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
        var localeIdentifier = new LocaleOptionalIdentifier { Locale = "fr-CA" };
        var downloadInput = new DownloadProductRequest
        {
            OnlyLocalizableProperties = true,
        };

        // Act
        var result = await actions.DownloadProduct(identifier, downloadInput, localeIdentifier);

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
        };
        var localeIdentifier = new LocaleIdentifier { Locale = "fr-CA" };
        var productIdentifier = new ProductOptionalIdentifier { };

        // Act
        await actions.UploadProduct(uploadInput, localeIdentifier, productIdentifier);
    }

    [TestMethod, TargetConnections]
    public async Task CreateProduct_IsSuccess(InvocationContext context)
    {
        // Arrange
        var actions = new ProductActions(context, FileManager);
        var createInput = new CreateProductRequest
        {
            Id = "test from tests bb",
            Name = "test name 123"
        };

        // Act
        var result = await actions.CreateProduct(createInput);

        // Assert
        Assert.IsNotNull(result);
        PrintResult(result);
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
        var localeIdentifier = new LocaleOptionalIdentifier { Locale = "fr-CA" };
        var updateInput = new UpdatePropertyValueRequest
        {
            PropertyValue = propertyValue,
        };

        // Act
        await actions.UpdatePropertyValue(productIdentifier, propertyIdentifier, updateInput, localeIdentifier);
    }
}