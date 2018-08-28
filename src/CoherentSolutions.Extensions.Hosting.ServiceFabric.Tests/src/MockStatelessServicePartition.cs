﻿using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Health;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests
{
    public class MockStatelessServicePartition : IStatelessServicePartition
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

        public void ReportInstanceHealth(
            HealthInformation healthInfo)
        {
        }

        public void ReportInstanceHealth(
            HealthInformation healthInfo,
            HealthReportSendOptions sendOptions)
        {
        }
    }
}