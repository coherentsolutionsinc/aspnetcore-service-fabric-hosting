using System;
using System.ComponentModel;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Web
{
    [Description("ASP.NET Core self-hosted implementation.")]
    public class WebHostDescriptor : IHostDescriptor
    {
        public IHostKeywords Keywords { get; }

        public IHostRunner Runner { get; }

        public WebHostDescriptor(
            IWebHostKeywords keywords,
            IWebHostRunner runner)
        {
            this.Keywords = keywords
             ?? throw new ArgumentNullException(nameof(keywords));

            this.Runner = runner
             ?? throw new ArgumentNullException(nameof(runner));
        }
    }
}