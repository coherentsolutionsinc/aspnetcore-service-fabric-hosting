using System;
using System.Reflection;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests
{
    public static class Extensions
    {
        public static void UpstreamConfiguration<T>(
            this IConfigurableObject<T> @this,
            T configurator)
        {
            if (@this == null)
            {
                throw new ArgumentNullException(nameof(@this));
            }

            if (configurator == null)
            {
                throw new ArgumentNullException(nameof(configurator));
            }

            var mi = @this.GetType().GetMethod("UpstreamConfiguration", BindingFlags.Instance | BindingFlags.NonPublic);
            if (mi == null)
            {
                throw new InvalidOperationException("UpstreamConfiguration method doesn't exist");
            }

            mi.Invoke(
                @this,
                new object[]
                {
                    configurator
                });
        }
    }
}