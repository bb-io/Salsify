using Apps.Salsify.Actions;
using Apps.Salsify.Models.Identifiers;
using Apps.Salsify.Models.Requests.Product;
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
        var actions = new ProductActions(context);
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
        var actions = new ProductActions(context);
        var identifier = new ProductIdentifier { ProductId = "s-c3659828-bf4e-4ade-9f06-11448cc7169e" };

        // Act
        var result = await actions.GetProduct(identifier);

        // Assert
        PrintResult(result);
        Assert.IsNotNull(result);
    }
}