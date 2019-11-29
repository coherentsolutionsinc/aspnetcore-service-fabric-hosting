using System.IO;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ActivationContexts
{
    public class ServiceActivationContextProvider : IServiceActivationContextProvider
    {
        private const string APPLICATION_NAME = "fabric:/ApplicationName";

        private const string APPLICATION_TYPE_NAME = "ApplicationTypeName";

        private const string ACTIVATION_CONTEXT_ID = "366B8CCC-8CC3-4EAA-8B90-938000A5EF52";

        private const string CODE_PACKAGE_NAME = "Code";

        private const string CODE_PACKAGE_VERSION = "1.0.0";

        private const string LOG_DIRECTORY = "Log";

        private const string WORK_DIRECTORY = "Work";

        private const string TEMP_DIRECTORY = "Temp";

        public IServiceActivationContext GetActivationContext()
        {
            var tempPath = Path.GetTempPath();
            return new ServiceActivationContext(
                APPLICATION_NAME,
                APPLICATION_TYPE_NAME,
                ACTIVATION_CONTEXT_ID,
                CODE_PACKAGE_NAME,
                CODE_PACKAGE_VERSION,
                Path.Combine(tempPath, ACTIVATION_CONTEXT_ID, LOG_DIRECTORY),
                Path.Combine(tempPath, ACTIVATION_CONTEXT_ID, TEMP_DIRECTORY),
                Path.Combine(tempPath, ACTIVATION_CONTEXT_ID, WORK_DIRECTORY));
        }
    }
}