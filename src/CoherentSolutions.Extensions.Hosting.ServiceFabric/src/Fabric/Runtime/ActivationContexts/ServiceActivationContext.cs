using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ActivationContexts
{
    public class ServiceActivationContext : IServiceActivationContext
    {
        public string ApplicationName { get; }

        public string ApplicationTypeName { get; }

        public string ActivationContextId { get; }

        public string CodePackageName { get; }

        public string CodePackageVersion { get; }

        public string LogDirectory { get; }

        public string TempDirectory { get; }

        public string WorkDirectory { get; }

        public ServiceActivationContext(
            string applicationName,
            string applicationTypeName,
            string activationContextId,
            string codePackageName,
            string codePackageVersion,
            string logDirectory,
            string tempDirectory,
            string workDirectory)
        {
            if (string.IsNullOrWhiteSpace(applicationName))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(applicationName));
            }

            if (string.IsNullOrWhiteSpace(applicationTypeName))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(applicationTypeName));
            }

            if (string.IsNullOrWhiteSpace(activationContextId))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(activationContextId));
            }

            if (string.IsNullOrWhiteSpace(codePackageName))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(codePackageName));
            }

            if (string.IsNullOrWhiteSpace(codePackageVersion))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(codePackageVersion));
            }

            if (string.IsNullOrWhiteSpace(logDirectory))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(logDirectory));
            }

            if (string.IsNullOrWhiteSpace(tempDirectory))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(tempDirectory));
            }

            if (string.IsNullOrWhiteSpace(workDirectory))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(workDirectory));
            }

            this.ApplicationName = applicationName;
            this.ApplicationTypeName = applicationTypeName;
            this.ActivationContextId = activationContextId;
            this.CodePackageName = codePackageName;
            this.CodePackageVersion = codePackageVersion;
            this.LogDirectory = logDirectory;
            this.TempDirectory = tempDirectory;
            this.WorkDirectory = workDirectory;
        }
    }
}