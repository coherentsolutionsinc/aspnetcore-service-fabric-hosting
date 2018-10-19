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

        public override string ToString()
        {
            return this.name;
        }

        public TheoryItem Resolve()
        {
            var item = new TheoryItem(this.name);

            switch (this.setup)
            {
                case TheoryItemSetup.AsStatefulEventSource:
                    item.SetupExtensionsAsEventSource()
                       .SetupConfigAsStatefulService(TheoryItemConfigure.ConfigureEventSourceExtensions);
                    break;
                case TheoryItemSetup.AsStatelessEventSource:
                    item.SetupExtensionsAsEventSource()
                       .SetupConfigAsStatelessService(TheoryItemConfigure.ConfigureEventSourceExtensions);
                    break;
                case TheoryItemSetup.AsStatefulDelegate:
                    item.SetupExtensionsAsStatefulDelegate()
                       .SetupConfigAsStatefulService(TheoryItemConfigure.ConfigureDelegateExtensions);
                    break;
                case TheoryItemSetup.AsStatelessDelegate:
                    item.SetupExtensionsAsStatelessDelegate()
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

        public void Deserialize(
            IXunitSerializationInfo info)
        {
            this.name = info.GetValue<string>(nameof(this.name));
            this.setup = info.GetValue<TheoryItemSetup>(nameof(this.setup));
        }

        public void Serialize(
            IXunitSerializationInfo info)
        {
            info.AddValue(nameof(this.name), this.name);
            info.AddValue(nameof(this.setup), this.setup);
        }
    }
}