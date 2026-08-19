using Apps.Salsify.Api.Authenticators;
using Apps.Salsify.Api.Utility;
using Apps.Salsify.Constants;
using Apps.Salsify.Models.Utility.Current;
using Apps.Salsify.Models.Utility.Error;
using Apps.Salsify.Models.Utility.Pagination;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.Salsify.Api;

public class SalsifyClient(IEnumerable<AuthenticationCredentialsProvider> creds) : BlackBirdRestClient(new()
{
    BaseUrl = new Uri(ApiRoot),
    Authenticator = new ApiTokenAuthenticator(creds),
    ConfigureMessageHandler = inner => new MethodOverrideHandler(inner) // RestSharp does not support the REPORT HTTP method, so this exists
})
{
    private readonly string _orgId = creds.Get(CredsNames.OrgId).Value.Trim();
    private const string ApiRoot = "https://app.salsify.com/api";
    
    private CurrentResponse? _current;

    public async Task<List<TItem>> PaginateOffset<TResponse, TItem>(RestRequest request, int? paginateTimes = null)
        where TResponse : PaginatedResponse<TItem>
    {
        var all = new List<TItem>();

        int finalPaginateTimes = paginateTimes ?? 100;
        for (int page = 1; page <= finalPaginateTimes; page++)
        {
            request.AddOrUpdateParameter(new QueryParameter("page", page.ToString()));
            
            var response = await ExecuteWithErrorHandling<TResponse>(request);
            if (response.Items.Count == 0) 
                break;
            
            all.AddRange(response.Items);

            if (response.Meta is not { } meta) 
                break;
            
            if (all.Count >= meta.TotalEntries) 
                break;
        }

        return all;
    }
    
    public async Task<List<TItem>> PaginateCursor<TResponse, TItem>(RestRequest request, int? paginateTimes = null)
        where TResponse : PaginatedResponse<TItem>
    {
        var all = new List<TItem>();
        int limit = paginateTimes ?? 100;
        var seen = new HashSet<string>(StringComparer.Ordinal);
        
        request.AddOrUpdateParameter(new QueryParameter("per_page", "100"));
        string? cursor = null;

        for (var i = 0; i < limit; i++)
        {
            if (!string.IsNullOrEmpty(cursor))
                request.AddOrUpdateParameter(new QueryParameter("cursor", cursor));

            var response = await ExecuteWithErrorHandling<TResponse>(request);
            all.AddRange(response.Items);

            if (response.Meta is not { } meta)
                break;
            
            cursor = meta.Cursor;
            if (string.IsNullOrEmpty(cursor))
                break;

            // Not a user-facing error, that's why it's just Exception
            if (!seen.Add(cursor))
                throw new Exception($"Pagination stalled: cursor '{cursor}' was returned twice after {all.Count} items.");
        }

        return all;
    }

    public async Task<CurrentResponse> GetCurrentOrgInfo()
    {
        if (_current is not null)
            return _current;

        var request = new SalsifyRequest("current", Method.Get, ApiVersion.Unversioned);
        _current = await ExecuteWithErrorHandling<CurrentResponse>(request);
        return _current;
    }
    
    public override async Task<T> ExecuteWithErrorHandling<T>(RestRequest request)
    {
        PrepareRequest(request);
        return await base.ExecuteWithErrorHandling<T>(request);
    }
    
    public override Task<RestResponse> ExecuteWithErrorHandling(RestRequest request)
    {
        PrepareRequest(request);
        return base.ExecuteWithErrorHandling(request);
    }

    protected override Exception ConfigureErrorException(RestResponse response)
    {
        string statusCodePart = $"Status code {response.StatusCode} ({(int)response.StatusCode}).";
        if (string.IsNullOrWhiteSpace(response.Content))
            return new PluginApplicationException($"{statusCodePart} Server returned no content");
        
        var error = JsonConvert.DeserializeObject<ErrorResponse>(response.Content);
        string? singleError = error?.Error;
        string? multipleErrors = string.Join("; ", error?.Errors ?? []);
        
        if (!string.IsNullOrWhiteSpace(singleError))
            return new PluginApplicationException(singleError);
        if (!string.IsNullOrWhiteSpace(multipleErrors))
            return new PluginApplicationException(multipleErrors);
        return new PluginApplicationException($"{statusCodePart} Could not deserialize error. Raw: {response.Content}");
    }

    private void PrepareRequest(RestRequest request)
    {
        if (request is not SalsifyRequest salsify || salsify.Prepared) 
            return;

        string segment = salsify.ApiVersion switch
        {
            ApiVersion.V1 => "v1/",
            ApiVersion.Unversioned => string.Empty,
            _ => throw new PluginApplicationException(nameof(salsify.ApiVersion))
        };

        request.Resource = $"{segment}orgs/{_orgId}/{request.Resource.TrimStart('/')}";

        if (salsify.OverrideVerb is { } verb)
            request.AddOrUpdateHeader(MethodOverrideHandler.HeaderName, verb);

        salsify.Prepared = true;
    }
}