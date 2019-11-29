namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ActivationContexts
{
    public interface IServiceActivationContext
    {
        string ApplicationName { get; }

        string ApplicationTypeName { get; }

        string ActivationContextId { get; }

        string LogDirectory { get; }

        string TempDirectory { get; }

        string WorkDirectory { get; }

        string CodePackageName { get; }

        string CodePackageVersion { get; }

        //Environment.SetEnvironmentVariable("Fabric_ServiceName", serviceName);
        //Environment.SetEnvironmentVariable("Fabric_IsContainerHost", bool.FalseString);

        //Environment.SetEnvironmentVariable("Fabric_CodePackageName", activationContext.CodePackageName);
    }
}