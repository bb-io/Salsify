using Apps.Salsify.Api.Authenticators;
using Apps.Salsify.Constants;
using Apps.Salsify.Models.Utility.GraphQl;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using RestSharp;

namespace Apps.Salsify.Api;

public class SalsifyGraphQlClient(IEnumerable<AuthenticationCredentialsProvider> creds) : BlackBirdRestClient(new()
{
    BaseUrl = new Uri("https://app.salsify.com"),
    Authenticator = new ApiTokenAuthenticator(creds)
})
{
    private readonly string _orgId = creds.Get(CredsNames.OrgId).Value.Trim();
    
    public async Task<List<TItem>> Paginate<TResponse, TItem>(GraphQlRequest request, int? paginateTimes = null)
        where TResponse : IGraphQlPaged<TItem>
    {
        var all = new List<TItem>();

        for (int page = 1; paginateTimes is null || page <= paginateTimes; page++)
        {
            var response = await Execute<TResponse>(request.ForPage(page, 100));
            var currentPage = response.Page;

            if (currentPage.Entries.Count == 0) 
                break;
            all.AddRange(currentPage.Entries);

            if (currentPage.PageMetadata.HasNext is false) 
                break;
            
            if (currentPage.PageMetadata.HasNext is null && all.Count >= currentPage.PageMetadata.TotalEntries) 
                break;
        }

        return all;
    }

    public async Task<T> Execute<T>(GraphQlRequest request)
    {
        var response = await Send<T>(request);
        return response.Data ?? throw new PluginApplicationException("Salsify returned no data");
    }
    
    public Task Execute(GraphQlRequest request)
    {
        return Send<object>(request);
    }
    
    protected override Exception ConfigureErrorException(RestResponse response)
    {
        string statusCodePart = $"Status code {response.StatusCode} ({(int)response.StatusCode}).";
        if (string.IsNullOrWhiteSpace(response.Content))
            return new PluginApplicationException($"{statusCodePart} Server returned no content");
        
        return new PluginApplicationException(statusCodePart);
    }
    
    private async Task<GraphQlResponse<T>> Send<T>(GraphQlRequest request)
    {
        if (request.DeclaresVariable(GraphQlRequest.OrganizationVariable))
            request = request.WithVariable(GraphQlRequest.OrganizationVariable, _orgId);

        var response = await base.ExecuteWithErrorHandling<GraphQlResponse<T>>(request);

        if (response.Errors.Count > 0)
            throw new PluginApplicationException(string.Join("; ", response.Errors.Select(x => x.Message)));

        return response;
    }
}