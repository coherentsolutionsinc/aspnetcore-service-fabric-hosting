using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatefulServiceEventPayloadOnDataLoss : IStatefulServiceEventPayloadOnDataLoss
    {
        public IStatefulServiceRestoreContext RestoreContext { get; }

        public StatefulServiceEventPayloadOnDataLoss(
            IStatefulServiceRestoreContext restoreContext)
        {
            this.RestoreContext = restoreContext
             ?? throw new ArgumentNullException(nameof(restoreContext));
        }
    }
}