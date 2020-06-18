using System.Collections.Generic;
using CalculateFunding.Common.Extensions;
using CalculateFunding.Common.Graph.Serializer.Converters;
using Newtonsoft.Json;

namespace CalculateFunding.Common.Graph.Serializer
{
    public static class ParameterSerializer
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
        };

        public static Dictionary<string, object> ToDictionary<T>(this T source)
            where T : class
            => source.AsJson(Settings)
                .AsPoco<Dictionary<string, object>>(NewCustomDictionaryConverter());

        public static IEnumerable<Dictionary<string, object>> ToDictionaries<T>(this IEnumerable<T> source) =>
            source.AsJson(Settings)
                .AsPoco<IEnumerable<Dictionary<string, object>>>(NewCustomDictionaryConverter());

        private static CustomDictionaryConverter NewCustomDictionaryConverter() => new CustomDictionaryConverter();
    }
}