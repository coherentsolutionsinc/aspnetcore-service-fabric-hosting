using System;
using System.Fabric;
using System.Numerics;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.NodeContexts
{
    public class NodeContextProvider
    {
        public NodeContext GetNodeContext()
        {
            return new NodeContext(
                Environment.MachineName,
                new NodeId(new BigInteger(1), new BigInteger(0)),
                new BigInteger(1),
                Environment.MachineName,
                Environment.MachineName);
        }
    }
}