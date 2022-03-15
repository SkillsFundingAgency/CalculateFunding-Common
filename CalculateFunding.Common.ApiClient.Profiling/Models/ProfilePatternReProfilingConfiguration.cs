using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Profiling.Models;

public class ProfilePatternReProfilingConfiguration
{
    [JsonProperty("reProfilingEnabled")]
    public bool ReProfilingEnabled { get; set; }

    [JsonProperty("increasedAmountStrategyKey")]
    public string IncreasedAmountStrategyKey { get; set; }

    [JsonProperty("decreasedAmountStrategyKey")]
    public string DecreasedAmountStrategyKey { get; set; }

    [JsonProperty("sameAmountStrategyKey")]
    public string SameAmountStrategyKey { get; set; }

    [JsonProperty("initialFundingStrategyKey")]
    public string InitialFundingStrategyKey { get; set; }

    [JsonProperty("initialFundingStrategyWithCatchupKey")]
    public string InitialFundingStrategyWithCatchupKey { get; set; }

    [JsonProperty("initialClosureFundingStrategyKey")]
    public string InitialClosureFundingStrategyKey { get; set; }

    [JsonProperty("converterFundingStrategyKey")]
    public string ConverterFundingStrategyKey { get; set; }
}