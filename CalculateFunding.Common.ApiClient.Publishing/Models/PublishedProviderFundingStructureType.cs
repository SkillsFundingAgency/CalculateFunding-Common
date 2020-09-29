using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace CalculateFunding.Common.ApiClient.Publishing.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PublishedProviderFundingStructureType
    {
        FundingLine,
        Calculation
    }
}
