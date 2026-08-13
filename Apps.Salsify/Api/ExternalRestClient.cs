using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using RestSharp;

namespace Apps.Salsify.Api;

public class ExternalRestClient() : BlackBirdRestClient(new())
{
    protected override Exception ConfigureErrorException(RestResponse response)
    {
        string errorMessage = $"{(int)response.StatusCode} {response.StatusDescription}: {response.Content ?? response.ErrorMessage}";
        return new PluginApplicationException(errorMessage);
    }
}