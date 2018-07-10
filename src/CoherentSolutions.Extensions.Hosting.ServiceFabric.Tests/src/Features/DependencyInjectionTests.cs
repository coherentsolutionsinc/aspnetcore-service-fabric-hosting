using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions.Support;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Items;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceFabric.Data;

using Moq;

using Xunit;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Features
{
    public static class DependencyInjectionTests
    {
        private static class UseDependencies
        {
            public class Case
            {
                public TheoryItem TheoryItem { get; }

                public Case(
                    TheoryItem theoryItem)
                {
                    this.TheoryItem = theoryItem;
                }

                public override string ToString()
                {
                    return this.TheoryItem.ToString();
                }
            }

            public static IEnumerable<object[]> Cases
            {
                get
                {
                    foreach (var item in TheoryItemsSet
                       .AllItems
                       .Where(i => i is IUseDependenciesTheoryExtensionSupported))
                    {
                        yield return new object[]
                        {
                            new Case(item)
                        };
                    }
                }
            }
        }

        private static class AutomaticServicesRegistration
        {
            public class Case
            {
                public TheoryItem TheoryItem { get; }

                public Type RequiredType { get; }

                public Case(
                    TheoryItem theoryItem,
                    Type requiredType)
                {
                    this.TheoryItem = theoryItem;
                    this.RequiredType = requiredType;
                }

                public override string ToString()
                {
                    return $"{this.TheoryItem} & {this.RequiredType.Name}";
                }
            }

            public static IEnumerable<object[]> Cases
            {
                get
                {
                    foreach (var item in TheoryItemsSet.AllItems)
                    {
                        yield return new object[]
                        {
                            new Case(item, typeof(ServiceContext))
                        };
                        yield return new object[]
                        {
                            new Case(item, typeof(IServicePartition))
                        };
                        yield return new object[]
                        {
                            new Case(item, typeof(IServiceEventSource))
                        };
                        yield return new object[]
                        {
                            new Case(item, typeof(ILoggerProvider))
                        };
                    }

                    foreach (var item in TheoryItemsSet.AllListenerItems)
                    {
                        yield return new object[]
                        {
                            new Case(item, typeof(IServiceHostListenerInformation))
                        };
                    }

                    foreach (var item in TheoryItemsSet.AspNetCoreListenerItems)
                    {
                        yield return new object[]
                        {
                            new Case(item, typeof(IServiceHostAspNetCoreListenerInformation))
                        };
                    }

                    foreach (var item in TheoryItemsSet.RemotingListenerItems)
                    {
                        yield return new object[]
                        {
                            new Case(item, typeof(IServiceHostRemotingListenerInformation))
                        };
                    }

                    foreach (var item in TheoryItemsSet.StatefulItems)
                    {
                        yield return new object[]
                        {
                            new Case(item, typeof(IReliableStateManager))
                        };
                        yield return new object[]
                        {
                            new Case(item, typeof(StatefulServiceContext))
                        };
                        yield return new object[]
                        {
                            new Case(item, typeof(IStatefulServicePartition))
                        };
                    }

                    foreach (var item in TheoryItemsSet.StatelessItems)
                    {
                        yield return new object[]
                        {
                            new Case(item, typeof(StatelessServiceContext))
                        };
                        yield return new object[]
                        {
                            new Case(item, typeof(IStatelessServicePartition))
                        };
                    }
                }
            }
        }

        private static class HierarchyServiceRegistration
        {
            public class Case
            {
                public TheoryItem TheoryItem { get; }

                public Case(
                    TheoryItem theoryItem)
                {
                    this.TheoryItem = theoryItem;
                }

                public override string ToString()
                {
                    return this.TheoryItem.ToString();
                }
            }

            public static IEnumerable<object[]> SingletonInstanceCases
            {
                get
                {
                    foreach (var item in TheoryItemsSet.AllItems)
                    {
                        yield return new object[]
                        {
                            new Case(item)
                        };
                    }
                }
            }
        }

        [Theory]
        [MemberData(nameof(UseDependencies.Cases), MemberType = typeof(UseDependencies))]
        private static void Should_use_custom_collection_For_services_registrations(
            UseDependencies.Case @case)
        {
            // Arrange
            var mockCollection = new Mock<ServiceCollection>
            {
                CallBase = true
            };
            mockCollection
               .As<IServiceCollection>()
               .Setup(instance => instance.Add(It.IsAny<ServiceDescriptor>()))
               .Verifiable();

            var arrangeCollection = mockCollection.Object;

            var theoryItem = @case.TheoryItem;

            // Act
            theoryItem.SetupExtension(new UseDependenciesTheoryExtension().Setup(() => arrangeCollection));
            theoryItem.Try();

            // Assert
            mockCollection.Verify();
        }

        [Theory]
        [MemberData(nameof(AutomaticServicesRegistration.Cases), MemberType = typeof(AutomaticServicesRegistration))]
        private static void Should_resolve_instance_From_autoregistred_types(
            AutomaticServicesRegistration.Case @case)
        {
            // Arrange
            var theoryItem = @case.TheoryItem;
            var requiredType = @case.RequiredType;

            object actualObject = null;

            // Act
            theoryItem.SetupExtension(new ResolveDependencyTheoryExtension().Setup(requiredType, o => actualObject = o));
            theoryItem.Try();

            // Assert
            Assert.IsAssignableFrom(requiredType, actualObject);
        }

        [Theory]
        [MemberData(nameof(HierarchyServiceRegistration.SingletonInstanceCases), MemberType = typeof(HierarchyServiceRegistration))]
        private static void Should_resolve_same_singleton_instance_From_hierarchy_singleton_instance_registration(
            HierarchyServiceRegistration.Case @case)
        {
            // Arrange
            var theoryItem = @case.TheoryItem;

            var arrangeDescriptor = new ServiceDescriptor(typeof(Tools.ITestDependency), new Tools.TestDependency());

            object expectedObject = null;
            object actualObject = null;

            // Act
            theoryItem.SetupConfig(
                builder =>
                {
                    builder.ConfigureServices((
                        context,
                        collection) =>
                    {
                        collection.Add(arrangeDescriptor);
                    });
                });
            theoryItem.SetupCheck(
                host =>
                {
                    expectedObject = host.Services.GetService<Tools.ITestDependency>();
                });

            theoryItem.SetupExtension(new ResolveDependencyTheoryExtension().Setup(typeof(Tools.ITestDependency), o => actualObject = o));
            theoryItem.Try();

            // Assert
            Assert.Same(expectedObject, actualObject);
        }

        [Theory]
        [MemberData(nameof(HierarchyServiceRegistration.SingletonInstanceCases), MemberType = typeof(HierarchyServiceRegistration))]
        private static void Should_resolve_same_singleton_instance_From_hierarchy_singleton_type_registration(
            HierarchyServiceRegistration.Case @case)
        {
            // Arrange
            var theoryItem = @case.TheoryItem;

            var arrangeDescriptor = new ServiceDescriptor(typeof(Tools.ITestDependency), typeof(Tools.TestDependency), ServiceLifetime.Singleton);

            object expectedObject = null;
            object actualObject = null;

            // Act
            theoryItem.SetupConfig(
                builder =>
                {
                    builder.ConfigureServices((
                        context,
                        collection) =>
                    {
                        collection.Add(arrangeDescriptor);
                    });
                });
            theoryItem.SetupCheck(
                host =>
                {
                    expectedObject = host.Services.GetService<Tools.ITestDependency>();
                });

            theoryItem.SetupExtension(new ResolveDependencyTheoryExtension().Setup(typeof(Tools.ITestDependency), o => actualObject = o));
            theoryItem.Try();

            // Assert
            Assert.Same(expectedObject, actualObject);
        }

        [Theory]
        [MemberData(nameof(HierarchyServiceRegistration.SingletonInstanceCases), MemberType = typeof(HierarchyServiceRegistration))]
        private static void Should_resolve_same_open_generic_singleton_instance_From_hierarchy_open_generic_singleton_type_registration(
            HierarchyServiceRegistration.Case @case)
        {
            // Arrange
            var theoryItem = @case.TheoryItem;

            var arrangeDescriptor = new ServiceDescriptor(typeof(Tools.ITestGenericDependency<>), typeof(Tools.TestGenericDependency<>), ServiceLifetime.Singleton);

            object expectedObject = null;
            object actualObject = null;

            // Act
            theoryItem.SetupConfig(
                builder =>
                {
                    builder.ConfigureServices((
                        context,
                        collection) =>
                    {
                        collection.Add(arrangeDescriptor);
                    });
                });
            theoryItem.SetupCheck(
                host =>
                {
                    expectedObject = host.Services.GetService<Tools.ITestGenericDependency<int>>();
                });

            theoryItem.SetupExtension(new ResolveDependencyTheoryExtension().Setup(typeof(Tools.ITestGenericDependency<int>), o => actualObject = o));
            theoryItem.Try();

            // Assert
            Assert.Same(expectedObject, actualObject);
        }

        [Theory]
        [MemberData(nameof(HierarchyServiceRegistration.SingletonInstanceCases), MemberType = typeof(HierarchyServiceRegistration))]
        private static void Should_resolve_different_transient_instance_From_hierarchy_transient_type_registration(
            HierarchyServiceRegistration.Case @case)
        {
            // Arrange
            var theoryItem = @case.TheoryItem;

            var arrangeDescriptor = new ServiceDescriptor(typeof(Tools.ITestDependency), typeof(Tools.TestDependency), ServiceLifetime.Transient);

            object expectedObject = null;
            object actualObject = null;

            // Act
            theoryItem.SetupConfig(
                builder =>
                {
                    builder.ConfigureServices((
                        context,
                        collection) =>
                    {
                        collection.Add(arrangeDescriptor);
                    });
                });
            theoryItem.SetupCheck(
                host =>
                {
                    expectedObject = host.Services.GetService<Tools.ITestDependency>();
                });

            theoryItem.SetupExtension(new ResolveDependencyTheoryExtension().Setup(typeof(Tools.ITestDependency), o => actualObject = o));
            theoryItem.Try();

            // Assert
            Assert.NotSame(expectedObject, actualObject);
        }

        [Theory]
        [MemberData(nameof(HierarchyServiceRegistration.SingletonInstanceCases), MemberType = typeof(HierarchyServiceRegistration))]
        private static void Should_resolve_different_open_generic_transient_instance_From_hierarchy_open_generic_transient_type_registration(
            HierarchyServiceRegistration.Case @case)
        {
            // Arrange
            var theoryItem = @case.TheoryItem;

            var arrangeDescriptor = new ServiceDescriptor(typeof(Tools.ITestGenericDependency<>), typeof(Tools.TestGenericDependency<>), ServiceLifetime.Transient);

            object expectedObject = null;
            object actualObject = null;

            // Act
            theoryItem.SetupConfig(
                builder =>
                {
                    builder.ConfigureServices((
                        context,
                        collection) =>
                    {
                        collection.Add(arrangeDescriptor);
                    });
                });
            theoryItem.SetupCheck(
                host =>
                {
                    expectedObject = host.Services.GetService<Tools.ITestGenericDependency<int>>();
                });

            theoryItem.SetupExtension(new ResolveDependencyTheoryExtension().Setup(typeof(Tools.ITestGenericDependency<int>), o => actualObject = o));
            theoryItem.Try();

            // Assert
            Assert.NotSame(expectedObject, actualObject);
        }
    }
}