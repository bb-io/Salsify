using RestSharp;

namespace Apps.Salsify.Extensions;

public static class RestRequestExtensions
{
    public static RestRequest AddQueryParameterIfNotEmpty(this RestRequest request, string paramName, string? paramValue)
    {
        if (string.IsNullOrWhiteSpace(paramValue) || string.IsNullOrWhiteSpace(paramValue))
            return request;

        request.AddQueryParameter(paramName, paramValue);
        return request;
    }
}