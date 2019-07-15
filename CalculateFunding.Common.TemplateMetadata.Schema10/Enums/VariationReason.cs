using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CalculateFunding.Common.TemplateMetadata.Schema10.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum VariationReason
    {
        [EnumMember(Value = "Authority Field Updated")]
        AuthorityFieldUpdated,

        [EnumMember(Value = "Establishment Number Field Updated")]
        EstablishmentNumberFieldUpdated,

        [EnumMember(Value = "Dfe Establishment Number Field Updated")]
        DfeEstablishmentNumberFieldUpdated,

        [EnumMember(Value = "Number Field Updated")]
        NameFieldUpdated,

        [EnumMember(Value = "LA Code Field Updated")]
        LACodeFieldUpdated,

        [EnumMember(Value = "Legal Name Field Updated")]
        LegalNameFieldUpdated,

        [EnumMember(Value = "Trust Code Field Updated")]
        TrustCodeFieldUpdated,

        [EnumMember(Value = "Funding Updated")]
        FundingUpdated,

        [EnumMember(Value = "Profiling Updated")]
        ProfilingUpdated,
    }
}
