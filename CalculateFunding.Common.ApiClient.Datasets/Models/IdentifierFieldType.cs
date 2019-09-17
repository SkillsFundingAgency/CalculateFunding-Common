using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CalculateFunding.Common.ApiClient.DataSets.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum IdentifierFieldType
    {
        None,
        UKPRN,
        UPIN,
        URN,
        EstablishmentNumber,
        LACode
    }
}