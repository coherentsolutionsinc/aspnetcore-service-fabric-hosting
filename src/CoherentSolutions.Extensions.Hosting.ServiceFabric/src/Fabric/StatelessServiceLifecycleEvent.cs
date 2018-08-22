namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public enum StatelessServiceLifecycleEvent
    {
        OnRunBeforeListenersAreOpened,
        OnRunAfterListenersAreOpened,
        OnAbort,
        OnOpen,
        OnClose
    }
}