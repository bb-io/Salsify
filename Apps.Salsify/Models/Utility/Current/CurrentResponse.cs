using Blackbird.Applications.Sdk.Common.Exceptions;
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
        return ValidateLocale(string.IsNullOrWhiteSpace(requested) ? DefaultLocaleId : requested);
    }

    public string ValidateLocale(string locale)
    {
        if (Locales.All(x => x.Id != locale))
            throw new PluginMisconfigurationException(
                $"Locale '{locale}' is not configured in this organization. " +
                $"Available: {string.Join(", ", Locales.Select(x => x.Id))}");

        return locale;
    }
}