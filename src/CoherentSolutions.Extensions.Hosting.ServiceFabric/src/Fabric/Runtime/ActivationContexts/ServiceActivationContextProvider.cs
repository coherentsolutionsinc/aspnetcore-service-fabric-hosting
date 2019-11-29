using System;
using System.IO;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ActivationContexts
{
    public class ServiceActivationContextProvider : IServiceActivationContextProvider
    {
        private const string APPLICATION_NAME = "fabric:/ApplicationName";

        private const string APPLICATION_TYPE_NAME = "ApplicationTypeName";

        private const string CODE_PACKAGE_NAME = "Code";

        private const string CODE_PACKAGE_VERSION = "1.0.0";

        private const string LOG_DIRECTORY = "Log";

        private const string WORK_DIRECTORY = "Work";

        private const string TEMP_DIRECTORY = "Temp";

        public IServiceActivationContext GetActivationContext()
        {
            var id = Guid.NewGuid().ToString();
            var path = Path.GetTempPath();
            return new ServiceActivationContext(
                APPLICATION_NAME,
                APPLICATION_TYPE_NAME,
                id,
                CODE_PACKAGE_NAME,
                CODE_PACKAGE_VERSION,
                Path.Combine(path, id, LOG_DIRECTORY),
                Path.Combine(path, id, TEMP_DIRECTORY),
                Path.Combine(path, id, WORK_DIRECTORY));
        }
    }
}