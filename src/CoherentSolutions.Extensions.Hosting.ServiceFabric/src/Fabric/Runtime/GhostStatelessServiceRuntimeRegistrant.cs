using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging.Console;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime
{
    public class GhostStatelessServiceRuntimeRegistrant : IStatelessServiceRuntimeRegistrant
    {
        public async Task RegisterAsync(
            string serviceTypeName,
            Func<StatelessServiceContext, StatelessService> serviceFactory,
            CancellationToken cancellationToken)
        {
            var context = new StatelessServiceContext(
                GhostServiceRuntime.GetNodeContext(),
                GhostServiceRuntime.GetCodePackageActivationContext(),
                serviceTypeName,
                runtime.CreateServiceName(),
                null,
                Guid.NewGuid(),
                runtime.CreateInstanceId());

            var service = serviceFactory(context);
            
            var logger = new ConsoleLogger(serviceTypeName, null, true);

            var instance = new GhostStatelessServiceInstance(service, partition, logger);

            await instance.StartupAsync();


        private static readonly Lazy<Action<StatelessService, IStatelessServicePartition>> setPartition;
        setPartition = new Lazy<Action<StatelessService, IStatelessServicePartition>>(
                () =>
                {
                    const string NAME = "Partition";
                    const BindingFlags FLAGS = BindingFlags.NonPublic | BindingFlags.Instance;

                    var p = typeof(StatelessService).GetProperty(NAME, FLAGS);
                    if (p?.SetMethod is null)
                    {
                        if (p?.DeclaringType is null)
                        {
                            throw new MissingMemberException(nameof(StatelessService), NAME);
                        }

                        p = p.DeclaringType.GetProperty(NAME, FLAGS);
                    }

                    if (p is null)
                    {
                        throw new MissingMemberException(nameof(StatelessService), NAME);
                    }

                    return (
                        service,
                        partition) =>
                    {
                        p.SetValue(service, partition);
                    };
                },
                true);
        }
    

        private void SetPartition()
{
    this.logger.LogInformation("Injecting partition information");

    setPartition.Value(this.service, this.partition);

    this.logger.LogInformation("Done injecting partition information");
}
public Task UnregisterAsync(
            string serviceTypeName,
            CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}