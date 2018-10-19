using System;
using System.Collections;
using System.Collections.Generic;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Proxynator.DependencyInjection
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
                    return UnwrapProxies(list);
                case IEnumerable enumerable:
                    return UnwrapProxies(enumerable);
                default:
                    return UnwrapProxy(services);
            }
        }

        private static IEnumerable UnwrapProxies(
            IList list)
        {
            for (var i = 0; i < list.Count; ++i)
            {
                list[i] = UnwrapProxy(list[i]);
            }

            return list;
        }

        private static IEnumerable UnwrapProxies(
            IEnumerable enumerable)
        {
            var type = enumerable.GetType().GetGenericArguments()[0];

            var list = (IList) Activator.CreateInstance(typeof(List<>).MakeGenericType(type), 1);
            foreach (var item in enumerable)
            {
                list.Add(UnwrapProxy(item));
            }

            return list;
        }

        private static object UnwrapProxy(
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