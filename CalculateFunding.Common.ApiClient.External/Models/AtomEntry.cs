using System;
using System.Xml.Serialization;

namespace CalculateFunding.Common.ApiClient.External.Models
{
    [Serializable]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class AtomEntry<T> where T: class
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Summary { get; set; }

        public DateTimeOffset? Published { get; set; }

        public DateTimeOffset Updated { get; set; }

        public string Version { get; set; }

        public AtomLink Link { get; set; }

        public AtomContent<object> Content { get; set; }

    }
}