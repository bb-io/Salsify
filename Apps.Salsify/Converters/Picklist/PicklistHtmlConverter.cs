using System.Net;
using Apps.Salsify.Constants;
using Apps.Salsify.Converters.Picklist.Models;
using Blackbird.Applications.Sdk.Common.Exceptions;
using HtmlAgilityPack;

namespace Apps.Salsify.Converters.Picklist;

public static class PicklistHtmlConverter
{
    public static HtmlDocument GenerateHtml(List<PicklistValue> picklistValues)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml("<html><head><meta charset=\"utf-8\"></head><body></body></html>");
        var body = doc.DocumentNode.SelectSingleNode("//body")!;

        int emitted = 0;
        foreach (var picklistValue in picklistValues.Where(picklistValue => !string.IsNullOrWhiteSpace(picklistValue.Text)))
        {
            body.AppendChild(BuildValueNode(doc, picklistValue));
            emitted++;
        }

        return emitted == 0
            ? throw new PluginMisconfigurationException("The picklist has no values with translatable labels")
            : doc;
    }

    private static HtmlNode BuildValueNode(HtmlDocument doc, PicklistValue picklistValue)
    {
        var node = doc.CreateElement("div");
        node.SetAttributeValue(HtmlConstants.KeyAttribute, picklistValue.Id);
        node.InnerHtml = WebUtility.HtmlEncode(picklistValue.Text);

        return node;
    }
}