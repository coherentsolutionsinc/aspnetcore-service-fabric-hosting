using System;

using Microsoft.AspNetCore.Hosting;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Web
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