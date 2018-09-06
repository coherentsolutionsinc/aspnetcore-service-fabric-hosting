using System.Threading;
using System.Threading.Tasks;

using Microsoft.ServiceFabric.Data;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceRestoreContext
    {
        Task RestoreAsync(
            RestoreDescription restoreDescription);

        Task RestoreAsync(
            RestoreDescription restoreDescription,
            CancellationToken cancellationToken);
    }
}