﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace CalculateFunding.Common.Models
{
    [DataContract]
    public class DocumentEntity<T> : IIdentifiable where T : IIdentifiable
    {
        public DocumentEntity(T content = default(T))
        {
            DocumentType = typeof(T).Name;
            Content = content;
        }

        [JsonProperty("id")]
        [Key]
        public string Id => Content?.Id;

        [JsonProperty("documentType")]
        public string DocumentType { get; set; }

        [JsonProperty("content")]
        public T Content { get; set; }

        [JsonProperty("createdAt")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("updatedAt")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonProperty("deleted")]
        public bool Deleted { get; set; }
        
        [JsonProperty("_etag")]
        public string ETag { get; set; }
    }
}