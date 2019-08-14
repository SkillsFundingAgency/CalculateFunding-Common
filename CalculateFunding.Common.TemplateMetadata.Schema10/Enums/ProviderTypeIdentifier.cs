using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CalculateFunding.Common.TemplateMetadata.Schema10.Enums
{
    /// <summary>
    /// Valid list of the different unique ways to identifier an organisation.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ProviderTypeIdentifier
    {
        /// <summary>
        /// UK Provider Reference Number - the unique identifier allocated to providers by the UK Register of Learning Providers (UKRLP) - 8 digits.
        /// </summary>
        UKPRN,

        /// <summary>
        /// The code of the local education authority.
        /// </summary>
        LACode,

        /// <summary>
        ///  Unique provider identification number. A 6 digit number to represent a provider.
        /// </summary>
        UPIN,

        /// <summary>
        /// Unique Reference Number.
        /// </summary>
        URN,

        /// <summary>
        /// Unique Identifier (used for MATs).
        /// </summary>
        UID,

        /// <summary>
        /// The DfE number.
        /// </summary>
        DfeNumber,
    }
}