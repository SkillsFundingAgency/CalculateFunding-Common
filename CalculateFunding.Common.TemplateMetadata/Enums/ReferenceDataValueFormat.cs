namespace CalculateFunding.Common.TemplateMetadata.Enums
{
    /// <summary>
    /// Valid list of the ways a number show be displayed
    /// </summary>
    public enum ReferenceDataValueFormat
    {
        /// <summary>
        /// A number (e.g. a pupil number). eg 5, the return value should be 5 and 2.7334 should return 2.7334. Values are a represented as a decimal
        /// </summary>
        Number,

        /// <summary>
        /// A percentage amount. eg for 25%, the return value should be 25
        /// </summary>
        Percentage,

        /// <summary>
        /// A currency. for example for £5.83, the return value should be 5.83. Values are a represented as a decimal
        /// </summary>
        Currency,
    }
}