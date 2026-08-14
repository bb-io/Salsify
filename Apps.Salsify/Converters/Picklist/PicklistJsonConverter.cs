using Apps.Salsify.Constants;
using HtmlAgilityPack;

namespace Apps.Salsify.Converters.Picklist;

public static class PicklistJsonConverter
{
    public static Dictionary<string, string> ParseValues(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var values = new Dictionary<string, string>(StringComparer.Ordinal);
        var nodes = doc.DocumentNode.Descendants().Where(x => x.Attributes.Contains(HtmlConstants.KeyAttribute));

        foreach (var node in nodes)
        {
            string valueId = node.GetAttributeValue(HtmlConstants.KeyAttribute, string.Empty);
            if (string.IsNullOrEmpty(valueId)) 
                continue;

            string text = HtmlEntity.DeEntitize(node.InnerText) ?? string.Empty;
            if (string.IsNullOrWhiteSpace(text)) 
                continue;

            values[valueId] = text;
        }

        return values;
    }
}