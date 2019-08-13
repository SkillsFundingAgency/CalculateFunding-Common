using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CalculateFunding.Common.ApiClient.Calcs.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TemplateMappingEntityType
    {
        [EnumMember(Value = "Calculation")]
        Calculation,

        [EnumMember(Value = "ReferenceData")]
        ReferenceData,
    }
}
