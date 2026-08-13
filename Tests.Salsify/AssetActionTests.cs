using Apps.Salsify.Actions;
using Apps.Salsify.Models.Requests.Asset;
using Blackbird.Applications.Sdk.Common.Invocation;
using Tests.Salsify.Base;

namespace Tests.Salsify;

[TestClass]
public class AssetActionTests : TestBaseMultipleConnections
{
    [TestMethod, TargetConnections]
    public async Task SearchAssets_ReturnsAssets(InvocationContext context)
    {
        // Arrange
        var actions = new AssetActions(context, FileManager);
        var searchInput = new SearchAssetsRequest
        {
            NameContains = ".xlsx"
        };

        // Act
        var result = await actions.SearchAssets(searchInput);
        
        // Assert
        PrintResult(result);
        Assert.IsNotNull(result);
    }
}