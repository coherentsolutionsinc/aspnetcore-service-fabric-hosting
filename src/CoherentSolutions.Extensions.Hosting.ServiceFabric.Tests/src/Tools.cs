using System;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests
{
    public static class Tools
    {
        public static Func<IServiceCollection> GetDependenciesFunc()
        {
            return () => new ServiceCollection();
        }

        public static Func<IWebHostBuilder> GetWebHostBuilderFunc()
        {
            return () => WebHost.CreateDefaultBuilder();
        }

        public static Func<IServiceProvider, IService> GetRemotingImplementationFunc<T>()
            where T : IService
        {
            return provider => ActivatorUtilities.CreateInstance<T>(provider);
        }

        public static Func<IServiceProvider, IServiceRemotingMessageSerializationProvider> GetRemotingSerializerFunc<T>()
            where T : IServiceRemotingMessageSerializationProvider
        {
            return provider => ActivatorUtilities.CreateInstance<T>(provider);
        }

        public static Func<IServiceProvider, IServiceRemotingMessageHandler> GetRemotingHandlerFunc<T>()
            where T : IServiceRemotingMessageHandler
        {
            return provider => ActivatorUtilities.CreateInstance<T>(provider);
        }
    }
}