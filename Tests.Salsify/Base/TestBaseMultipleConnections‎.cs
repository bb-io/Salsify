using Blackbird.Applications.Sdk.Common.Dynamic;
using Newtonsoft.Json;

namespace Tests.Salsify.Base;

public class TestBaseMultipleConnections : TestBase
{
    public new TestContext TestContext
    {
        get => base.TestContext!;
        set => base.TestContext = value;
    }

    protected void PrintResult(object result)
    {
        TestContext?.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
    }

    protected void PrintDataHandlerResult(IEnumerable<DataSourceItem> items)
    {
        TestContext.WriteLine($"Total: {items.Count()}");
        foreach (var item in items)
            TestContext?.WriteLine($"ID: {item.Value}, Display name: {item.DisplayName}");
    }
}