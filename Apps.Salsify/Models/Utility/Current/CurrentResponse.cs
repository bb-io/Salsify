using Newtonsoft.Json;

namespace Apps.Salsify.Models.Utility.Current;

public class CurrentResponse
{
    [JsonProperty("role_properties")]
    public List<RolePropertyResponse> RoleProperties { get; set; } = [];

    [JsonProperty("locales")]
    public List<LocaleResponse> Locales { get; set; } = [];

    [JsonProperty("default_locale_id")]
    public string DefaultLocaleId { get; set; } = string.Empty;

    public string? GetRolePropertyId(string role)
    {
        return RoleProperties.FirstOrDefault(x => x.Role == role)?.Id;
    }

    public string ResolveLocale(string? requested)
    {
        return string.IsNullOrWhiteSpace(requested) ? DefaultLocaleId : requested;
    }
}