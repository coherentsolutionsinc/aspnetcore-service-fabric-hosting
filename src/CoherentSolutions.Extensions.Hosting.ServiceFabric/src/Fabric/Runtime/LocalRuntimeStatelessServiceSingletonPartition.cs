using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Health;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.Services;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime
{
    public class LocalRuntimeStatelessServiceSingletonPartition : IStatelessServicePartition
    {
        public ServicePartitionInformation PartitionInfo { get; }

        public LocalRuntimeStatelessServiceSingletonPartition(
            Guid id)
        {
            this.PartitionInfo = new ServicePartitionInformationAccessor<SingletonPartitionInformation>(
                    new SingletonPartitionInformation())
                {
                    Id = id
                }
               .Instance;
        }

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