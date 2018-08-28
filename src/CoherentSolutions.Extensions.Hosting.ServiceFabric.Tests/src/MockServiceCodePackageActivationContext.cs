using System.Collections.ObjectModel;
using System.Fabric;
using System.Fabric.Description;

using ServiceFabric.Mocks;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests
{
    public class MockServiceCodePackageActivationContext
    {
        private class EndpointResourceDescriptionCollection : KeyedCollection<string, EndpointResourceDescription>
        {
            protected override string GetKeyForItem(
                EndpointResourceDescription item)
            {
                return item.Name;
            }
        }

        public static ICodePackageActivationContext Default
        {
            get
            {
                var code = MockCodePackageActivationContext.Default;
                if (code is MockCodePackageActivationContext package)
                {
                    package.EndpointResourceDescriptions = new EndpointResourceDescriptionCollection
                    {
                        new EndpointResourceDescription
                        {
                            Name = "ServiceEndpoint"
                        }
                    };
                }

                return code;
            }
        }
    }
}