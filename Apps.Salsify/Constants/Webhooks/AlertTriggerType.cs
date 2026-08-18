using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Apps.Salsify.Constants.Webhooks;

[JsonConverter(typeof(StringEnumConverter))]
public enum AlertTriggerType
{
    [EnumMember(Value = "add")] 
    Add,
    
    [EnumMember(Value = "change")] 
    Change,
}