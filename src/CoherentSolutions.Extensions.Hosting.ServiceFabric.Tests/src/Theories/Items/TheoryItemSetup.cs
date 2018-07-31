using System;
using Xunit.Abstractions;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Items
{
    [Flags]
    public enum TheoryItemSetup
    {
        None,
        AsStatefulDelegate,
        AsStatelessDelegate,
        AsStatefulAspNetCoreListener,
        AsStatelessAspNetCoreListener,
        AsStatefulRemotingListener,
        AsStatelessRemotingListener
    }
}