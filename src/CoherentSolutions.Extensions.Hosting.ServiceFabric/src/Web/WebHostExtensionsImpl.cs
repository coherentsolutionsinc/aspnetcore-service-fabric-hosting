using System;

using Microsoft.AspNetCore.Hosting;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Web
{
    public class WebHostExtensionsImpl : IWebHostExtensionsImpl
    {
        public void Run(
            IWebHost @this)
        {
            if (@this == null)
            {
                throw new ArgumentNullException(nameof(@this));
            }

            @this.Run();
        }
    }
}