using RestSharp;

namespace Apps.Salsify.Api;

public class SalsifyRequest(string endpoint, Method method = Method.Get, string? apiVersion = null)
    : RestRequest(endpoint.TrimStart('/'), method)
{
    public string? ApiVersion { get; } = apiVersion;
}