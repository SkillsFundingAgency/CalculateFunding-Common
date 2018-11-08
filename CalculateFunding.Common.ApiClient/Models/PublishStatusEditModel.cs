
using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Models
{
    public class PublishStatusEditModel
    {
        [JsonProperty("publishStatus")]
        public PublishStatus PublishStatus { get; set; }
    }
}
