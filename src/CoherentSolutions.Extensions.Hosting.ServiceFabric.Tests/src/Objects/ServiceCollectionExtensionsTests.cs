using System;
using System.Collections.Generic;
using System.Linq;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Tools;

using Microsoft.Extensions.DependencyInjection;

using Xunit;
using Xunit.Abstractions;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Objects
{
    public static class ServiceCollectionExtensionsTests
    {
        public interface ITestInterface
        {
        }
        
        public interface ITestGenericInterface<T>
        {
        }

        private class TestVariant : ITestInterface
        {
        }
        
        private class TestVariantOne : ITestInterface
        {
        }

        private class TestVariantTwo : ITestInterface
        {
        }

        private class TestGenericVariant<T> : ITestGenericInterface<T>
        {
        }

        private class TestGenericVariantOne<T> : ITestGenericInterface<T>
        {
        }

        private class TestGenericVariantTwo<T> : ITestGenericInterface<T>
        {
        }

        private static class Cases
        {
            public class Case : IXunitSerializable
            {
                public Type ServiceType { get; private set; }

                public Type RequestType { get; private set; }
                
                public Type[] ImplementationTypes { get; private set; }

                public Case()
                {
                }

                public Case(
                    Type serviceType,
                    Type requestType,
                    params Type[] implementationType)
                {
                    this.ServiceType = serviceType;
                    this.RequestType = requestType;
                    this.ImplementationTypes = implementationType;
                }

                public override string ToString()
                {
                    return string.Join(",", this.ImplementationTypes.Select(i => i.Name));
                }

                public void Deserialize(IXunitSerializationInfo info)
                {
                    this.ServiceType = info.GetValue<Type>(nameof(ServiceType));
                    this.RequestType = info.GetValue<Type>(nameof(RequestType));
                    this.ImplementationTypes = info.GetValue<Type[]>(nameof(ImplementationTypes));
                }

                public void Serialize(IXunitSerializationInfo info)
                {
                    info.AddValue(nameof(this.ServiceType), this.ServiceType);
                    info.AddValue(nameof(this.RequestType), this.RequestType);
                    info.AddValue(nameof(this.ImplementationTypes), this.ImplementationTypes);
                }
            }

            public static IEnumerable<object[]> EmitTypeCases
            {
                get
                {
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestInterface),
                            typeof(ITestInterface),
                            typeof(TestVariant)),
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestInterface),
                            typeof(ITestInterface),
                            typeof(TestVariantOne), typeof(TestVariantTwo)),
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestGenericInterface<>),
                            typeof(ITestGenericInterface<int>),
                            typeof(TestGenericVariant<>)),
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestGenericInterface<>),
                            typeof(ITestGenericInterface<int>),
                            typeof(TestGenericVariantOne<>), typeof(TestGenericVariantTwo<>)),
                    };
                }
            }
        }

        [Theory]
        [MemberData(nameof(Cases.EmitTypeCases), MemberType = typeof(Cases))]
        private static void Should_proxinate_interface_types_and_keep_interface_to_type_mapping(
            Cases.Case @case)
        {
            // Arrange
            var arrangeRootCollection = (IServiceCollection) new ServiceCollection();
            foreach (var implementationType in @case.ImplementationTypes)
            {
                arrangeRootCollection.Add(new ServiceDescriptor(
                    @case.ServiceType, 
                    implementationType, 
                    ServiceLifetime.Singleton));
            }
            
            var arrangeRootServices = (IServiceProvider) arrangeRootCollection.BuildServiceProvider();

            // Act
            var arrangeCollection = (IServiceCollection) new ServiceCollection();
            arrangeCollection.Proxinate(arrangeRootCollection, arrangeRootServices);
            
            var arrangeServices = (IServiceProvider) arrangeCollection.BuildServiceProvider();

            var proxies = arrangeServices.GetServices(@case.RequestType)
               .Cast<IProxynatorProxy>()
               .ToArray();

            // Assert
            foreach (var implementationType in @case.ImplementationTypes)
            {
                Assert.Contains(proxies, proxy =>
                {
                    var t = proxy.Target.GetType();
                    if (t.IsGenericType)
                    {
                        return t.GetGenericTypeDefinition() == implementationType;
                    }
                    return t == implementationType;
                });
            }
        }
    }
}