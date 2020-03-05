using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CalculateFunding.Common.Testing
{
    public static class JsonExtensions
    {
        public static string AsJson<TPoco>(this TPoco dto, JsonSerializerSettings settings = null)
            where TPoco : class
        {
            return dto == null ? null : JsonConvert.SerializeObject(dto, GetOrCreateSettings(settings));
        }

        public static TPoco AsPoco<TPoco>(this string json, JsonSerializerSettings settings = null)
            where TPoco : class
        {
            return string.IsNullOrWhiteSpace(json) ? null : JsonConvert.DeserializeObject<TPoco>(json, GetOrCreateSettings(settings));
        }

        private static JsonSerializerSettings GetOrCreateSettings(JsonSerializerSettings settings)
        {
            return settings ?? new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }
    }
}