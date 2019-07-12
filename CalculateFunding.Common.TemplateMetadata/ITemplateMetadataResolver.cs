namespace CalculateFunding.Common.TemplateMetadata
{
    public interface ITemplateMetadataResolver
    {
        ITemplateMetadataGenerator GetService(string schemaVersion);

        bool TryGetService(string schemaVersion, out ITemplateMetadataGenerator templateMetadataGenerator);

        bool Contains(string schemaVersion);

        void Register(string schemaVersion, ITemplateMetadataGenerator templateMetadataGenerator);
    }
}
