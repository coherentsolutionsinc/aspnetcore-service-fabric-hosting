using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Health;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime
{
    public class GhostStatelessServiceSingletonPartition : IStatelessServicePartition
    {
        public ServicePartitionInformation PartitionInfo => new SingletonPartitionInformation();

        public void ReportFault(
            FaultType faultType)
        {
        }

        public void ReportInstanceHealth(
            HealthInformation healthInfo)
        {
        }

        public void ReportInstanceHealth(
            HealthInformation healthInfo,
            HealthReportSendOptions sendOptions)
        {
        }

        public void ReportLoad(
            IEnumerable<LoadMetric> metrics)
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
    }
}