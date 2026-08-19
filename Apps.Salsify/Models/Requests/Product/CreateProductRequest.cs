using Blackbird.Applications.Sdk.Common;

namespace Apps.Salsify.Models.Requests.Product;

public class CreateProductRequest
{
    [Display("Product ID")] 
    public string Id { get; set; } = string.Empty;

    [Display("Product name")]
    public string? Name { get; set; }
}