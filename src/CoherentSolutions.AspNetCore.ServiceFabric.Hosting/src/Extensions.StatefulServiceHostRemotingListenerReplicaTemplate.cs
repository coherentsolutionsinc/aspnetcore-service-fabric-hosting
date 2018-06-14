using System;

using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric;

using Microsoft.ServiceFabric.Services.Remoting.V2;

using IService = Microsoft.ServiceFabric.Services.Remoting.IService;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting
{
    public static partial class Extensions
    {
        public static IStatefulServiceHostRemotingListenerReplicaTemplate UseImplementation<TRemotingImplementation>(
            this IStatefulServiceHostRemotingListenerReplicaTemplate @this)
            where TRemotingImplementation : IService
        {
            @this.ConfigureObject(
                configurator => configurator.UseImplementation<TRemotingImplementation>(null));

            return @this;
        }

        public static IStatefulServiceHostRemotingListenerReplicaTemplate UseImplementation<TRemotingImplementation>(
            this IStatefulServiceHostRemotingListenerReplicaTemplate @this,
            Func<TRemotingImplementation> factoryFunc)
            where TRemotingImplementation : IService
        {
            @this.ConfigureObject(
                configurator => configurator.UseImplementation(factoryFunc));

            return @this;
        }

        public static IStatefulServiceHostRemotingListenerReplicaTemplate UseSerializer<TSerializer>(
            this IStatefulServiceHostRemotingListenerReplicaTemplate @this)
            where TSerializer : IServiceRemotingMessageSerializationProvider
        {
            @this.ConfigureObject(
                configurator => configurator.UseSerializer<TSerializer>(null));

            return @this;
        }

        public static IStatefulServiceHostRemotingListenerReplicaTemplate UseSerializer<TSerializer>(
            this IStatefulServiceHostRemotingListenerReplicaTemplate @this,
            Func<TSerializer> factoryFunc)
            where TSerializer : IServiceRemotingMessageSerializationProvider
        {
            @this.ConfigureObject(
                configurator => configurator.UseSerializer(factoryFunc));

            return @this;
        }

        public static IStatefulServiceHostRemotingListenerReplicaTemplate Configure(
            this IStatefulServiceHostRemotingListenerReplicaTemplate @this,
            Action<IStatefulServiceHostRemotingListenerReplicaTemplateConfigurator> configAction)
        {
            @this.ConfigureObject(configAction);

            return @this;
        }
    }
}