namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IServiceHostAspNetCoreListenerInformation
    {
        string EndpointName { get; }

        string UrlSuffix { get; }
    }
}
