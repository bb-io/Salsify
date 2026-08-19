using Apps.Salsify.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Salsify.Models.Identifiers.Optional;

public class LocaleOptionalIdentifier
{
    [Display("Locale"), DataSource(typeof(LocaleDataHandler))]
    public string? Locale { get; set; }
}