using System;
using System.Runtime.Serialization;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Common.Exceptions
{
    public class FactoryProducesNullInstanceException<T> : Exception
    {
        public FactoryProducesNullInstanceException()
            : this(null)
        {
        }

        public FactoryProducesNullInstanceException(
            Exception innerException)
            : base($"The instance of {nameof(T)} produced by factory cannot be null", innerException)
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