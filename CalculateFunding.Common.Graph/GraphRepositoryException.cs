using System;
using System.Runtime.Serialization;

namespace CalculateFunding.Common.Graph
{
    [Serializable]
    public class GraphRepositoryException : Exception
    {
        public GraphRepositoryException()
        {
        }

        protected GraphRepositoryException(SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }

        public GraphRepositoryException(string message) : base(message)
        {
        }

        public GraphRepositoryException(string message,
            Exception innerException) : base(message, innerException)
        {
        }
    }
}