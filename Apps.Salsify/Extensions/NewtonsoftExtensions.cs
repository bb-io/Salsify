using Newtonsoft.Json.Linq;

namespace Apps.Salsify.Extensions;

public static class NewtonsoftExtensions
{
    public static IReadOnlyList<string> Flatten(this JToken token)
    {
        return token is JArray array ? array.Select(x => x.ToString()).ToList() : [token.ToString()];
    }
}