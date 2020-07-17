using Newtonsoft.Json;

namespace CalculateFunding.Common.Identity.Authentication.Models
{
    public class UserClaim
    {
        [JsonProperty("typ")]
        public string Type { get; set; }
        [JsonProperty("val")]
        public string Value { get; set; }
    }
}
