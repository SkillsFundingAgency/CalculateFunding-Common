﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CalculateFunding.Common.TemplateMetadata.Schema10.Enums
{
    /// <summary>
    /// The reason for the grouping
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum GroupingReason
    {
        /// <summary>
        /// Paid in this way.
        /// </summary>
        Payment,

        /// <summary>
        /// Informational only.
        /// </summary>
        Information,

        /// <summary>
        /// Contracting
        /// </summary>
        Contracting,
    }
}