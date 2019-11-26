using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Extensions
{
    public static partial class ReflectionQuery
    {
        public static Root Query(
            this Type @this)
        {
            return new Root(@this);
        }
    }
}