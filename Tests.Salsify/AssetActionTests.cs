using Apps.Salsify.Actions;
using Apps.Salsify.Models.Identifiers;
using Apps.Salsify.Models.Requests.Asset;
using Blackbird.Applications.Sdk.Common.Files;
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
    
    [TestMethod, TargetConnections]
    public async Task GetAsset_ReturnsAsset(InvocationContext context)
    {
        // Arrange
        var actions = new AssetActions(context, FileManager);
        var assetIdentifier = new AssetIdentifier { AssetId = "1b58312628bafc4f5ffbea3b66d3cdcf2012eca8" };

        // Act
        var result = await actions.GetAsset(assetIdentifier);
        
        // Assert
        PrintResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod, TargetConnections]
    public async Task DownloadAsset_IsSuccess(InvocationContext context)
    {
        // Arrange
        var actions = new AssetActions(context, FileManager);
        var assetIdentifier = new AssetIdentifier { AssetId = "1b58312628bafc4f5ffbea3b66d3cdcf2012eca8" };

        // Act
        var result = await actions.DownloadAsset(assetIdentifier);
        
        // Assert
        TestContext.WriteLine(result.Content.Name);
        TestContext.WriteLine(result.Content.ContentType);
        Assert.IsNotNull(result);
    }
    
    [TestMethod, TargetConnections]
    public async Task UploadAsset_ReturnsAsset(InvocationContext context)
    {
        // Arrange
        var actions = new AssetActions(context, FileManager);
        var uploadInput = new UploadAssetRequest
        {
            Content = new FileReference { Name = "test123.xlsx" },
            ListName = "lookup table testing - Blackbird",
            Name = "updated name 123"
        };

        // Act
        var result = await actions.UploadAsset(uploadInput);
        
        // Assert
        PrintResult(result);
        Assert.IsNotNull(result);
    }
}