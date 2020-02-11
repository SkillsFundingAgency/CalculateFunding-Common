using System.Collections.Generic;
using CalculateFunding.Common.Graph.Serializers.Converters;
using Newtonsoft.Json;

namespace CalculateFunding.Services.Graph.Serializer
{
    public static class ParameterSerializer
    {
        public static IList<Dictionary<string, object>> ToDictionary<TSourceType>(IList<TSourceType> source)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            string json = JsonConvert.SerializeObject(source, settings);

            return JsonConvert.DeserializeObject<IList<Dictionary<string, object>>>(json, new CustomDictionaryConverter());
        }
    }
}
