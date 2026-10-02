namespace Apps.Salsify.Events.Polling.Models.Memory;

public class ProductIdsMemory
{
    public ProductIdsMemory() { }
    
    public ProductIdsMemory(string[] productIds)
    {
        ProductIds = productIds;
    }
    
    public string[] ProductIds { get; set; }
}