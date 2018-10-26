using Newtonsoft.Json.Linq;

namespace CalculateFunding.Common.Identity.Authentication
{
    public class AzureAuthenticationResponse
    {
        public JObject AuthMeResponse { get; set; }

        public JArray GroupMemberships { get; set; }

        public string GraphAccessToken { get; set; }
    }
}
