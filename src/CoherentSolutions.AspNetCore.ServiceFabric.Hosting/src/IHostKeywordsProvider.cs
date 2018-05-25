using System.Collections.Generic;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting
{
    public interface IHostKeywordsProvider
    {
        IEnumerable<string> GetKeywords();
    }
}