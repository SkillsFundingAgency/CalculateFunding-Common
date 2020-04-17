using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CalculateFunding.Common.ApiClient.Specifications.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum JobType
    {
        Undefined,
        CurrentState,
        Released,
        History,
        HistoryProfileValues,
        CurrentProfileValues,
        CurrentOrganisationGroupValues,
        HistoryOrganisationGroupValues,
        HistoryPublishedProviderEstate,
        CalcResult
    }
}