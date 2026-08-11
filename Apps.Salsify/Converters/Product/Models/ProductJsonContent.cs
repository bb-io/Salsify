using Apps.Salsify.Extensions;
using Apps.Salsify.Models.Entities.Product;
using Apps.Salsify.Models.Entities.Properties;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Apps.Salsify.Converters.Product.Models;

public record ProductJsonContent
{
    [JsonProperty("attributes")]
    public List<PropertyEntity> Attributes { get; set; }

    [JsonProperty("products")]
    public List<ProductEntity> Products { get; set; }

    public ProductJsonContent(string json)
    {
        var sections = JArray.Parse(json).OfType<JObject>().ToList();

        string? version = sections.FirstOrDefault(x => x["header"] is not null)?["header"]?["version"]?.ToString();
        if (version != "2")
            throw new PluginApplicationException($"Unsupported export format version '{version}'.");

        Attributes = sections.GetSection<PropertyEntity>("attributes");
        Products = sections.GetSection<ProductEntity>("products");
    }
}