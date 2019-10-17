using System;
using System.Collections.Generic;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceDelegateInvocationContextRegistrant
    {
        IEnumerable<(Type t, object o)> GetInvocationContextRegistrations(
            IServiceDelegateInvocationContext invocationContext);
    }
}