﻿using System;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public interface IUseDelegateInvokerTheoryExtension : ITheoryExtension
    {
        Func<Delegate, IServiceProvider, IServiceHostDelegateInvoker> Factory { get; }
    }
}