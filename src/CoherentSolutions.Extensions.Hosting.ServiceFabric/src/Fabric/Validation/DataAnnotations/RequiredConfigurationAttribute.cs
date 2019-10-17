using System;
using System.ComponentModel.DataAnnotations;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Validation.DataAnnotations
{
    public sealed class RequiredConfigurationAttribute : RequiredAttribute
    {
        public RequiredConfigurationAttribute(
            string configuredBy)
        {
            if (string.IsNullOrWhiteSpace(configuredBy))
            {
                throw new ArgumentException("message", nameof(configuredBy));
            }

            this.ErrorMessage = $"{{0}} wasn't configured. Please use {configuredBy} to configure.";
        }
    }
}
