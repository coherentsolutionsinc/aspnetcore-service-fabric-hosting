using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Items
{
    [Flags]
    public enum TheoryItemSetup
    {
        None,

        AsStatefulEventSource,

        AsStatelessEventSource,

        AsStatefulDelegate,

        AsStatelessDelegate,

        AsStatefulAspNetCoreListener,

        AsStatelessAspNetCoreListener,

        AsStatefulRemotingListener,

        AsStatelessRemotingListener
    }
}