using Apps.Salsify.Api.Utility;
using RestSharp;

namespace Apps.Salsify.Api;

public class SalsifyRequest(
    string endpoint, 
    Method method = Method.Get,
    ApiVersion apiVersion = ApiVersion.V1)
    : RestRequest(endpoint.TrimStart('/'), method)
{
    public ApiVersion ApiVersion { get; } = apiVersion;
    public string? OverrideVerb { get; init; }
    internal bool Prepared { get; set; }
}