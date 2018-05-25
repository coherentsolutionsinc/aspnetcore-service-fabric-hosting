using System.Collections.Generic;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting
{
    public interface IHostSelector
    {
        IHostDescriptor Select(
            IEnumerable<IHostKeywordsProvider> keywordsProviders,
            IEnumerable<IHostDescriptor> descriptors);
    }
}