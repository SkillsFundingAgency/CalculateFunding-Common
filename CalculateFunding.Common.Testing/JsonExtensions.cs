using Newtonsoft.Json;

namespace CalculateFunding.Common.Testing
{
    public static class JsonExtensions
    {
        public static string AsJson<TPoco>(this TPoco dto)
            where TPoco : class
        {
            return dto == null ? null : JsonConvert.SerializeObject(dto);
        }

        public static TPoco AsPoco<TPoco>(this string json)
            where TPoco : class
        {
            return string.IsNullOrWhiteSpace(json) ? null : JsonConvert.DeserializeObject<TPoco>(json);
        }
    }
}