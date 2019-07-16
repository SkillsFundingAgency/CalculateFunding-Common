using CalculateFunding.Common.Utility;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace CalculateFunding.Common.TemplateMetadata
{
    public class TemplateMetadataResolver : ITemplateMetadataResolver
    {
        private readonly ConcurrentDictionary<string, ITemplateMetadataGenerator> _supportedVersions;

        public TemplateMetadataResolver()
        {
            _supportedVersions = new ConcurrentDictionary<string, ITemplateMetadataGenerator>();
        }

        public bool Contains(string schemaVersion)
        {
            Guard.IsNullOrWhiteSpace(schemaVersion, nameof(schemaVersion));

            return _supportedVersions.ContainsKey(schemaVersion);
        }

        /// <summary>
        /// Get a resolver registered to the schema version
        /// </summary>
        /// <param name="schemaVersion">The schema version</param>
        /// <returns>A resolver regsitered for the schema value</returns>
        /// <exception cref="Exception">Thrown when no resolver registered for schema value</exception>
        public ITemplateMetadataGenerator GetService(string schemaVersion)
        {
            Guard.IsNullOrWhiteSpace(schemaVersion, nameof(schemaVersion));

            ITemplateMetadataGenerator templateMetadataGenerator;

            if (_supportedVersions.TryGetValue(schemaVersion, out templateMetadataGenerator))
            {
                return templateMetadataGenerator;
            }
            else
            {
                throw new Exception($"Unable to find a registered resolver for schema version : {schemaVersion}");
            }
        }

        public void Register(string schemaVersion, ITemplateMetadataGenerator templateMetadataGenerator)
        {
            Guard.IsNullOrWhiteSpace(schemaVersion, nameof(schemaVersion));
            Guard.ArgumentNotNull(templateMetadataGenerator, nameof(templateMetadataGenerator));

            _supportedVersions.TryAdd(schemaVersion, templateMetadataGenerator);
        }

        public bool TryGetService(string schemaVersion, out ITemplateMetadataGenerator templateMetadataGenerator)
        {
            Guard.IsNullOrWhiteSpace(schemaVersion, nameof(schemaVersion));

            return _supportedVersions.TryGetValue(schemaVersion, out templateMetadataGenerator);
        }
    }
}
