using System;
using System.Runtime.Serialization;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Exceptions
{
    public class FactoryProducesNullInstanceException<T> : Exception
    {
        public FactoryProducesNullInstanceException()
            : this(null)
        {
        }

        public FactoryProducesNullInstanceException(
            Exception innerException)
            : base($"The instance of {typeof(T).Name} produced by factory cannot be null", innerException)
        {
        }

        protected FactoryProducesNullInstanceException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}