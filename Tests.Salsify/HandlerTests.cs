using Apps.Salsify.Handlers;
using Apps.Salsify.Handlers.List;
using Apps.Salsify.Handlers.LookupTable;
using Apps.Salsify.Models.Identifiers;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Tests.Salsify.Base;

namespace Tests.Salsify;

[TestClass]
public class HandlerTests : TestBaseMultipleConnections
{
    [TestMethod, TargetConnections]
    public async Task PropertyDataHandler_ReturnsProperties(InvocationContext context)
    {
        // Arrange
        var handler = new PropertyDataHandler(context);
    
        // Act
        var result = await handler.GetDataAsync(new DataSourceContext { SearchString = "amazon" }, CancellationToken.None);
    
        // Assert
        PrintDataHandlerResult(result);
        Assert.IsNotNull(result);
    }
    
    [TestMethod, TargetConnections]
    public async Task ProductDataHandler_ReturnsProducts(InvocationContext context)
    {
        // Arrange
        var handler = new ProductDataHandler(context);
    
        // Act
        var result = await handler.GetDataAsync(new DataSourceContext { SearchString = "test" }, CancellationToken.None);
    
        // Assert
        PrintDataHandlerResult(result);
        Assert.IsNotNull(result);
    }
    
    [TestMethod, TargetConnections]
    public async Task LocaleDataHandler_ReturnsLocales(InvocationContext context)
    {
        // Arrange
        var handler = new LocaleDataHandler(context);
    
        // Act
        var result = await handler.GetDataAsync(new DataSourceContext { SearchString = "ca" }, CancellationToken.None);
    
        // Assert
        PrintDataHandlerResult(result);
        Assert.IsNotNull(result);
    }
    
    [TestMethod, TargetConnections]
    public async Task AssetDataHandler_ReturnsAssets(InvocationContext context)
    {
        // Arrange
        var handler = new AssetDataHandler(context);
    
        // Act
        var result = await handler.GetDataAsync(new DataSourceContext { SearchString = ".xlsx" }, CancellationToken.None);
    
        // Assert
        PrintDataHandlerResult(result);
        Assert.IsNotNull(result);
    }
    
    [TestMethod, TargetConnections]
    public async Task LookupTableDataHandler_ReturnsTableAssets(InvocationContext context)
    {
        // Arrange
        var handler = new LookupTableDataHandler(context);
    
        // Act
        var result = await handler.GetDataAsync(new DataSourceContext { SearchString = "" }, CancellationToken.None);
    
        // Assert
        PrintDataHandlerResult(result);
        Assert.IsNotNull(result);
    }
    
    [TestMethod, TargetConnections]
    public async Task AssetListNameDataHandler_ReturnsAssets(InvocationContext context)
    {
        // Arrange
        var handler = new AssetListNameDataHandler(context);
    
        // Act
        var result = await handler.GetDataAsync(new DataSourceContext { SearchString = "" }, CancellationToken.None);
    
        // Assert
        PrintDataHandlerResult(result);
        Assert.IsNotNull(result);
    }
    
    [TestMethod, TargetConnections]
    public async Task AssetListIdDataHandler_ReturnsAssets(InvocationContext context)
    {
        // Arrange
        var handler = new AssetListIdDataHandler(context);
    
        // Act
        var result = await handler.GetDataAsync(new DataSourceContext { SearchString = "" }, CancellationToken.None);
    
        // Assert
        PrintDataHandlerResult(result);
        Assert.IsNotNull(result);
    }
    
    [TestMethod, TargetConnections]
    public async Task PicklistPropertyDataHandler_ReturnsPicklistProperties(InvocationContext context)
    {
        // Arrange
        var handler = new PicklistPropertyDataHandler(context);
    
        // Act
        var result = await handler.GetDataAsync(new DataSourceContext { SearchString = "_LOC" }, CancellationToken.None);
    
        // Assert
        PrintDataHandlerResult(result);
        Assert.IsNotNull(result);
    }
    
    [TestMethod, TargetConnections]
    public async Task ProductListIdDataHandler_ReturnsListIds(InvocationContext context)
    {
        // Arrange
        var handler = new ProductListIdDataHandler(context);
    
        // Act
        var result = await handler.GetDataAsync(new DataSourceContext { SearchString = "Delete" }, CancellationToken.None);
    
        // Assert
        PrintDataHandlerResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod, TargetConnections]
    public async Task LookupTableSheetNameDataHandler_ReturnsSheetNames(InvocationContext context)
    {
        // Arrange
        var lookupTableIdentifier = new LookupTableIdentifier { AssetId = "525787423bdbfe2d6cf7eec6a24d018f56f7ce9d" };
        var handler = new LookupTableSheetNameDataHandler(context, lookupTableIdentifier);
    
        // Act
        var result = await handler.GetDataAsync(new DataSourceContext { SearchString = "" }, CancellationToken.None);
    
        // Assert
        PrintDataHandlerResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod, TargetConnections]
    public async Task LookupTableColumnDataHandler_ReturnsColumnNames(InvocationContext context)
    {
        // Arrange
        var lookupTableIdentifier = new LookupTableIdentifier { AssetId = "525787423bdbfe2d6cf7eec6a24d018f56f7ce9d" };
        var sheetNameIdentifier = new LookupTableSheetNameIdentifier { SheetName = "Sheet1" };
        var handler = new LookupTableColumnDataHandler(context, lookupTableIdentifier, sheetNameIdentifier);
    
        // Act
        var result = await handler.GetDataAsync(new DataSourceContext { SearchString = "" }, CancellationToken.None);
    
        // Assert
        PrintDataHandlerResult(result);
        Assert.IsNotNull(result);
    }
}
