using System;
using System.Runtime.Serialization;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Exceptions
{
    public class ReplicatorProducesNullInstanceException<T> : Exception
    {
        public ReplicatorProducesNullInstanceException()
            : this(null)
        {
        }

        public ReplicatorProducesNullInstanceException(
            Exception innerException)
            : base($"The instance of {nameof(T)} produced by replicator cannot be null", innerException)
        {
        }

        protected ReplicatorProducesNullInstanceException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}