using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CalculateFunding.Common.ApiClient.Policies
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FundingStructureType
    {
        FundingLine,
        Calculation
    }
}