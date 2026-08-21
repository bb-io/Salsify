using Apps.Salsify.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Salsify.Models.Requests.Product;

public class DownloadProductRequest
{
    [Display("Include only localizable properties", Description = "True by default")]
    public bool? OnlyLocalizableProperties { get; set; }

    [Display("Exclude properties"), DataSource(typeof(PropertyDataHandler))]
    public List<string>? ExcludeProperties { get; set; }
    
    [Display("Include properties", Description = "Only include properties specified in this input")] 
    [DataSource(typeof(PropertyDataHandler))]
    public List<string>? IncludeProperties { get; set; }
}