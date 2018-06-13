using System;

using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric;

using Microsoft.ServiceFabric.Services.Remoting.V2;

using IService = Microsoft.ServiceFabric.Services.Remoting.IService;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting
{
    public static partial class Extensions
    {
        public static IStatelessServiceHostRemotingListenerReplicaTemplate UseImplementation<TRemotingImplementation>(
            this IStatelessServiceHostRemotingListenerReplicaTemplate @this)
            where TRemotingImplementation : IService
        {
            @this.ConfigureObject(
                configurator => configurator.UseImplementation<TRemotingImplementation>(null));

            return @this;
        }

        public static IStatelessServiceHostRemotingListenerReplicaTemplate UseImplementation<TRemotingImplementation>(
            this IStatelessServiceHostRemotingListenerReplicaTemplate @this,
            Func<TRemotingImplementation> factoryFunc)
            where TRemotingImplementation : IService
        {
            @this.ConfigureObject(
                configurator => configurator.UseImplementation(factoryFunc));

            return @this;
        }

        public static IStatelessServiceHostRemotingListenerReplicaTemplate UseSerializer<TSerializer>(
            this IStatelessServiceHostRemotingListenerReplicaTemplate @this)
            where TSerializer : IServiceRemotingMessageSerializationProvider
        {
            @this.ConfigureObject(
                configurator => configurator.UseSerializer<TSerializer>(null));

            return @this;
        }

        public static IStatelessServiceHostRemotingListenerReplicaTemplate UseSerializer<TSerializer>(
            this IStatelessServiceHostRemotingListenerReplicaTemplate @this,
            Func<TSerializer> factoryFunc)
            where TSerializer : IServiceRemotingMessageSerializationProvider
        {
            @this.ConfigureObject(
                configurator => configurator.UseSerializer(factoryFunc));

            return @this;
        }

        public static IStatelessServiceHostRemotingListenerReplicaTemplate Configure(
            this IStatelessServiceHostRemotingListenerReplicaTemplate @this,
            Action<IStatelessServiceHostRemotingListenerReplicaTemplateConfigurator> configAction)
        {
            @this.ConfigureObject(configAction);

            return @this;
        }
    }
}