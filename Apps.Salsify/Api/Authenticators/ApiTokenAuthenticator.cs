using Apps.Salsify.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using RestSharp;
using RestSharp.Authenticators;

namespace Apps.Salsify.Api.Authenticators;

public class ApiTokenAuthenticator(IEnumerable<AuthenticationCredentialsProvider> creds) : IAuthenticator
{
    public ValueTask Authenticate(IRestClient client, RestRequest request)
    {
        string apiToken = creds.Get(CredsNames.ApiToken).Value;
        request.AddOrUpdateHeader("Authorization", $"Bearer {apiToken}");
        return ValueTask.CompletedTask;
    }
}