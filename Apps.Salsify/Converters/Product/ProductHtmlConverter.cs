using System.Net;
using Apps.Salsify.Models.Entities.Product;
using Apps.Salsify.Models.Entities.Properties;
using Blackbird.Applications.Sdk.Common.Exceptions;
using HtmlAgilityPack;

namespace Apps.Salsify.Converters.Product;

public static class ProductHtmlConverter
{
    public const string KeyAttribute = "data-blackbird-key";
    public const string IndexAttribute = "data-index";
    public const string TypeAttribute = "data-type";
    
    private static readonly HashSet<string> MarkupDataTypes = new(StringComparer.OrdinalIgnoreCase) { "rich_text", "html" };

    public static HtmlDocument GenerateHtml(
        ProductEntity product,
        IReadOnlyDictionary<string, PropertyEntity> definitions,
        string locale,
        bool includeNonLocalizable = false,
        IReadOnlyCollection<string>? excludeProperties = null)
    {
        var excluded = new HashSet<string>(excludeProperties ?? [], StringComparer.Ordinal);

        var doc = new HtmlDocument();
        doc.LoadHtml("<html><head><meta charset=\"utf-8\"></head><body></body></html>");
        var body = doc.DocumentNode.SelectSingleNode("//body")!;

        int emitted = 0;
        foreach (var (propertyId, _) in product.Values)
        {
            var definition = definitions.GetValueOrDefault(propertyId);
            if (excluded.Contains(propertyId) || !PropertyIsEligible(definition, includeNonLocalizable))
                continue;

            string lookupKey = definition!.Localizable ? locale : string.Empty;
            if (!product.GetLocalizedValues(propertyId).TryGetValue(lookupKey, out var values))
                continue;

            for (int index = 0; index < values.Count; index++)
            {
                body.AppendChild(BuildValueNode(doc, propertyId, index, definition.DataType, values[index]));
                emitted++;
            }
        }

        return emitted == 0 
            ? throw new PluginMisconfigurationException($"Product '{product.Id}' has no translatable content for {locale}") 
            : doc;
    }

    private static bool PropertyIsEligible(PropertyEntity? definition, bool includeNonLocalizable)
    {
        if (definition is null) 
            return false;
        
        if (definition.Type == "computed_property") 
            return false;

        return definition.Localizable || includeNonLocalizable;
    }

    private static HtmlNode BuildValueNode(HtmlDocument doc, string propertyId, int index, string dataType, string value)
    {
        var node = doc.CreateElement("div");
        node.SetAttributeValue(KeyAttribute, propertyId);
        node.SetAttributeValue(IndexAttribute, index.ToString());
        node.SetAttributeValue(TypeAttribute, dataType);

        node.InnerHtml = MarkupDataTypes.Contains(dataType) ? value : WebUtility.HtmlEncode(value);
        return node;
    }
}