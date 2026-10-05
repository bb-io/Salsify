using Apps.Salsify.Api;
using Apps.Salsify.Constants.GraphQl;
using Apps.Salsify.Models.Identifiers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Salsify.Actions;

[ActionList("Lists")]
public class ListActions(InvocationContext invocationContext) : SalsifyInvocable(invocationContext)
{
    [Action("Add product to list", Description = "Add a product to a specific list")]
    public Task AddProductToList(
        [ActionParameter] ProductIdentifier productIdentifier,
        [ActionParameter] ProductListIdentifier productListIdentifier)
    {
        var request = new GraphQlRequest("AddProductToListMutation", GraphQlMutations.AddProductToList, new
        {
            input = new
            {
                organizationId = OrgId,
                listId = productListIdentifier.ProductListId,
                recordId = productIdentifier.ProductId
            }
        });

        return GraphQlClient.Execute(request);
    }
}