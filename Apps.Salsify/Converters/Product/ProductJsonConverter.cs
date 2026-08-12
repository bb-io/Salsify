using Apps.Salsify.Constants;
using Apps.Salsify.Models.Entities.Properties;
using Blackbird.Applications.Sdk.Common.Exceptions;
using HtmlAgilityPack;

namespace Apps.Salsify.Converters.Product;

public static class ProductJsonConverter
{
    public static Dictionary<string, List<string>> ParseValues(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var byProperty = new Dictionary<string, SortedDictionary<int, string>>(StringComparer.Ordinal);
        var nodes = doc.DocumentNode.Descendants().Where(x => x.Attributes.Contains(HtmlConstants.KeyAttribute));
        
        foreach (var node in nodes)
        {
            var propertyId = node.GetAttributeValue(HtmlConstants.KeyAttribute, string.Empty);
            if (string.IsNullOrEmpty(propertyId))
                continue;

            var index = node.GetAttributeValue(HtmlConstants.IndexAttribute, 0);
            
            string dataType = node.GetAttributeValue(HtmlConstants.TypeAttribute, string.Empty);
            if (string.IsNullOrWhiteSpace(dataType))
                throw new PluginMisconfigurationException(
                    $"Value '{propertyId}' has no {HtmlConstants.TypeAttribute}. " + 
                    "The file may not have come from the 'Download product' action.");

            var value = PropertyTypeConstants.MarkupTypes.Contains(dataType)
                ? node.InnerHtml
                : HtmlEntity.DeEntitize(node.InnerText);

            if (!byProperty.TryGetValue(propertyId, out var byIndex))
                byProperty[propertyId] = byIndex = new SortedDictionary<int, string>();

            byIndex[index] = value ?? string.Empty;
        }

        return byProperty.ToDictionary(x => x.Key, x => x.Value.Values.ToList(), StringComparer.Ordinal);
    }

    public static Dictionary<string, object> BuildUpdateBody(
        IReadOnlyDictionary<string, List<string>> values,
        IReadOnlyDictionary<string, PropertyEntity> definitions,
        string locale)
    {
        var body = new Dictionary<string, object>(StringComparer.Ordinal);
        
        foreach (var (propertyId, rawValues) in values)
        {
            var definition = definitions.GetValueOrDefault(propertyId);
            var cleaned = rawValues.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

            if (definition is null || definition.Type == PropertyTypeConstants.ComputedPropertyType || cleaned.Length == 0)
                continue;

            body[propertyId] = definition.Localizable
                ? new Dictionary<string, string[]> { [locale] = cleaned }
                : cleaned;
        }

        return body;
    }
}