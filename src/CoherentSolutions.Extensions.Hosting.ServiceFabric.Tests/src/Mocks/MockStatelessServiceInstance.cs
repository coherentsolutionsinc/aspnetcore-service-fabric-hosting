using System.Linq;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Tools;

using ServiceFabric.Mocks;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Mocks
{
    public class MockStatelessServiceInstance
    {
        private readonly StatelessService serviceInstance;

        public MockStatelessServiceInstance(
            StatelessService instance)
        {
            this.serviceInstance = instance;

            Injector.InjectProperty(this.serviceInstance, "Partition", new MockStatelessServicePartition(), true);
        }

        public async Task InitiateStartupSequenceAsync()
        {
            var communicationListeners = this.serviceInstance
               .InvokeCreateServiceInstanceListeners()
               .Select(l => l.CreateCommunicationListener(this.serviceInstance.Context))
               .ToArray();

            for (var i = 0; i < communicationListeners.Length; ++i)
            {
                await communicationListeners[i].OpenAsync(default);
            }

            var runAsyncTask = this.serviceInstance.InvokeRunAsync();
            var openAsyncTask = this.serviceInstance.InvokeOnOpenAsync();

            await Task.WhenAll(openAsyncTask, runAsyncTask);
        }

        public async Task InitiateShutdownSequenceAsync()
        {
            var communicationListeners = this.serviceInstance
               .InvokeCreateServiceInstanceListeners()
               .Select(l => l.CreateCommunicationListener(this.serviceInstance.Context))
               .ToArray();

            for (var i = 0; i < communicationListeners.Length; ++i)
            {
                await communicationListeners[i].CloseAsync(default);
            }

            await this.serviceInstance.InvokeOnCloseAsync();
        }
    }
}