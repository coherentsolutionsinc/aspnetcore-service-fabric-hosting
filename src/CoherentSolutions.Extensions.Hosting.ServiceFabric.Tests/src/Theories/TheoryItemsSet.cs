using System.Collections.Generic;
using System.Linq;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Items;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories
{
    public static class TheoryItemsSet
    {
        public static TheoryItemPromise StatefulServiceEventSource => new TheoryItemPromise("Stateful-EventSource", TheoryItemSetup.AsStatefulEventSource);

        public static TheoryItemPromise StatelessServiceEventSource => new TheoryItemPromise("Stateless-EventSource", TheoryItemSetup.AsStatelessEventSource);

        public static TheoryItemPromise StatefulServiceDelegate => new TheoryItemPromise("Stateful-Delegate", TheoryItemSetup.AsStatefulDelegate);

        public static TheoryItemPromise StatelessServiceDelegate => new TheoryItemPromise("Stateless-Delegate", TheoryItemSetup.AsStatelessDelegate);

        public static TheoryItemPromise StatefulServiceAspNetCoreListener
            => new TheoryItemPromise("Stateful-AspNetCoreListener", TheoryItemSetup.AsStatefulAspNetCoreListener);

        public static TheoryItemPromise StatelessServiceAspNetCoreListener
            => new TheoryItemPromise("Stateless-AspNetCoreListener", TheoryItemSetup.AsStatelessAspNetCoreListener);

        public static TheoryItemPromise StatefulServiceRemotingListener
            => new TheoryItemPromise("Stateful-RemotingListener", TheoryItemSetup.AsStatefulRemotingListener);

        public static TheoryItemPromise StatelessServiceRemotingListener
            => new TheoryItemPromise("Stateless-RemotingListener", TheoryItemSetup.AsStatelessRemotingListener);

        public static IEnumerable<TheoryItemPromise> SupportDependencyInjection
        {
            get
            {
                yield return StatefulServiceEventSource;
                yield return StatelessServiceEventSource;
                yield return StatefulServiceDelegate;
                yield return StatefulServiceRemotingListener;
                yield return StatelessServiceDelegate;
                yield return StatelessServiceRemotingListener;
            }
        }

        public static IEnumerable<TheoryItemPromise> SupportEventSourcingAndLogging
        {
            get
            {
                yield return StatefulServiceDelegate;
                yield return StatefulServiceAspNetCoreListener;
                yield return StatefulServiceRemotingListener;
                yield return StatelessServiceDelegate;
                yield return StatelessServiceAspNetCoreListener;
                yield return StatelessServiceRemotingListener;
            }
        }

        public static IEnumerable<TheoryItemPromise> StatefulItems
        {
            get
            {
                yield return StatefulServiceEventSource;
                yield return StatefulServiceDelegate;
                yield return StatefulServiceAspNetCoreListener;
                yield return StatefulServiceRemotingListener;
            }
        }

        public static IEnumerable<TheoryItemPromise> StatelessItems
        {
            get
            {
                yield return StatelessServiceEventSource;
                yield return StatelessServiceDelegate;
                yield return StatelessServiceAspNetCoreListener;
                yield return StatelessServiceRemotingListener;
            }
        }

        public static IEnumerable<TheoryItemPromise> EventSourceItems
            => new[]
            {
                StatefulServiceEventSource,
                StatelessServiceEventSource
            };

        public static IEnumerable<TheoryItemPromise> DelegateItems
            => new[]
            {
                StatefulServiceDelegate,
                StatelessServiceDelegate
            };

        public static IEnumerable<TheoryItemPromise> AspNetCoreListenerItems
            => new[]
            {
                StatefulServiceAspNetCoreListener,
                StatelessServiceAspNetCoreListener
            };

        public static IEnumerable<TheoryItemPromise> RemotingListenerItems
            => new[]
            {
                StatefulServiceRemotingListener,
                StatelessServiceRemotingListener
            };

        public static IEnumerable<TheoryItemPromise> AllListenerItems => AspNetCoreListenerItems.Concat(RemotingListenerItems);

        public static IEnumerable<TheoryItemPromise> AllItems => StatefulItems.Concat(StatelessItems);
    }
}