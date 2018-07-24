using System;

using Microsoft.AspNetCore.Hosting;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public interface IUseAspNetCoreListenerWebHostBuilderTheoryExtension
    {
        Func<IWebHostBuilder> Factory { get; }
    }
}