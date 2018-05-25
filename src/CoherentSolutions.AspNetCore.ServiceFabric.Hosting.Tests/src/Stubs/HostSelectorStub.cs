using System;
using System.Collections.Generic;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tests.Stubs
{
    internal class HostSelectorStub : IHostSelector
    {
        private class HostDescriptorStub : IHostDescriptor
        {
            private class HostKeywordsStub : IHostKeywords
            {
                public string[] GetKeywords()
                {
                    return Array.Empty<string>();
                }
            }

            private class HostRunnerStub : IHostRunner
            {
                public void Run()
                {
                }
            }

            public IHostKeywords Keywords { get; }

            public IHostRunner Runner { get; }

            public HostDescriptorStub()
            {
                this.Keywords = new HostKeywordsStub();
                this.Runner = new HostRunnerStub();
            }
        }

        public IHostDescriptor Select(
            IEnumerable<IHostKeywordsProvider> keywordsProviders,
            IEnumerable<IHostDescriptor> descriptors)
        {
            return new HostDescriptorStub();
        }

        public static IHostSelector Func()
        {
            return new HostSelectorStub();
        }
    }
}