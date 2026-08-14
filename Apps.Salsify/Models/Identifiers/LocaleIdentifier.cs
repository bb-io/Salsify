using Apps.Salsify.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Salsify.Models.Identifiers;

public class LocaleIdentifier
{
    [Display("Locale"), DataSource(typeof(LocaleDataHandler))]
    public string Locale { get; set; } = string.Empty;
}