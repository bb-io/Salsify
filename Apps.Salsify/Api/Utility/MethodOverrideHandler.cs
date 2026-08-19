namespace Apps.Salsify.Api.Utility;

public class MethodOverrideHandler(HttpMessageHandler inner) : DelegatingHandler(inner)
{
    public const string HeaderName = "X-Blackbird-Method-Override";

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!request.Headers.TryGetValues(HeaderName, out var values))
            return base.SendAsync(request, cancellationToken);
        
        request.Headers.Remove(HeaderName);
        request.Method = new HttpMethod(values.First());

        return base.SendAsync(request, cancellationToken);
    }
}