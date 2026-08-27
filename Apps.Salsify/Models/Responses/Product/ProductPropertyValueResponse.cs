using Apps.Salsify.Models.Entities.Product;
using Apps.Salsify.Models.Entities.Properties;
using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Apps.Salsify.Models.Responses.Product;

public class ProductPropertyValueResponse
{
    public ProductPropertyValueResponse(ProductEntity product, PropertyEntity property, string? locale)
    {
        if (!product.RawValues.TryGetValue(property.Id, out var rawValue) ||
            rawValue.Type is JTokenType.Null or JTokenType.Undefined)
        {
            return;
        }

        RawValue = rawValue.ToString(Formatting.None);

        JToken? selectedValue = rawValue;
        if (property.Localizable)
        {
            if (string.IsNullOrWhiteSpace(locale) ||
                rawValue is not JObject localizedValues ||
                !localizedValues.TryGetValue(locale, StringComparison.OrdinalIgnoreCase, out selectedValue))
            {
                return;
            }
        }

        if (selectedValue is null || selectedValue.Type is JTokenType.Null or JTokenType.Undefined)
            return;

        var values = Flatten(selectedValue).ToArray();
        Values = values;
        Value = values.FirstOrDefault();
        HasValue = values.Length > 0;
    }

    [Display("Property value")]
    public string? Value { get; set; }

    [Display("Property values")]
    public string[] Values { get; set; } = [];

    [Display("Has value")]
    public bool HasValue { get; set; }

    [Display("Raw value")]
    public string? RawValue { get; set; }

    private static IEnumerable<string> Flatten(JToken token)
    {
        IEnumerable<JToken> values = token is JArray array ? array : [token];
        return values.Select(FormatValue);
    }

    private static string FormatValue(JToken token)
    {
        return token is JValue ? token.ToString() : token.ToString(Formatting.None);
    }
}
