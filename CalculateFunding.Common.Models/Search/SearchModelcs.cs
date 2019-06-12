using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace CalculateFunding.Common.Models.Search
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SearchMode
    {
        Any = 0,
        All = 1
    }
}
