using System;
using System.Collections.Generic;

namespace CalculateFunding.Common.ApiClient.External.Models
{
    [Serializable]
    public class AtomFeed<T> where T: class
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public AtomAuthor Author { get; set; }

        public DateTimeOffset Updated { get; set; }

        public string Rights { get; set; }

        public List<AtomLink> Link { get; set; }

        public string Archive { get; set; } = string.Empty;

        public List<AtomEntry<T>> AtomEntry { get; set; }
    }
}