﻿using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Items.Base;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Items
{
    public sealed class StatefulServiceRemotingListenerTheoryItem
        : ServiceRemotingListenerTheoryItem<IStatefulServiceHostRemotingListenerReplicaTemplateConfigurator>
    {
        public StatefulServiceRemotingListenerTheoryItem()
            : base("StatefulService-RemotingListener")
        {
            this.SetupConfig(
                builder =>
                {
                    builder.DefineStatefulService(
                        serviceBuilder =>
                        {
                            serviceBuilder.ConfigureObject(
                                c =>
                                {
                                    c.UseRuntimeRegistrant(Tools.GetStatefulRuntimeRegistrantFunc());
                                    c.DefineRemotingListener(
                                        listenerBuilder =>
                                        {
                                            listenerBuilder.UseCommunicationListener(Tools.GetRemotingCommunicationListenerFunc());
                                            listenerBuilder.ConfigureObject(this.ConfigureExtensions);
                                        });
                                });
                        });
                });
        }
    }
}