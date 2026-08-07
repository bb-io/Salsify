using Apps.Salsify.Models.Entities.Product;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Salsify.Models.Responses.Product;

public class ProductResponse(ProductEntity entity)
{
    [Display("Product ID")]
    public string Id { get; set; } = entity.Id;

    [Display("Product created at")] 
    public DateTime CreatedAt { get; set; } = entity.CreatedAt;
    
    [Display("Product updated at")]
    public DateTime UpdatedAt { get; set; } = entity.UpdatedAt;
}