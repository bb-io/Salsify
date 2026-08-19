using Apps.Salsify.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Salsify.Models.Identifiers.Optional;

public class ProductOptionalIdentifier
{
    [Display("Product ID"), DataSource(typeof(ProductDataHandler))]
    public string? ProductId { get; set; }
}