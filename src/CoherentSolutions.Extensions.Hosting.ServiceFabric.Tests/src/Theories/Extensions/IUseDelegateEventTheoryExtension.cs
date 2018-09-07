namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public interface IUseDelegateEventTheoryExtension<out TLifecycleEvent>
    {
        TLifecycleEvent Event { get; }
    }
}