namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ActivationContexts
{
    public interface IServiceActivationContextProvider
    {
        IServiceActivationContext GetActivationContext();
    }
}