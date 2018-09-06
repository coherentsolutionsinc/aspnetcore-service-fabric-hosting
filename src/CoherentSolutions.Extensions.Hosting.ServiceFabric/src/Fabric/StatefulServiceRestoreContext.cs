using System.Threading;
using System.Threading.Tasks;

using Microsoft.ServiceFabric.Data;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatefulServiceRestoreContext : IStatefulServiceRestoreContext
    {
        private RestoreContext restoreCtx;

        public bool IsRestored { get; private set; }

        public string BackupFolderPath { get; private set; }

        public RestorePolicy BackupPolicy { get; private set; }

        public StatefulServiceRestoreContext(
            RestoreContext restoreCtx)
        {
            this.restoreCtx = restoreCtx;
        }

        public Task RestoreAsync(
            RestoreDescription restoreDescription)
        {
            return this.RestoreAsync(restoreDescription, CancellationToken.None);
        }

        public async Task RestoreAsync(
            RestoreDescription restoreDescription,
            CancellationToken cancellationToken)
        {
            await this.restoreCtx.RestoreAsync(restoreDescription, cancellationToken);

            this.IsRestored = true;
            this.BackupFolderPath = restoreDescription.BackupFolderPath;
            this.BackupPolicy = restoreDescription.Policy;
        }
    }
}