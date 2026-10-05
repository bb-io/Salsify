using Apps.Salsify.Actions;
using Apps.Salsify.Models.Identifiers;
using Apps.Salsify.Models.Requests.Product;
using Apps.Salsify.Models.Responses.Product;
using Blackbird.Applications.Sdk.Common.Invocation;
using Tests.Salsify.Base;

namespace Tests.Salsify;

[TestClass]
public class ListActionTests : TestBaseMultipleConnections
{
    [TestMethod, TargetConnections]
    public async Task AddProductToList_IsSuccess(InvocationContext invocationContext)
    {
        // Arrange
        string productId = "partcodeid";
        string listId = "1237442";
        
        var listActions = new ListActions(invocationContext);
        var productIdentifier = new ProductIdentifier { ProductId = productId };
        var productListIdentifier = new ProductListIdentifier { ProductListId = listId };

        // Act
        await listActions.AddProductToList(productIdentifier, productListIdentifier);
        
        // Assert
        await Task.Delay(TimeSpan.FromSeconds(5));  // It takes some time for the API to sync data
        
        var productAddedToList = await FindProductInList(invocationContext, listId, productId);
        Assert.IsNotNull(productAddedToList, $"No product ID {productId} found in list ID {listId}. Check from the UI");
    }

    [TestMethod, TargetConnections]
    public async Task RemoveProductFromList_IsSuccess(InvocationContext invocationContext)
    {
        // Arrange
        string productId = "partcodeid";
        string listId = "1237442";
        
        var listActions = new ListActions(invocationContext);
        var productIdentifier = new ProductIdentifier { ProductId = productId };
        var productListIdentifier = new ProductListIdentifier { ProductListId = listId };

        // Act
        await listActions.RemoveProductFromList(productIdentifier, productListIdentifier);
        
        // Assert
        await Task.Delay(TimeSpan.FromSeconds(5));  // It takes some time for the API to sync data
        
        var removedProduct = await FindProductInList(invocationContext, listId, productId);
        Assert.IsNull(removedProduct, $"Product ID {productId} is still in list ID {listId}. Check from the UI");
    }

    private async Task<ProductResponse?> FindProductInList(
        InvocationContext invocationContext, 
        string listId,
        string productId)
    {
        var productActions = new ProductActions(invocationContext, FileManager);
        var searchProductsRequest = new SearchProductsRequest { ListId = listId };
        var products = await productActions.SearchProducts(searchProductsRequest);

        return products.Products.FirstOrDefault(x => string.Equals(x.Id, productId, StringComparison.OrdinalIgnoreCase));
    }
}