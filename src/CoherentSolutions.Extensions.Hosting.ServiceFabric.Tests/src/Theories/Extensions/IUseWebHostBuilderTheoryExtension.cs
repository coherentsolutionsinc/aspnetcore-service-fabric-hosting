using System;

using Microsoft.AspNetCore.Hosting;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public interface IUseWebHostBuilderTheoryExtension : ITheoryExtension
    {
        Func<IWebHostBuilder> Factory { get; }
    }
}