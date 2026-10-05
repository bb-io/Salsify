using Apps.Salsify.Actions;
using Apps.Salsify.Models.Identifiers;
using Apps.Salsify.Models.Requests.Product;
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
        // If you query products right after adding one to a list, the API might not return the added one right away
        await Task.Delay(TimeSpan.FromSeconds(3));
        
        var productActions = new ProductActions(invocationContext, FileManager);
        var searchProductsRequest = new SearchProductsRequest { ListId = listId };
        var products = await productActions.SearchProducts(searchProductsRequest);

        var productAddedToList = products.Products.FirstOrDefault(x => x.Id == productId);
        Assert.IsNotNull(productAddedToList, $"No product ID {productId} found in list ID {listId}");
    }
}