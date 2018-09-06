using System.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatefulServiceEventPayloadOnChangeRole : IStatefulServiceEventPayloadOnChangeRole
    {
        public ReplicaRole NewRole { get; }

        public StatefulServiceEventPayloadOnChangeRole(
            ReplicaRole newRole)
        {
            this.NewRole = newRole;
        }
    }
}