using System;
using System.Fabric;
using System.Fabric.Description;
using System.IO;
using System.Reflection;
using System.Threading;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest.Objects;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest.Extensions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest.Objects.Factories;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest;
using System.Linq;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime
{
    public static class GhostServiceRuntime
    {
        private const string CODE_PACKAGE_NAME = "Code";

        private const string CODE_PACKAGE_VERSION = "1.0.0";

        private const string SERVICE_NAME = "ServiceName";

        private readonly static byte[] initializationData;

        static GhostServiceRuntime()
        {
            initializationData = new byte[0];
        }

        public static (StatelessServiceContext, IStatelessServicePartition) CreateStatelessServiceContextAndPartition(
            string serviceTypeName)
        {
            if (serviceTypeName is null)
            {
                throw new ArgumentNullException(nameof(serviceTypeName));
            }

            var servicePackage = LoadServicePackage();

            var serviceTypeDescriptor = servicePackage.Manifest.ServiceTypes.FirstOrDefault(i => i.ServiceTypeName == serviceTypeName);
            if (serviceTypeDescriptor is null)
            {
                throw new InvalidOperationException("");
            }
            if (serviceTypeDescriptor.Kind != ServiceTypeElementKind.Stateless)
            {
                throw new InvalidOperationException("");
            }

            var activeCodePackage = new CodePackageFactory()
                .Create(new CodePackageElement
                {
                    Manifest = servicePackage.Manifest,
                    Name = CODE_PACKAGE_NAME,
                    Version = CODE_PACKAGE_VERSION
                });

            var nodeContext = new GhostNodeContext();
            var activationContext = new GhostCodePackageActivationContext(
                servicePackage.Manifest.Name,
                servicePackage.Manifest.Version,
                activeCodePackage,
                new ApplicationPrincipalsDescription(),
                servicePackage.Manifest.ReadConfigurationPackages(),
                servicePackage.Manifest.ReadDataPackages(),
                servicePackage.Manifest.ReadServiceTypesDescriptions(),
                servicePackage.Manifest.ReadServiceEndpoints());

            var servicePartition = new GhostStatelessServiceSingletonPartition(Guid.NewGuid());
            var serviceContext = new StatelessServiceContext(
                nodeContext,
                activationContext,
                serviceTypeName,
                new Uri($"{activationContext.ApplicationName}/{SERVICE_NAME}"),
                initializationData,
                servicePartition.PartitionInfo.Id,
                1);

            return (serviceContext, servicePartition);
        }

        private static ServicePackage LoadServicePackage()
        {
            var location = Assembly.GetExecutingAssembly().Location;
            var current = location;

            var br = false;
            for (; !br;)
            {
                current = Path.GetDirectoryName(current);
                if (current is null)
                {
                    current = Path.GetPathRoot(location);
                    br = true;
                }

                var path = Path.Combine(current, "PackageRoot");
                if (ServicePackage.TryLoad(path, out var package))
                {
                    return package;
                }
            }

            throw new InvalidOperationException(
                string.Format(
                    "Could not locate 'PackageRoot' directory inside {0} -> {0}{1}..{1} directory tree.", 
                    current, 
                    Path.DirectorySeparatorChar));
        }
    }
}