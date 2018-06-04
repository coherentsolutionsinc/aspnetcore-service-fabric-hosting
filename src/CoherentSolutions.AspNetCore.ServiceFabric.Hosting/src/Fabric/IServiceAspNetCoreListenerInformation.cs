namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IServiceAspNetCoreListenerInformation : IServiceListenerInformation
    {
        string UrlSuffix { get; }
    }
}