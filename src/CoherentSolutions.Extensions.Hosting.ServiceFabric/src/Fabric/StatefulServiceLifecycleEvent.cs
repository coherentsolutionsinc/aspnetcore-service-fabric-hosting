namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public enum StatefulServiceLifecycleEvent
    {
        OnRunBeforeListenersAreOpened,
        OnRunAfterListenersAreOpened,
        OnRunBeforeRoleChanged,
        OnRunAfterRoleChanged,
        OnAbort,
        OnOpen,
        OnClose,
        OnDataLoss,
        OnRestoreCompleted
    }
}