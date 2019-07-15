using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CalculateFunding.Common.TemplateMetadata.Schema10.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ProviderCloseReason
    {
        [EnumMember(Value = "Not applicable")]
        NotApplicable,

        [EnumMember(Value = "Result of Amalgamation/Merger")]
        ResultOfAmalgamationMerger,

        [EnumMember(Value = "Close Nursery School")]
        CloseNurserySchool,

        [EnumMember(Value = "Academy Converter")]
        AcademyConverter,

        [EnumMember(Value = "Fresh Start")]
        FreshStart,

        [EnumMember(Value = "Closure")]
        Closure,

        [EnumMember(Value = "For Academy")]
        ForAcademy,

        [EnumMember(Value = "Academy Free School")]
        AcademyFreeSchool,

        [EnumMember(Value = "Change Religious Character")]
        ChangeReligiousCharacter,

        [EnumMember(Value = "Does not meet criteria for registration")]
        DoesNotMeetCriteriaForRegistration,

        [EnumMember(Value = "Change in status")]
        ChangeInStatus,

        [EnumMember(Value = "De-registered")]
        DeRegistered,

        [EnumMember(Value = "Transferred to new sponsor")]
        TransferredToNewSponsor,
    }
}
