using Apps.Salsify.Api;
using Apps.Salsify.Api.Utility;
using Apps.Salsify.Models.Utility.Current;
using RestSharp;

namespace Apps.Salsify.Helpers;

public static class ProductHelper
{
    // In Salsify, there's no simple 'product name' in the response body
    // Instead, an admin can select any property that will represent product name
    // The 'current' endpoint is undocumented and can be accessed from the UI
    public static async Task<string> GetProductNamePropertyName(SalsifyClient client)
    {
        var currentRequest = new SalsifyRequest("current", Method.Get, ApiVersion.Unversioned);
        var currentResponse = await client.ExecuteWithErrorHandling<CurrentResponse>(currentRequest);
        return currentResponse.RoleProperties.First(x => x.Role == "product_name").Id;
    }
}