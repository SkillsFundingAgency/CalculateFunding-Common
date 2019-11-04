using System;
using System.Collections.Generic;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CalculateFunding.Common.CosmosDb
{
    public static class CosmosDbConnectionString
    {

        private static readonly CosmosClientOptions ConnectionPolicy = new CosmosClientOptions
        {
            ConnectionMode = ConnectionMode.Direct,
            RequestTimeout = new TimeSpan(1, 0, 0),
            MaxRequestsPerTcpConnection = 300,
        };

        public static CosmosClient Parse(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("Connection string cannot be empty.");
            }

            if (ParseImpl(connectionString, out var ret, err => throw new FormatException(err)))
            {
                return ret;
            }

            throw new ArgumentException($"Connection string was not able to be parsed into a document client.");
        }

        public static bool TryParse(string connectionString, out CosmosClient cosmosClient)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                cosmosClient = null;
                return false;
            }

            try
            {
                return ParseImpl(connectionString, out cosmosClient, err => { });
            }
            catch (Exception)
            {
                cosmosClient = null;
                return false;
            }
        }

        private const string AccountEndpointKey = "AccountEndpoint";
        private const string AccountKeyKey = "AccountKey";
        private static readonly HashSet<string> RequireSettings = new HashSet<string>(new[] { AccountEndpointKey, AccountKeyKey }, StringComparer.OrdinalIgnoreCase);

        internal static bool ParseImpl(string connectionString, out CosmosClient cosmosClient, Action<string> error)
        {
            IDictionary<string, string> settings = ParseStringIntoSettings(connectionString, error);

            if (settings == null)
            {
                cosmosClient = null;
                return false;
            }

            if (!RequireSettings.IsSubsetOf(settings.Keys))
            {
                cosmosClient = null;
                return false;
            }

            var jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            cosmosClient = new CosmosClient(settings[AccountEndpointKey], settings[AccountKeyKey], ConnectionPolicy);
            return true;
        }

        /// <summary>
        /// Tokenizes input and stores name value pairs.
        /// </summary>
        /// <param name="connectionString">The string to parse.</param>
        /// <param name="error">Error reporting delegate.</param>
        /// <returns>Tokenized collection.</returns>
        private static IDictionary<string, string> ParseStringIntoSettings(string connectionString, Action<string> error)
        {
            IDictionary<string, string> settings = new Dictionary<string, string>();
            string[] splitted = connectionString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var nameValue in splitted)
            {
                var splittedNameValue = nameValue.Split(new[] { '=' }, 2);

                if (splittedNameValue.Length != 2)
                {
                    error("Settings must be of the form \"name=value\".");
                    return null;
                }

                if (settings.ContainsKey(splittedNameValue[0]))
                {
                    error($"Duplicate setting '{splittedNameValue[0]}' found.");
                    return null;
                }

                settings.Add(splittedNameValue[0], splittedNameValue[1]);
            }

            return settings;
        }
    }
}