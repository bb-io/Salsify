using Newtonsoft.Json;

namespace Apps.Salsify.Models.Utility.Current;

public class LocaleResponse
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("language_name")]
    public string LanguageName { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"{LanguageName} ({Id})";
    }
}