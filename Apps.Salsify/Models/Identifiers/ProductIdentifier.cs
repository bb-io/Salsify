using Apps.Salsify.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Salsify.Models.Identifiers;

public class ProductIdentifier
{
    [Display("Product ID"), DataSource(typeof(ProductDataHandler))]
    public string ProductId { get; set; } = string.Empty;
}