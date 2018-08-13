using System;
using Xunit.Abstractions;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Items
{
    public class TheoryItemPromise : IXunitSerializable
    {
        private string name;
        private TheoryItemSetup setup;

        public TheoryItemPromise()
        {
        }

        public TheoryItemPromise(
            string name,
            TheoryItemSetup setup)
        {
            this.name = name 
                ?? throw new ArgumentNullException(nameof(name));

            this.setup = setup;
        }

        public TheoryItem Resolve()
        {
            var item = new TheoryItem(this.name);

            switch (this.setup)
            {
                case TheoryItemSetup.AsStatefulDelegate:
                    item.SetupExtensionsAsDelegate()
                        .SetupConfigAsStatefulService(TheoryItemConfigure.ConfigureDelegateExtensions);
                    break;
                case TheoryItemSetup.AsStatelessDelegate:
                    item.SetupExtensionsAsDelegate()
                        .SetupConfigAsStatelessService(TheoryItemConfigure.ConfigureDelegateExtensions);
                    break;
                case TheoryItemSetup.AsStatefulAspNetCoreListener:
                    item.SetupExtensionsAsAspNetCoreListener()
                        .SetupConfigAsStatefulService(TheoryItemConfigure.ConfigureAspNetCoreListenerExtensions);
                    break;
                case TheoryItemSetup.AsStatelessAspNetCoreListener:
                    item.SetupExtensionsAsAspNetCoreListener()
                        .SetupConfigAsStatelessService(TheoryItemConfigure.ConfigureAspNetCoreListenerExtensions);
                    break;
                case TheoryItemSetup.AsStatefulRemotingListener:
                    item.SetupExtensionsAsRemotingListener()
                        .SetupConfigAsStatefulService(TheoryItemConfigure.ConfigureRemotingListenerExtensions);
                    break;
                case TheoryItemSetup.AsStatelessRemotingListener:
                    item.SetupExtensionsAsRemotingListener()
                        .SetupConfigAsStatelessService(TheoryItemConfigure.ConfigureRemotingListenerExtensions);
                    break;
            }

            return item;
        }

        public void Deserialize(IXunitSerializationInfo info)
        {
            this.name = info.GetValue<string>(nameof(name));
            this.setup = info.GetValue<TheoryItemSetup>(nameof(setup));
        }

        public void Serialize(IXunitSerializationInfo info)
        {
            info.AddValue(nameof(name), this.name);
            info.AddValue(nameof(setup), this.setup);
        }
    }
}