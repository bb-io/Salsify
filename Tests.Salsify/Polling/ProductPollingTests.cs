using Apps.Salsify.Events.Polling;
using Apps.Salsify.Events.Polling.Models.Memory;
using Apps.Salsify.Events.Polling.Models.Request.Product;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using Tests.Salsify.Base;

namespace Tests.Salsify.Polling;

[TestClass]
public class ProductPollingTests : TestBaseMultipleConnections
{
    [TestMethod, TargetConnections]
    public async Task OnPropertyCreatedOrUpdated_ReturnsProperties(InvocationContext context)
    {
        // Arrange
        var polling = new ProductPollingList(context);
        var memory = new ProductIdsMemory { ProductIds = 
            [
                "s-ba0ea72f-ca1b-4512-8684-b7ceae5a8355",
                "s-7c6a07a7-d2a8-4d0c-bcdf-23a43d509393",
                "s-5a625490-4a5b-4fec-9105-dac1daebd0b0",
                "s-e80e18d7-81c4-40fd-8b3b-128c404a1702",
                "s-5683c996-20c5-40cd-a3f8-cce380204155",
                "s-5f14d03e-2c36-4563-8f00-560f44a0c986",
                "s-6cfa2d7a-be21-46e2-8167-b4f4fe4f8872",
                "s-95de32ec-736a-433d-a50e-2fda7f4d90d8",
                "s-c74df535-1d1e-406b-a7b1-5c3e229a5694",
                "s-0b70eaf9-ddda-437f-b178-42fd6ab709ff",
                "s-677e4af0-5ec7-4ee4-8b60-4cbd1650af2c",
                "s-7d44b141-3d3a-4d1c-b55f-30257270b096",
                "s-dbaf23b0-f446-44d1-9d18-d0c576d9f4d3",
                "s-5bd020e2-c9f4-4b7f-8be7-cb99befd84f6"
            ]
        };
        var request = new PollingEventRequest<ProductIdsMemory> { Memory = memory };
        var input = new OnProductAddedToListRequest { ProductListId = "1237442" };

        // Act
        var result = await polling.OnProductAddedToList(request, input);

        // Assert
        PrintResult(result);
        Assert.IsNotNull(result);
    }
}