using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Fabric;
using System.Fabric.Description;
using System.Fabric.Health;
using System.IO;
using System.Linq;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ActivationContexts
{
    public class CodePackageActivationContext : ICodePackageActivationContext
    {
        private const string APPLICATION_NAME = "fabric:/ApplicationName";

        private const string APPLICATION_TYPE_NAME = "ApplicationTypeName";

        private const string CONTEXT_ID = "366B8CCC-8CC3-4EAA-8B90-938000A5EF52";

        private const string LOG_DIRECTORY = "Log";

        private const string WORK_DIRECTORY = "Work";

        private const string TEMP_DIRECTORY = "Temp";

        private readonly ApplicationPrincipalsDescription applicationPrincipalsDescription;

        private readonly string serviceManifestName;

        private readonly string serviceManifestVersion;

        private readonly CodePackage activeCodePackage;

        private readonly CodePackageCollection codePackages;

        private readonly ConfigurationPackageCollection configurationPackages;

        private readonly DataPackageCollection dataPackages;

        private readonly ServiceTypeDescriptionCollection serviceTypeDescriptions;

        private readonly EndpointResourceDescriptionCollection endpointResourceDescriptions;

        public string ApplicationName => APPLICATION_NAME;

        public string ApplicationTypeName => APPLICATION_TYPE_NAME;

        public string ContextId => CONTEXT_ID;

        public string WorkDirectory
        {
            get;
        }

        public string LogDirectory
        {
            get;
        }

        public string TempDirectory
        {
            get;
        }

        public string CodePackageName => this.activeCodePackage.Description.Name;

        public string CodePackageVersion => this.activeCodePackage.Description.Version;

        public CodePackageActivationContext(
            string serviceManifestName,
            string serviceManifestVersion,
            CodePackage activeCodePackage,
            ApplicationPrincipalsDescription applicationPrincipalsDescription,
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

            this.LogDirectory = Path.Combine(activeCodePackage.Path, LOG_DIRECTORY);
            this.WorkDirectory = Path.Combine(activeCodePackage.Path, WORK_DIRECTORY);
            this.TempDirectory = Path.Combine(activeCodePackage.Path, TEMP_DIRECTORY);

            this.serviceManifestName = serviceManifestName;
            this.serviceManifestVersion = serviceManifestVersion;
            this.applicationPrincipalsDescription =
                applicationPrincipalsDescription ?? throw new ArgumentNullException(nameof(applicationPrincipalsDescription));
            this.activeCodePackage = activeCodePackage ?? throw new ArgumentNullException(nameof(activeCodePackage));

            this.codePackages = new CodePackageCollection()
            {
                activeCodePackage
            };

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