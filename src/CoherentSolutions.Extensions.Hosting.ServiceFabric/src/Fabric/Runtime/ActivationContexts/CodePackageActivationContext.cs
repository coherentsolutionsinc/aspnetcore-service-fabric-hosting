using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Fabric;
using System.Fabric.Description;
using System.Fabric.Health;
using System.Linq;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ActivationContexts
{
    public class CodePackageActivationContext : ICodePackageActivationContext
    {
        private readonly ApplicationPrincipalsDescription applicationPrincipalsDescription;

        private readonly string serviceManifestName;

        private readonly string serviceManifestVersion;

        private readonly CodePackageCollection codePackages;

        private readonly ConfigurationPackageCollection configurationPackages;

        private readonly DataPackageCollection dataPackages;

        private readonly ServiceTypeDescriptionCollection serviceTypeDescriptions;

        private readonly EndpointResourceDescriptionCollection endpointResourceDescriptions;

        public string ApplicationName { get; }

        public string ApplicationTypeName { get; }

        public string ContextId { get; }

        public string WorkDirectory { get; }

        public string LogDirectory { get; }

        public string TempDirectory { get; }

        public string CodePackageName { get; }

        public string CodePackageVersion { get; }

        public CodePackageActivationContext(
            string applicationName,
            string applicationTypeName,
            string contextId,
            string logDirectory,
            string tempDirectory,
            string workDirectory,
            string codePackageName,
            string codePackageVersion,
            string serviceManifestName,
            string serviceManifestVersion,
            ApplicationPrincipalsDescription applicationPrincipalsDescription,
            IEnumerable<CodePackage> codePackages,
            IEnumerable<ConfigurationPackage> configurationPackages,
            IEnumerable<DataPackage> dataPackages,
            IEnumerable<ServiceTypeDescription> serviceTypeDescriptions,
            IEnumerable<EndpointResourceDescription> endpointResourceDescriptions)
        {
            if (configurationPackages is null)
            {
                throw new ArgumentNullException(nameof(configurationPackages));
            }

            if (dataPackages is null)
            {
                throw new ArgumentNullException(nameof(dataPackages));
            }

            if (serviceTypeDescriptions is null)
            {
                throw new ArgumentNullException(nameof(serviceTypeDescriptions));
            }

            if (endpointResourceDescriptions is null)
            {
                throw new ArgumentNullException(nameof(endpointResourceDescriptions));
            }

            if (string.IsNullOrWhiteSpace(applicationName))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(applicationName));
            }

            if (string.IsNullOrWhiteSpace(applicationTypeName))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(applicationTypeName));
            }

            if (string.IsNullOrWhiteSpace(contextId))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(contextId));
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

            if (string.IsNullOrWhiteSpace(codePackageName))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(codePackageName));
            }

            if (string.IsNullOrWhiteSpace(codePackageVersion))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(codePackageVersion));
            }

            if (string.IsNullOrWhiteSpace(serviceManifestName))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(serviceManifestName));
            }

            if (string.IsNullOrWhiteSpace(serviceManifestVersion))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(serviceManifestVersion));
            }

            this.ApplicationName = applicationName;
            this.ApplicationTypeName = applicationTypeName;
            this.ContextId = contextId;
            this.LogDirectory = logDirectory;
            this.TempDirectory = tempDirectory;
            this.WorkDirectory = workDirectory;
            this.CodePackageName = codePackageName;
            this.CodePackageVersion = codePackageVersion;

            this.serviceManifestName = serviceManifestName;
            this.serviceManifestVersion = serviceManifestVersion;
            this.applicationPrincipalsDescription = applicationPrincipalsDescription ?? throw new ArgumentNullException(nameof(applicationPrincipalsDescription));

            this.codePackages = new CodePackageCollection(codePackages);
            this.configurationPackages = new ConfigurationPackageCollection(configurationPackages);
            this.dataPackages = new DataPackageCollection(dataPackages);
            this.serviceTypeDescriptions = new ServiceTypeDescriptionCollection(serviceTypeDescriptions);
            this.endpointResourceDescriptions = new EndpointResourceDescriptionCollection(endpointResourceDescriptions);
        }

        public event EventHandler<PackageAddedEventArgs<CodePackage>> CodePackageAddedEvent;

        public event EventHandler<PackageRemovedEventArgs<CodePackage>> CodePackageRemovedEvent;

        public event EventHandler<PackageModifiedEventArgs<CodePackage>> CodePackageModifiedEvent;

        public event EventHandler<PackageAddedEventArgs<ConfigurationPackage>> ConfigurationPackageAddedEvent;

        public event EventHandler<PackageRemovedEventArgs<ConfigurationPackage>> ConfigurationPackageRemovedEvent;

        public event EventHandler<PackageModifiedEventArgs<ConfigurationPackage>> ConfigurationPackageModifiedEvent;

        public event EventHandler<PackageAddedEventArgs<DataPackage>> DataPackageAddedEvent;

        public event EventHandler<PackageRemovedEventArgs<DataPackage>> DataPackageRemovedEvent;

        public event EventHandler<PackageModifiedEventArgs<DataPackage>> DataPackageModifiedEvent;

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public ApplicationPrincipalsDescription GetApplicationPrincipals()
        {
            return this.applicationPrincipalsDescription;
        }

        public IList<string> GetCodePackageNames()
        {
            return this.codePackages.Select(package => package.Description.Name).ToArray();
        }

        public CodePackage GetCodePackageObject(
            string packageName)
        {
            return this.codePackages[packageName];
        }

        public IList<string> GetConfigurationPackageNames()
        {
            return this.configurationPackages.Select(package => package.Description.Name).ToArray();
        }

        public ConfigurationPackage GetConfigurationPackageObject(
            string packageName)
        {
            return this.configurationPackages[packageName];
        }

        public IList<string> GetDataPackageNames()
        {
            return this.dataPackages.Select(package => package.Description.Name).ToArray();
        }

        public DataPackage GetDataPackageObject(
            string packageName)
        {
            return this.dataPackages[packageName];
        }

        public string GetServiceManifestName()
        {
            return this.serviceManifestName;
        }

        public string GetServiceManifestVersion()
        {
            return this.serviceManifestVersion;
        }

        public KeyedCollection<string, ServiceTypeDescription> GetServiceTypes()
        {
            return this.serviceTypeDescriptions;
        }

        public KeyedCollection<string, ServiceGroupTypeDescription> GetServiceGroupTypes()
        {
            return null;
        }

        public EndpointResourceDescription GetEndpoint(
            string endpointName)
        {
            return this.endpointResourceDescriptions[endpointName];
        }

        public KeyedCollection<string, EndpointResourceDescription> GetEndpoints()
        {
            return this.endpointResourceDescriptions;
        }

        public void ReportApplicationHealth(
            HealthInformation healthInfo)
        {
        }

        public void ReportApplicationHealth(
            HealthInformation healthInfo,
            HealthReportSendOptions sendOptions)
        {
        }

        public void ReportDeployedApplicationHealth(
            HealthInformation healthInfo)
        {
        }

        public void ReportDeployedApplicationHealth(
            HealthInformation healthInfo,
            HealthReportSendOptions sendOptions)
        {
        }

        public void ReportDeployedServicePackageHealth(
            HealthInformation healthInfo)
        {
        }

        public void ReportDeployedServicePackageHealth(
            HealthInformation healthInfo,
            HealthReportSendOptions sendOptions)
        {
        }
    }
}