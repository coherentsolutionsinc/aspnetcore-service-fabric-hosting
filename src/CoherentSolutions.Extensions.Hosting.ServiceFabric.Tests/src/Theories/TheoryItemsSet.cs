using System.Collections.Generic;
using System.Linq;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Items;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories
{
    public static class TheoryItemsSet
    {
        public static TheoryItem StatefulServiceDelegate => new StatefulServiceDelegateTheoryItem();

        public static TheoryItem StatefulServiceAspNetCoreListener => new StatefulServiceAspNetCoreListenerTheoryItem();

        public static TheoryItem StatefulServiceRemotingListener => new StatefulServiceRemotingListenerTheoryItem();

        public static TheoryItem StatelessServiceDelegate => new StatelessServiceDelegateTheoryItem();

        public static TheoryItem StatelessServiceAspNetCoreListener => new StatelessServiceAspNetCoreListenerTheoryItem();

        public static TheoryItem StatelessServiceRemotingListener => new StatelessServiceRemotingListenerTheoryItem();

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