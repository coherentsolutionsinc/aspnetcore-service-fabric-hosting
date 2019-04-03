namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class ServiceEventPayloadOnPackageAdded<TPackage>
        : IServiceEventPayloadOnPackageAdded<TPackage>
    {
        public TPackage Package { get; }

        public ServiceEventPayloadOnPackageAdded(
            TPackage package)
        {
            this.Package = package;
        }
    }
}