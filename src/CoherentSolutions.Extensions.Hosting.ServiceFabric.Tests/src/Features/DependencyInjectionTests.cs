using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Items;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceFabric.Data;

using Xunit;
using Xunit.Abstractions;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Features
{
    public static class DependencyInjectionTests
    {
        private static class UseDependencies
        {
            public class Case : IXunitSerializable
            {
                public TheoryItemPromise Promise { get; private set; }

                public Case()
                {
                }

                public Case(
                    TheoryItemPromise theoryItem)
                {
                    this.Promise = theoryItem;
                }

                public override string ToString()
                {
                    return this.Promise.ToString();
                }

                public void Deserialize(IXunitSerializationInfo info)
                {
                    this.Promise = info.GetValue<TheoryItemPromise>(nameof(Promise));
                }

                public void Serialize(IXunitSerializationInfo info)
                {
                    info.AddValue(nameof(Promise), this.Promise);
                }
            }

            public static IEnumerable<object[]> Cases
            {
                get
                {
                    foreach (var item in TheoryItemsSet.SupportDependencyInjection)
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
            public class Case : IXunitSerializable
            {
                public TheoryItemPromise Promise { get; private set; }

                public Type RequiredType { get; private set; }

                public Case()
                {
                }

                public Case(
                    TheoryItemPromise theoryItem,
                    Type requiredType)
                {
                    this.Promise = theoryItem;
                    this.RequiredType = requiredType;
                }

                public override string ToString()
                {
                    return $"{this.Promise} & {this.RequiredType.Name}";
                }

                public void Deserialize(IXunitSerializationInfo info)
                {
                    this.Promise = info.GetValue<TheoryItemPromise>(nameof(Promise));
                    this.RequiredType = info.GetValue<Type>(nameof(RequiredType));
                }

                public void Serialize(IXunitSerializationInfo info)
                {
                    info.AddValue(nameof(Promise), this.Promise);
                    info.AddValue(nameof(RequiredType), this.RequiredType);
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

                    foreach (var item in TheoryItemsSet.DelegateItems)
                    {
                        yield return new object[]
                        {
                            new Case(item, typeof(CancellationToken))
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

                    yield return new object[]
                    {
                        new Case(TheoryItemsSet.StatefulServiceDelegate, typeof(IStatefulServiceDelegateInvocationContext))
                    };
                    yield return new object[]
                    {
                        new Case(TheoryItemsSet.StatelessServiceDelegate, typeof(IStatelessServiceDelegateInvocationContext))
                    };
                }
            }
        }

        private static class HierarchyServiceRegistration
        {
            public class Case : IXunitSerializable
            {
                public TheoryItemPromise Promise { get; private set; }

                public Case()
                {
                }

                public Case(
                    TheoryItemPromise theoryItem)
                {
                    this.Promise = theoryItem;
                }

                public override string ToString()
                {
                    return this.Promise.ToString();
                }

                public void Deserialize(IXunitSerializationInfo info)
                {
                    this.Promise = info.GetValue<TheoryItemPromise>(nameof(Promise));
                }

                public void Serialize(IXunitSerializationInfo info)
                {
                    info.AddValue(nameof(Promise), this.Promise);
                }
            }

            public class OpenGenericCase : IXunitSerializable
            {
                public TheoryItemPromise Promise { get; private set; }

                public Type ServiceType { get; private set; }

                public Type ImplementationType { get; private set; }

                public Type RequestType { get; private set; }

                public OpenGenericCase()
                {
                }

                public OpenGenericCase(
                    TheoryItemPromise theoryItem,
                    Type serviceType,
                    Type implementationType,
                    Type requestType)
                {
                    this.Promise = theoryItem;
                    this.ServiceType = serviceType;
                    this.ImplementationType = implementationType;
                    this.RequestType = requestType;
                }

                public override string ToString()
                {
                    return $"{this.Promise}-{this.ImplementationType.Name}";
                }

                public void Deserialize(IXunitSerializationInfo info)
                {
                    this.Promise = info.GetValue<TheoryItemPromise>(nameof(Promise));
                    this.ServiceType = info.GetValue<Type>(nameof(ServiceType));
                    this.ImplementationType = info.GetValue<Type>(nameof(ImplementationType));
                    this.RequestType = info.GetValue<Type>(nameof(RequestType));
                }

                public void Serialize(IXunitSerializationInfo info)
                {
                    info.AddValue(nameof(Promise), this.Promise);
                    info.AddValue(nameof(ServiceType), this.ServiceType);
                    info.AddValue(nameof(ImplementationType), this.ImplementationType);
                    info.AddValue(nameof(RequestType), this.RequestType);
                }
            }

            public static IEnumerable<object[]> GeneralCases
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

            public static IEnumerable<object[]> OpenGenericCases
            {
                get
                {
                    foreach (var item in TheoryItemsSet.AllItems)
                    {
                        yield return new object[]
                        {
                            new OpenGenericCase(
                                item,
                                typeof(ITestOpenGenericDependency<>),
                                typeof(TestOpenGenericDependency<>),
                                typeof(ITestOpenGenericDependency<int>))
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
            var arrangeCollection = new ServiceCollection();

            var theoryItem = @case.Promise.Resolve();

            // Act
            theoryItem.SetupExtension(new UseDependenciesTheoryExtension().Setup(() => arrangeCollection));
            theoryItem.Try();

            // Assert
            Assert.NotEmpty(arrangeCollection);
        }

        [Theory]
        [MemberData(nameof(AutomaticServicesRegistration.Cases), MemberType = typeof(AutomaticServicesRegistration))]
        private static void Should_resolve_instance_From_auto_registered_types(
            AutomaticServicesRegistration.Case @case)
        {
            // Arrange
            var theoryItem = @case.Promise.Resolve();
            var requiredType = @case.RequiredType;

            object actualObject = null;

            // Act
            theoryItem.SetupExtension(new PickDependencyTheoryExtension().Setup(requiredType, o => actualObject = o));
            theoryItem.Try();

            // Assert
            Assert.IsAssignableFrom(requiredType, actualObject);
        }

        [Theory]
        [MemberData(nameof(HierarchyServiceRegistration.GeneralCases), MemberType = typeof(HierarchyServiceRegistration))]
        private static void Should_resolve_lower_level_container_registration_When_lower_level_container_has_same_registration_as_upper_container(
            HierarchyServiceRegistration.Case @case)
        {
            // Arrange
            var theoryItem = @case.Promise.Resolve();

            var arrangeUpperLevelObject = new TestDependency();
            var arrangeLowerLevelObject = new TestDependency();

            object expectedUpperLevelObject = arrangeUpperLevelObject;
            object actualUpperLevelObject = null;

            object expectedLowerLevelObject = arrangeLowerLevelObject;
            object actualLowerLevelObject = null;

            // Act
            theoryItem.SetupConfig(
                (
                    builder,
                    provider) =>
                {
                    builder.ConfigureServices(
                        (
                            context,
                            collection) =>
                        {
                            collection.AddSingleton<ITestDependency>(arrangeUpperLevelObject);
                        });
                });
            theoryItem.SetupCheck(
                host =>
                {
                    actualUpperLevelObject = host.Services.GetService<ITestDependency>();
                });

            theoryItem.SetupExtension(new ConfigureDependenciesTheoryExtension().Setup(s => s.AddSingleton<ITestDependency>(arrangeLowerLevelObject)));
            theoryItem.SetupExtension(new PickDependencyTheoryExtension().Setup(typeof(ITestDependency), o => actualLowerLevelObject = o));
            theoryItem.Try();

            // Assert
            Assert.Same(expectedUpperLevelObject, actualUpperLevelObject);
            Assert.Same(expectedLowerLevelObject, actualLowerLevelObject);
        }

        [Theory]
        [MemberData(nameof(HierarchyServiceRegistration.GeneralCases), MemberType = typeof(HierarchyServiceRegistration))]
        private static void Should_resolve_same_singleton_instance_From_hierarchy_singleton_instance_registration(
            HierarchyServiceRegistration.Case @case)
        {
            // Arrange
            var theoryItem = @case.Promise.Resolve();

            var arrangeDescriptor = new ServiceDescriptor(typeof(ITestDependency), new TestDependency());

            object expectedObject = null;
            object actualObject = null;

            // Act
            theoryItem.SetupConfig(
                (
                    builder,
                    provider) =>
                {
                    builder.ConfigureServices(
                        (
                            context,
                            collection) =>
                        {
                            collection.Add(arrangeDescriptor);
                        });
                });
            theoryItem.SetupCheck(
                host =>
                {
                    expectedObject = host.Services.GetService<ITestDependency>();
                });

            theoryItem.SetupExtension(new PickDependencyTheoryExtension().Setup(typeof(ITestDependency), o => actualObject = o));
            theoryItem.Try();

            // Assert
            Assert.Same(expectedObject, actualObject);
        }

        [Theory]
        [MemberData(nameof(HierarchyServiceRegistration.GeneralCases), MemberType = typeof(HierarchyServiceRegistration))]
        private static void Should_resolve_same_singleton_instance_From_hierarchy_singleton_type_registration(
            HierarchyServiceRegistration.Case @case)
        {
            // Arrange
            var theoryItem = @case.Promise.Resolve();

            var arrangeDescriptor = new ServiceDescriptor(typeof(ITestDependency), typeof(TestDependency), ServiceLifetime.Singleton);

            object expectedObject = null;
            object actualObject = null;

            // Act
            theoryItem.SetupConfig(
                (
                    builder,
                    provider) =>
                {
                    builder.ConfigureServices(
                        (
                            context,
                            collection) =>
                        {
                            collection.Add(arrangeDescriptor);
                        });
                });
            theoryItem.SetupCheck(
                host =>
                {
                    expectedObject = host.Services.GetService<ITestDependency>();
                });

            theoryItem.SetupExtension(new PickDependencyTheoryExtension().Setup(typeof(ITestDependency), o => actualObject = o));
            theoryItem.Try();

            // Assert
            Assert.Same(expectedObject, actualObject);
        }

        [Theory]
        [MemberData(nameof(HierarchyServiceRegistration.GeneralCases), MemberType = typeof(HierarchyServiceRegistration))]
        private static void Should_resolve_different_transient_instance_From_hierarchy_transient_type_registration(
            HierarchyServiceRegistration.Case @case)
        {
            // Arrange
            var theoryItem = @case.Promise.Resolve();

            var arrangeDescriptor = new ServiceDescriptor(typeof(ITestDependency), typeof(TestDependency), ServiceLifetime.Transient);

            object expectedObject = null;
            object actualObject = null;

            // Act
            theoryItem.SetupConfig(
                (
                    builder,
                    provider) =>
                {
                    builder.ConfigureServices(
                        (
                            context,
                            collection) =>
                        {
                            collection.Add(arrangeDescriptor);
                        });
                });
            theoryItem.SetupCheck(
                host =>
                {
                    expectedObject = host.Services.GetService<ITestDependency>();
                });

            theoryItem.SetupExtension(new PickDependencyTheoryExtension().Setup(typeof(ITestDependency), o => actualObject = o));
            theoryItem.Try();

            // Assert
            Assert.NotSame(expectedObject, actualObject);
        }

        [Theory]
        [MemberData(nameof(HierarchyServiceRegistration.OpenGenericCases), MemberType = typeof(HierarchyServiceRegistration))]
        private static void Should_resolve_same_open_generic_singleton_instance_From_hierarchy_open_generic_singleton_type_registration(
            HierarchyServiceRegistration.OpenGenericCase @case)
        {
            // Arrange
            var theoryItem = @case.Promise.Resolve();

            var arrangeDescriptor = new ServiceDescriptor(
                @case.ServiceType,
                @case.ImplementationType,
                ServiceLifetime.Singleton);

            object expectedObject = null;
            object actualObject = null;

            // Act
            theoryItem.SetupConfig(
                (
                    builder,
                    provider) =>
                {
                    builder.ConfigureServices(
                        (
                            context,
                            collection) =>
                        {
                            collection.Add(arrangeDescriptor);
                        });
                });
            theoryItem.SetupCheck(
                host =>
                {
                    expectedObject = host.Services.GetService(@case.RequestType);
                });

            theoryItem.SetupExtension(new PickDependencyTheoryExtension().Setup(@case.RequestType, o => actualObject = o));
            theoryItem.Try();

            // Assert
            Assert.Same(expectedObject, actualObject);
        }

        [Theory]
        [MemberData(nameof(HierarchyServiceRegistration.OpenGenericCases), MemberType = typeof(HierarchyServiceRegistration))]
        private static void Should_resolve_different_open_generic_transient_instance_From_hierarchy_open_generic_transient_type_registration(
            HierarchyServiceRegistration.OpenGenericCase @case)
        {
            // Arrange
            var theoryItem = @case.Promise.Resolve();

            var arrangeDescriptor = new ServiceDescriptor(
                @case.ServiceType,
                @case.ImplementationType,
                ServiceLifetime.Transient);
            ;

            object expectedObject = null;
            object actualObject = null;

            // Act
            theoryItem.SetupConfig(
                (
                    builder,
                    provider) =>
                {
                    builder.ConfigureServices(
                        (
                            context,
                            collection) =>
                        {
                            collection.Add(arrangeDescriptor);
                        });
                });
            theoryItem.SetupCheck(
                host =>
                {
                    expectedObject = host.Services.GetService(@case.RequestType);
                });

            theoryItem.SetupExtension(new PickDependencyTheoryExtension().Setup(@case.RequestType, o => actualObject = o));
            theoryItem.Try();

            // Assert
            Assert.NotSame(expectedObject, actualObject);
        }
    }
}