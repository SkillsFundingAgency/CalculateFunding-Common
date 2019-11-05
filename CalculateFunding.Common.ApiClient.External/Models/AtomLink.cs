using System;

namespace CalculateFunding.Common.ApiClient.External.Models
{
    [Serializable]
    public class AtomLink
    {
        public string Href { get; set; }

        public string Rel { get; set; }
    }
}