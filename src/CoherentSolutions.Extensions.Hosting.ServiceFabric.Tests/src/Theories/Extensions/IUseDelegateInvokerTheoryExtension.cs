﻿using System;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public interface IUseDelegateInvokerTheoryExtension
    {
        Func<IServiceProvider, IServiceDelegateInvoker> Factory { get; }
    }
}