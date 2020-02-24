using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Publishing.Models
{
    public class ProfileTotal
    {
        [JsonProperty("year")]
        public int Year { get; set; }
        
        [JsonProperty("typeValue")]
        public string TypeValue { get; set; }
        
        [JsonProperty("occurrence")]
        public int Occurrence { get; set; }
        
        [JsonProperty("value")]
        public decimal Value { get; set; }
    }
}