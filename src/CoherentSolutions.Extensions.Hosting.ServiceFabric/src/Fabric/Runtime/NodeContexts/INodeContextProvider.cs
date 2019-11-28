using System.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.NodeContexts
{
    public interface INodeContextProvider
    {
        NodeContext GetNodeContext();
    }
}