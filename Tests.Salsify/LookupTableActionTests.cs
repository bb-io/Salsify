using Apps.Salsify.Actions;
using Apps.Salsify.Models.Identifiers;
using Apps.Salsify.Models.Requests.LookupTable;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Tests.Salsify.Base;

namespace Tests.Salsify;

[TestClass]
public class LookupTableActionTests : TestBaseMultipleConnections
{
    [TestMethod, TargetConnections]
    public async Task DownloadLookupTable_IsSuccess(InvocationContext context)
    {
        // Arrange
        var actions = new LookupTableActions(context, FileManager);
        var tableIdentifier = new LookupTableIdentifier { AssetId = "525787423bdbfe2d6cf7eec6a24d018f56f7ce9d" };
        var sheetName = new LookupTableSheetNameIdentifier { SheetName = "Sheet1" };
        var downloadInput = new DownloadLookupTableRequest
        {
            ColumnLetters = ["c", "d", "b"],
        };

        // Act
        var result = await actions.DownloadLookupTable(tableIdentifier, sheetName, downloadInput);
        
        // Assert
        PrintFileResult(result.Content);
        Assert.IsNotNull(result);
    }
    
    [TestMethod, TargetConnections]
    public async Task UploadLookupTable_ReturnsCreatedAsset(InvocationContext context)
    {
        // Arrange
        var actions = new LookupTableActions(context, FileManager);
        var input = new UploadLookupTableRequest
        {
            Content = new FileReference { Name = "test.html" },
            ListName = "lookup table testing - Blackbird",
        };

        // Act
        var result = await actions.UploadLookupTable(input);
        
        // Assert
        PrintResult(result);
        Assert.IsNotNull(result);
    }
    
    [TestMethod, TargetConnections]
    public async Task UpdateLookupTable_IsSuccess(InvocationContext context)
    {
        // Arrange
        var actions = new LookupTableActions(context, FileManager);
        var tableIdentifier = new LookupTableIdentifier { AssetId = "525787423bdbfe2d6cf7eec6a24d018f56f7ce9d" };
        var input = new UpdateLookupTableRequest
        {
            Content = new FileReference { Name = "test.html" },
        };

        // Act
        await actions.UpdateLookupTable(tableIdentifier, input);
    }
}