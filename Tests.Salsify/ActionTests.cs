using Apps.Salsify.Actions;
using Tests.Salsify.Base;

namespace Tests.Salsify;

[TestClass]
public class ActionTests : TestBase
{
    [TestMethod]
    public async Task Dynamic_handler_works()
    {
        var actions = new Actions(InvocationContext);

        await actions.Action();
    }
}
