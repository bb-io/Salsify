using Apps.Salsify.Handlers.List;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Salsify.Models.Identifiers;

public class ProductListIdentifier
{
    [Display("Product list ID"), DataSource(typeof(ProductListIdDataHandler))]
    public string ProductListId { get; set; } = string.Empty;
}