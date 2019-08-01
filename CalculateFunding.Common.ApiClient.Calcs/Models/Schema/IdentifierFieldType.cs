using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CalculateFunding.Common.ApiClient.Calcs.Models.Schema
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