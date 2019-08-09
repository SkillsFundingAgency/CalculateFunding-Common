using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CalculateFunding.Common.ApiClient.Publishing.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SpecificationFundingStatus
    {
        AlreadyChosen,
        SharesAlreadyChoseFundingStream,
        CanChoose
    }
}
