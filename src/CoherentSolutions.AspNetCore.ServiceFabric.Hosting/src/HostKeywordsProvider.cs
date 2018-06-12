using System;
using System.Collections.Generic;

using Microsoft.Extensions.Configuration;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting
{
    public class HostKeywordsProvider : IHostKeywordsProvider
    {
        private const string ENV_FABRIC_APPLICATION_NAME = "Fabric_ApplicationName";

        private readonly IConfiguration configuration;

        public HostKeywordsProvider(
            IConfiguration configuration)
        {
            this.configuration = configuration
             ?? throw new ArgumentNullException(nameof(configuration));
        }

        public IEnumerable<string> GetKeywords()
        {
            if (!string.IsNullOrEmpty(this.configuration[ENV_FABRIC_APPLICATION_NAME]))
            {
                yield return HostKeywords.ENVIRONMENT_SERVICE_FABRIC;
            }

            yield return HostKeywords.ENVIRONMENT_ASPNET_CORE;
        }
    }
}