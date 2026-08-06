using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Salsify.Actions;

[ActionList("Actions")]
public class Actions(InvocationContext invocationContext) : SalsifyInvocable(invocationContext)
{
    [Action("Action", Description = "Describes the action")]
    public async Task Action()
    {
        
    }
}