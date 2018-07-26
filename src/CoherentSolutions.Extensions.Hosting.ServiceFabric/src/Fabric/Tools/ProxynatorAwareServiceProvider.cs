using System;
using System.Collections;
using System.Collections.Generic;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Tools
{
    public class ProxynatorAwareServiceProvider : IServiceProvider
    {
        private readonly IServiceProvider impl;

        public ProxynatorAwareServiceProvider(
            IServiceProvider impl)
        {
            this.impl = impl ?? throw new ArgumentNullException(nameof(impl));
        }

        public object GetService(
            Type serviceType)
        {
            var services = this.impl.GetService(serviceType);
            switch (services)
            {
                case IList list:
                    return GetServices(list);
                case IEnumerable enumerable:
                    return GetServices(enumerable);
                default:
                    return GetService(services);
            }
        }

        private static IEnumerable GetServices(
            IList list)
        {
            for (var i = 0; i < list.Count; ++i)
            {
                list[i] = GetService(list[i]);
            }

            return list;
        }

        private static IEnumerable GetServices(
            IEnumerable enumerable)
        {
            var type = enumerable.GetType().GetGenericArguments()[0];

            var list = (IList) Activator.CreateInstance(typeof(List<>).MakeGenericType(type), 1);
            foreach (var item in enumerable)
            {
                list.Add(GetService(item));
            }

            return list;
        }

        private static object GetService(
            object service)
        {
            if (service is IProxynatorProxy proxy)
            {
                return proxy.Target;
            }

            return service;
        }
    }
}