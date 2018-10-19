using System.Fabric;
using System.Linq;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Tools;

using Microsoft.ServiceFabric.Services.Runtime;

using ServiceFabric.Mocks;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Mocks
{
    internal class MockStatefulServiceReplica
    {
        private readonly StatefulServiceBase serviceReplica;

        public MockStatefulServiceReplica(
            StatefulServiceBase serviceReplica)
        {
            this.serviceReplica = serviceReplica;
            Injector.InjectProperty(this.serviceReplica, "Partition", new MockStatefulServicePartition(), true);

            if (this.serviceReplica is IStatefulService statefulService)
            {
                statefulService.GetEventSource(); // Provoke event source initialization
            }
        }

        public async Task InitiateStartupSequenceAsync()
        {
            await this.serviceReplica.InvokeOnOpenAsync();

            await this.PromoteSequenceAsync();
        }

        public async Task InitiateShutdownSequenceAsync()
        {
            await this.DemoteSequenceAsync();

            await this.serviceReplica.InvokeOnCloseAsync();
        }

        public async Task InitiatePromotionSequenceAsync()
        {
            await this.PromoteSequenceAsync();
        }

        public async Task InitiateDemotionSequenceAsync()
        {
            await this.DemoteSequenceAsync();
        }

        private async Task PromoteSequenceAsync()
        {
            var communicationListeners = this.serviceReplica
               .InvokeCreateServiceReplicaListeners()
               .Select(l => l.CreateCommunicationListener(this.serviceReplica.Context))
               .ToArray();

            for (var i = 0; i < communicationListeners.Length; ++i)
            {
                await communicationListeners[i].OpenAsync(default);
            }

            await this.serviceReplica.InvokeOnChangeRoleAsync(ReplicaRole.Primary);

            await this.serviceReplica.InvokeRunAsync();
        }

        private async Task DemoteSequenceAsync()
        {
            var communicationListeners = this.serviceReplica
               .InvokeCreateServiceReplicaListeners()
               .Select(l => l.CreateCommunicationListener(this.serviceReplica.Context))
               .ToArray();

            for (var i = 0; i < communicationListeners.Length; ++i)
            {
                await communicationListeners[i].CloseAsync(default);
            }

            await this.serviceReplica.InvokeOnChangeRoleAsync(ReplicaRole.ActiveSecondary);
        }
    }
}