namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class ServiceEventPayloadOnPackageRemoved<TPackage>
        : IServiceEventPayloadOnPackageRemoved<TPackage>
    {
        public TPackage Package { get; }

        public ServiceEventPayloadOnPackageRemoved(
            TPackage package)
        {
            this.Package = package;
        }
    }
}