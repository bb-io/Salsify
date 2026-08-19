using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Apps.Salsify.Constants.Webhooks;

[JsonConverter(typeof(StringEnumConverter))]
public enum AlertEntityType
{
    [EnumMember(Value = "product")] 
    Product,
    
    [EnumMember(Value = "digital_asset")] 
    DigitalAsset
}