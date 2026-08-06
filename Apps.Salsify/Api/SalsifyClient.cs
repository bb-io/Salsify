using Apps.Salsify.Authenticators;
using Apps.Salsify.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.Salsify.Api;

public class SalsifyClient(IEnumerable<AuthenticationCredentialsProvider> creds) : BlackBirdRestClient(new()
{
    BaseUrl = new Uri(GetBaseUrl(creds)),
    Authenticator = new ApiTokenAuthenticator(creds)
})
{
    private readonly string _orgId = creds.Get(CredsNames.OrgId).Value.Trim();
    private const string ApiRoot = "https://app.salsify.com/api";
    private const string DefaultApiVersion = "v1";

    private static string GetBaseUrl(IEnumerable<AuthenticationCredentialsProvider> creds)
    {
        return $"{ApiRoot}/{DefaultApiVersion}/orgs/{creds.Get(CredsNames.OrgId).Value.Trim()}";
    }

    public override Task<RestResponse> ExecuteWithErrorHandling(RestRequest request)
    {
        if (request is SalsifyRequest { ApiVersion: { } version } && 
            version != DefaultApiVersion && 
            !request.Resource.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            request.Resource = $"{ApiRoot}/{version}/orgs/{_orgId}/{request.Resource.TrimStart('/')}";
        }

        return base.ExecuteWithErrorHandling(request);
    }

    protected override Exception ConfigureErrorException(RestResponse response)
    {
        var error = JsonConvert.DeserializeObject(response.Content);
        var errorMessage = "";

        throw new PluginApplicationException(errorMessage);
    }
}