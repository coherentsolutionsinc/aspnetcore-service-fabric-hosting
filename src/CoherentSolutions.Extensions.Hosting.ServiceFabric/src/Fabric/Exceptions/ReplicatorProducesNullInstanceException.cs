using System;
using System.Runtime.Serialization;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Exceptions
{
    public class ReplicatorProducesNullInstanceException<T> : Exception
    {
        public ReplicatorProducesNullInstanceException()
            : this(null)
        {
        }

        public ReplicatorProducesNullInstanceException(
            Exception innerException)
            : base($"The instance of {typeof(T).Name} produced by replicator cannot be null", innerException)
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