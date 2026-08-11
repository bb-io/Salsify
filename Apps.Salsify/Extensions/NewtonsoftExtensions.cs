using Newtonsoft.Json.Linq;

namespace Apps.Salsify.Extensions;

public static class NewtonsoftExtensions
{
    public static List<T> GetSection<T>(this IEnumerable<JObject> sections, string name)
    {
        return sections
            .SelectMany(x => x[name] as JArray ?? [])
            .Select(x => x.ToObject<T>()!)
            .ToList();
    }

    public static IReadOnlyList<string> Flatten(this JToken token)
    {
        return token is JArray array ? array.Select(x => x.ToString()).ToList() : [token.ToString()];
    }
}