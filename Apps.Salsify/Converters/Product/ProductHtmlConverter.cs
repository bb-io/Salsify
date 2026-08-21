using System.Net;
using Apps.Salsify.Constants;
using Apps.Salsify.Converters.Product.Models;
using Apps.Salsify.Models.Entities.Product;
using Apps.Salsify.Models.Entities.Properties;
using Blackbird.Applications.Sdk.Common.Exceptions;
using HtmlAgilityPack;

namespace Apps.Salsify.Converters.Product;

public static class ProductHtmlConverter
{
    public static HtmlDocument GenerateHtml(
        ProductEntity product,
        IReadOnlyDictionary<string, PropertyEntity> definitions,
        ProductHtmlOptions options)
    {
        var excluded = new HashSet<string>(options.ExcludeProperties, StringComparer.Ordinal);
        var onlyIncluded = new HashSet<string>(options.IncludeProperties, StringComparer.Ordinal);

        var doc = new HtmlDocument();
        doc.LoadHtml("<html><head><meta charset=\"utf-8\"></head><body></body></html>");
        var body = doc.DocumentNode.SelectSingleNode("//body")!;

        int emitted = 0;
        foreach (var (propertyId, _) in product.Values)
        {
            var definition = definitions.GetValueOrDefault(propertyId);
            
            if (onlyIncluded.Count != 0 && !onlyIncluded.Contains(propertyId))
                continue;
            
            if (excluded.Contains(propertyId) || !PropertyIsEligible(definition, options.IncludeNonLocalizable))
                continue;

            string lookupKey = definition!.Localizable ? options.Locale : string.Empty;
            var localizedValues = product.GetLocalizedValues(propertyId);

            if (!localizedValues.TryGetValue(lookupKey, out var values) || values.Count == 0)
            {
                if (!definition.Localizable) 
                    continue;

                if (!localizedValues.TryGetValue(options.DefaultLocale, out values) || values.Count == 0) 
                    continue;
            }

            for (int index = 0; index < values.Count; index++)
            {
                body.AppendChild(BuildValueNode(doc, propertyId, index, definition.DataType, values[index]));
                emitted++;
            }
        }

        if (emitted > 0) 
            return doc;

        string filterHint = options.ExcludeProperties.Count != 0 || options.IncludeProperties.Count != 0
            ? "Check the 'Include properties' and 'Exclude properties' inputs"
            : string.Empty;

        throw new PluginMisconfigurationException($"Product '{product.Id}' has no translatable content for {options.Locale}. {filterHint}");
    }

    private static bool PropertyIsEligible(PropertyEntity? definition, bool includeNonLocalizable)
    {
        if (definition is null) 
            return false;
        
        if (definition.Type == PropertyTypeConstants.ComputedPropertyType) 
            return false;

        return definition.Localizable || includeNonLocalizable;
    }

    private static HtmlNode BuildValueNode(HtmlDocument doc, string propertyId, int index, string dataType, string value)
    {
        var node = doc.CreateElement("div");
        node.SetAttributeValue(HtmlConstants.KeyAttribute, propertyId);
        node.SetAttributeValue(HtmlConstants.IndexAttribute, index.ToString());
        node.SetAttributeValue(HtmlConstants.TypeAttribute, dataType);

        node.InnerHtml = PropertyTypeConstants.MarkupTypes.Contains(dataType) ? value : WebUtility.HtmlEncode(value);
        return node;
    }
}