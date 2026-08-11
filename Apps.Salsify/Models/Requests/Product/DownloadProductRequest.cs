using Apps.Salsify.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Salsify.Models.Requests.Product;

public class DownloadProductRequest
{
    [Display("Locale"), DataSource(typeof(LocaleDataHandler))]
    public string? Locale { get; set; }

    [Display("Include only localizable properties", Description = "True by default")]
    public bool? OnlyLocalizableProperties { get; set; }

    [Display("Exclude properties"), DataSource(typeof(PropertyDataHandler))]
    public List<string>? ExcludeProperties { get; set; }
}