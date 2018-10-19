using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

namespace Service
{
    public interface IApiServiceEventSource : IServiceEventSourceInterface
    {
        void GetValueMethodInvoked();
    }
}
