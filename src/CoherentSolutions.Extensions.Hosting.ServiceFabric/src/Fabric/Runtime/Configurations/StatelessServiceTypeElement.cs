namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.Configurations
{
    public class StatelessServiceTypeElement : ServiceTypeElement
    {
        public override ServiceTypeElementKind Kind => ServiceTypeElementKind.Stateless;
    }
}