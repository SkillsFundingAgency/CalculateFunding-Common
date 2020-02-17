using System.Collections.Generic;
using CalculateFunding.Common.Graph.Serializers.Converters;
using Newtonsoft.Json;

namespace CalculateFunding.Services.Graph.Serializer
{
    public static class ParameterSerializer
    {
        public static IEnumerable<Dictionary<string, object>> ToDictionary<TSourceType>(IEnumerable<TSourceType> source)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            string json = JsonConvert.SerializeObject(source, settings);

            return JsonConvert.DeserializeObject<IEnumerable<Dictionary<string, object>>>(json, new CustomDictionaryConverter());
        }
    }
}
