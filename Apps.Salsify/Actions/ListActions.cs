using Apps.Salsify.Api;
using Apps.Salsify.Constants;
using Apps.Salsify.Constants.GraphQl;
using Apps.Salsify.Models.Identifiers;
using Apps.Salsify.Models.Responses.List.Api;
using Apps.Salsify.Models.Utility.Wrapper;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Salsify.Actions;

[ActionList("Lists")]
public class ListActions(InvocationContext invocationContext) : SalsifyInvocable(invocationContext)
{
    // Undocumented endpoint. UI flow: Products -> Select product -> Actions -> Add to List
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

    // Undocumented endpoint, async operation. UI flow:
    // Products (dropdown) -> View all lists -> Select list -> Tick product -> Actions -> Remove Selected Products from List
    [Action("Remove product from list", Description = "Remove a product from a specific list")]
    public async Task RemoveProductFromList(
        [ActionParameter] ProductIdentifier productIdentifier,
        [ActionParameter] ProductListIdentifier productListIdentifier)
    {
        var startRemoveJobBody = new
        {
            data = new
            {
                action = "remove",
                list_id = productListIdentifier.ProductListId,
                member_ids = new[] { productIdentifier.ProductId },
                entity_type = "product"
            }
        };
        var startRemoveJobRequest = new SalsifyRequest("list_members_jobs", Method.Post).AddJsonBody(startRemoveJobBody);
        var startRemoveJobResponse = await Client.ExecuteWithErrorHandling<DataWrapper<ListMemberJobsResponse>>(startRemoveJobRequest);

        string jobId = startRemoveJobResponse.Data.ListMembersJobId;
        var statusRequest = new SalsifyRequest($"list_members_jobs/{jobId}");

        const int pollIntervalSeconds = 2;
        TimeSpan pollTimeout = TimeSpan.FromMinutes(18);
        var deadline = DateTime.UtcNow + pollTimeout;
        
        while (true)
        {
            var statusDataResponse = await Client.ExecuteWithErrorHandling<DataWrapper<GetMemberJobStatusResponse>>(statusRequest);
            var statusResponse = statusDataResponse.Data;
            
            if (string.Equals(statusResponse.Status, JobStatuses.Completed, StringComparison.OrdinalIgnoreCase))
                break;

            if (!string.Equals(statusResponse.Status, JobStatuses.Running, StringComparison.OrdinalIgnoreCase))
                throw new PluginApplicationException($"Unexpected job status from Salsify: '{statusResponse.Status}'");

            if (DateTime.UtcNow >= deadline)
            {
                throw new PluginApplicationException(
                    $"Salsify did not finish the job within {pollTimeout.TotalMinutes} minutes. " +
                    $"Last status: '{statusResponse.Status}'. Please try again later");
            }
                
            await Task.Delay(TimeSpan.FromSeconds(pollIntervalSeconds));
        }
    }
}