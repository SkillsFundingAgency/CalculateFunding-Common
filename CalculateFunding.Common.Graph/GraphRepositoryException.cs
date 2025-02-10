using System;

namespace CalculateFunding.Common.Graph
{
    public class GraphRepositoryException : Exception
    {
        public GraphRepositoryException()
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