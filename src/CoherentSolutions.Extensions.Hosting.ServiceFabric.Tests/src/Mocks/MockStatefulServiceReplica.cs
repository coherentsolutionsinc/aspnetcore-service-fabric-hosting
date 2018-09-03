using System.Fabric;
using System.Linq;
using System.Threading.Tasks;

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
            var openListenersTask = Task.Run(
                async () =>
                {
                    var communicationListeners = this.serviceReplica
                       .InvokeCreateServiceReplicaListeners()
                       .Select(l => l.CreateCommunicationListener(this.serviceReplica.Context))
                       .ToArray();

                    var communicationListenersOpenTasks = new Task[communicationListeners.Length];
                    for (var i = 0; i < communicationListeners.Length; ++i)
                    {
                        communicationListenersOpenTasks[i] = communicationListeners[i].OpenAsync(default);
                    }

                    await Task.WhenAll(communicationListenersOpenTasks);
                });

            var runAsyncTask = this.serviceReplica.InvokeRunAsync();

            await openListenersTask;

            await this.serviceReplica.InvokeOnChangeRoleAsync(ReplicaRole.Primary);

            await runAsyncTask;
        }

        private async Task DemoteSequenceAsync()
        {
            var closeListenersTask = Task.Run(
                async () =>
                {
                    var communicationListeners = this.serviceReplica
                       .InvokeCreateServiceReplicaListeners()
                       .Select(l => l.CreateCommunicationListener(this.serviceReplica.Context))
                       .ToArray();

                    var communicationListenersCloseTasks = new Task[communicationListeners.Length];
                    for (var i = 0; i < communicationListeners.Length; ++i)
                    {
                        communicationListenersCloseTasks[i] = communicationListeners[i].CloseAsync(default);
                    }

                    await Task.WhenAll(communicationListenersCloseTasks);
                });

            await closeListenersTask;

            await this.serviceReplica.InvokeOnChangeRoleAsync(ReplicaRole.ActiveSecondary);
        }
    }
}