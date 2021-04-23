using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Users.Models
{
    public class UserIndex
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("userName")]
        public string Username { get; set; }
    }
}
