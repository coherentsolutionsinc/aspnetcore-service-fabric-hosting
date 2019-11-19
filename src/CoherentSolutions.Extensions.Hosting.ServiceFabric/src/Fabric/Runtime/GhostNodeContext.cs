using System;
using System.Fabric;
using System.Numerics;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime
{
    public class GhostNodeContext : NodeContext
    {
        private const string LOCALHOST_NODE_TYPE = "LocalhostNodeType";

        private const string LOCALHOST_IPADDRESS_OR_FQDN = "localhost";

        public GhostNodeContext()
            : base(
                Environment.MachineName,
                new NodeId(new BigInteger(1), new BigInteger(0)),
                new BigInteger(1),
                LOCALHOST_NODE_TYPE,
                LOCALHOST_IPADDRESS_OR_FQDN)
        {
        }
    }
}