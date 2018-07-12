using System.Collections.Generic;
using System.Linq;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Items;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories
{
    public static class TheoryItemsSet
    {
        public static TheoryItem StatefulServiceDelegate
            => new TheoryItem("Stateful-Delegate")
               .SetupExtensionsAsDelegate()
               .SetupConfigAsStatefulService(TheoryItemConfigure.ConfigureDelegateExtensions);

        public static TheoryItem StatefulServiceAspNetCoreListener
            => new TheoryItem("Stateful-AspNetCoreListener")
               .SetupExtensionsAsAspNetCoreListener()
               .SetupConfigAsStatefulService(TheoryItemConfigure.ConfigureAspNetCoreListenerExtensions);

        public static TheoryItem StatefulServiceRemotingListener
            => new TheoryItem("Stateful-RemotingListener")
               .SetupExtensionsAsRemotingListener()
               .SetupConfigAsStatefulService(TheoryItemConfigure.ConfigureRemotingListenerExtensions);

        public static TheoryItem StatelessServiceDelegate
            => new TheoryItem("Stateless-Delegate")
               .SetupExtensionsAsDelegate()
               .SetupAsConfigStatelessService(TheoryItemConfigure.ConfigureDelegateExtensions);

        public static TheoryItem StatelessServiceAspNetCoreListener
            => new TheoryItem("Stateless-AspNetCoreListener")
               .SetupExtensionsAsAspNetCoreListener()
               .SetupAsConfigStatelessService(TheoryItemConfigure.ConfigureAspNetCoreListenerExtensions);

        public static TheoryItem StatelessServiceRemotingListener
            => new TheoryItem("Stateless-RemotingListener")
               .SetupExtensionsAsRemotingListener()
               .SetupAsConfigStatelessService(TheoryItemConfigure.ConfigureRemotingListenerExtensions);

        public static IEnumerable<TheoryItem> SupportDependencyInjection
        {
            get
            {
                yield return StatefulServiceDelegate;
                yield return StatefulServiceRemotingListener;
                yield return StatelessServiceDelegate;
                yield return StatelessServiceRemotingListener;
            }
        }

        public static IEnumerable<TheoryItem> StatefulItems
        {
            get
            {
                yield return StatefulServiceDelegate;
                yield return StatefulServiceAspNetCoreListener;
                yield return StatefulServiceRemotingListener;
            }
        }

        public static IEnumerable<TheoryItem> StatelessItems
        {
            get
            {
                yield return StatelessServiceDelegate;
                yield return StatelessServiceAspNetCoreListener;
                yield return StatelessServiceRemotingListener;
            }
        }

        public static IEnumerable<TheoryItem> DelegateItems
            => new[]
            {
                StatefulServiceDelegate,
                StatelessServiceDelegate
            };

        public static IEnumerable<TheoryItem> AspNetCoreListenerItems
            => new[]
            {
                StatefulServiceAspNetCoreListener,
                StatelessServiceAspNetCoreListener
            };

        public static IEnumerable<TheoryItem> RemotingListenerItems
            => new[]
            {
                StatefulServiceRemotingListener,
                StatelessServiceRemotingListener
            };

        public static IEnumerable<TheoryItem> AllListenerItems => AspNetCoreListenerItems.Concat(RemotingListenerItems);

        public static IEnumerable<TheoryItem> AllItems => StatefulItems.Concat(StatelessItems);
    }
}