using System;

namespace CalculateFunding.Common.ApiClient.External.Models
{
    [Serializable]
    public class AtomContent<T> where T: class
    {
        public T Allocation { get; set; }
    }
}