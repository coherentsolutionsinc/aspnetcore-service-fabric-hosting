using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Health;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests
{
    public class MockStatefulServicePartition : IStatefulServicePartition
    {
        public ServicePartitionInformation PartitionInfo => null;

        public void ReportLoad(
            IEnumerable<LoadMetric> metrics)
        {
        }

        public void ReportFault(
            FaultType faultType)
        {
        }

        public void ReportMoveCost(
            MoveCost moveCost)
        {
        }

        public void ReportPartitionHealth(
            HealthInformation healthInfo)
        {
        }

        public void ReportPartitionHealth(
            HealthInformation healthInfo,
            HealthReportSendOptions sendOptions)
        {
        }

        public FabricReplicator CreateReplicator(
            IStateProvider stateProvider,
            ReplicatorSettings replicatorSettings)
        {
            return null;
        }

        public void ReportReplicaHealth(
            HealthInformation healthInfo)
        {
        }

        public void ReportReplicaHealth(
            HealthInformation healthInfo,
            HealthReportSendOptions sendOptions)
        {
        }

        public PartitionAccessStatus ReadStatus => PartitionAccessStatus.Granted;

        public PartitionAccessStatus WriteStatus => PartitionAccessStatus.Granted;
    }
}