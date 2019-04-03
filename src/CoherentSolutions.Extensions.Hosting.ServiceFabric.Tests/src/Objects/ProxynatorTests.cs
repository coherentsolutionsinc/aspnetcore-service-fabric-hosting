using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Proxynator;

using Microsoft.Extensions.DependencyInjection;

using Xunit;
using Xunit.Abstractions;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Objects
{
    public static class ProxynatorTests
    {
        public interface ITestServiceProvider : IServiceProvider
        {
        }

        public class TestServiceProvider : ITestServiceProvider
        {
            private readonly IServiceProvider impl;

            public TestServiceProvider(
                IServiceProvider impl)
            {
                this.impl = impl;
            }

            public object GetService(
                Type serviceType)
            {
                return this.impl.GetService(serviceType);
            }
        }

        public interface ITestType
        {
        }

        public interface ITestTypeWithGetProperty
        {
            int Value { get; }
        }

        public interface ITestTypeWithInheritedProperty : ITestTypeWithGetProperty
        {
        }

        public interface ITestTypeWithRefGetProperty
        {
            ref int Value { get; }
        }

        public interface ITestTypeWithGetSetProperty
        {
            int Value { get; set; }
        }

        public interface ITestTypeWithEvent
        {
            event EventHandler<EventArgs> Event;
        }

        public interface ITestTypeWithInheritedEvent : ITestTypeWithEvent
        {
        }

        public interface ITestTypeWithNonVoidMethodVoidParameters
        {
            int Method();
        }

        public interface ITestTypeWithInheritedNonVoidMethodVoidParameters : ITestTypeWithNonVoidMethodVoidParameters
        {
        }

        public interface ITestTypeWithVoidMethodVoidParameters
        {
            void Method();
        }

        public interface ITestTypeWithVoidMethodWithParameters
        {
            void Method(
                int parameter);
        }

        public interface ITestTypeWithVoidMethodWithRefParameters
        {
            void Method(
                ref int parameter);
        }

        public interface ITestTypeWithVoidMethodWithOutParameters
        {
            void Method(
                out int parameter);
        }

        public interface ITestGenericType
        {
        }

        public interface ITestGenericType<T>
        {
        }

        public interface ITestGenericTypeWithOutModifier<out T>
        {
        }

        public interface ITestGenericTypeWithInModifier<in T>
        {
        }

        public interface ITestGenericTypeWithGenericGetProperty<T>
        {
            T Value { get; }
        }

        public interface ITestGenericTypeWithGenericGetSetProperty<T>
        {
            T Value { get; set; }
        }

        public interface ITestGenericTypeWithGenericReturnMethodVoidParameters<T>
        {
            T Method();
        }

        public interface ITestGenericTypeWithVoidMethodWithGenericParameters<T>
        {
            void Method(
                T parameter);
        }

        public interface ITestGenericTypeWithVoidMethodWithGenericGenericParameters<T>
        {
            void Method(
                Action<T> parameter);
        }

        public interface ITestGenericTypeWithIndependentGenericReturnMethodVoidParameters<T>
        {
            K Method<K>();
        }

        public interface ITestGenericTypeWithVoidMethodWithIndependentGenericParameters<T>
        {
            void Method<K>(
                K parameter);
        }

        public interface ITestGenericTypeWithVoidMethodWithIndependentGenericParametersWithClassConstraints<T>
        {
            void Method<K>(
                K parameter)
                where K : class;
        }

        public interface ITestGenericTypeWithVoidMethodWithIndependentGenericParametersWithStructConstraints<T>
        {
            void Method<K>(
                K parameter)
                where K : struct;
        }

        public interface ITestGenericTypeWithVoidMethodWithIndependentGenericParametersWithNewConstraints<T>
        {
            void Method<K>(
                K parameter)
                where K : new();
        }

        public interface ITestGenericTypeWithVoidMethodWithIndependentGenericParametersWithBaseTypeConstraints<T>
        {
            void Method<K>(
                K parameter)
                where K : TestGenericType;
        }

        public interface ITestGenericTypeWithVoidMethodWithIndependentGenericParametersWithBaseInterfaceConstraints<T>
        {
            void Method<K>(
                K parameter)
                where K : ITestGenericType;
        }

        public interface ITestGenericTypeWithVoidMethodWithIndependentGenericParametersWithGenericInterfaceConstraints<T>
        {
            void Method<K, X>(
                K parameter)
                where K : ITestGenericType<X>;
        }

        public interface ITestGenericTypeWithMultipleParameters<T, K>
        {
        }

        public interface ITestGenericTypeWithMultipleParametersAndDifferentConstraints<T, K>
            where T : class
            where K : struct
        {
        }

        public interface ITestGenericTypeWithClassConstraints<T>
            where T : class
        {
        }

        public interface ITestGenericTypeWithStructConstraints<T>
            where T : struct
        {
        }

        public interface ITestGenericTypeWithNewConstraints<T>
            where T : new()
        {
        }

        public interface ITestGenericTypeWithBaseTypeConstraints<T>
            where T : TestGenericType
        {
        }

        public interface ITestGenericTypeWithInterfaceConstraints<T>
            where T : ITestGenericType
        {
        }

        public interface ITestGenericTypeWithGenericInterfaceConstraint<T, K>
            where T : ITestGenericType<K>
        {
        }

        public class TestGenericType
        {
        }

        public static class Cases
        {
            public class Case : IXunitSerializable
            {
                public Type ServiceType { get; private set; }

                public Type ImplementationType { get; private set; }

                public Type RequestType { get; private set; }

                public Case()
                {
                }

                public Case(
                    Type serviceType,
                    Type implementationType,
                    Type requestType)
                {
                    this.ServiceType = serviceType;
                    this.ImplementationType = implementationType;
                    this.RequestType = requestType;
                }

                public override string ToString()
                {
                    return this.ImplementationType.Name;
                }

                public void Deserialize(
                    IXunitSerializationInfo info)
                {
                    this.ServiceType = info.GetValue<Type>(nameof(this.ServiceType));
                    this.ImplementationType = info.GetValue<Type>(nameof(this.ImplementationType));
                    this.RequestType = info.GetValue<Type>(nameof(this.RequestType));
                }

                public void Serialize(
                    IXunitSerializationInfo info)
                {
                    info.AddValue(nameof(this.ServiceType), this.ServiceType);
                    info.AddValue(nameof(this.ImplementationType), this.ImplementationType);
                    info.AddValue(nameof(this.RequestType), this.RequestType);
                }
            }

            public static IEnumerable<object[]> EmitTypeCases
            {
                get
                {
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestType),
                            typeof(TestType),
                            typeof(ITestType))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestTypeWithGetProperty),
                            typeof(TestTypeWithGetProperty),
                            typeof(ITestTypeWithGetProperty))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestTypeWithInheritedProperty),
                            typeof(TestTypeWithInheritedProperty),
                            typeof(ITestTypeWithInheritedProperty))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestTypeWithRefGetProperty),
                            typeof(TestTypeWithRefGetProperty),
                            typeof(ITestTypeWithRefGetProperty))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestTypeWithGetSetProperty),
                            typeof(TestTypeWithGetSetProperty),
                            typeof(ITestTypeWithGetSetProperty))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestTypeWithEvent),
                            typeof(TestTypeWithEvent),
                            typeof(ITestTypeWithEvent))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestTypeWithInheritedEvent),
                            typeof(TestTypeWithInheritedEvent),
                            typeof(ITestTypeWithInheritedEvent))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestTypeWithNonVoidMethodVoidParameters),
                            typeof(TestTypeWithNonVoidMethodVoidParameters),
                            typeof(ITestTypeWithNonVoidMethodVoidParameters))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestTypeWithInheritedNonVoidMethodVoidParameters),
                            typeof(TestTypeWithInheritedNonVoidMethodVoidParameters),
                            typeof(ITestTypeWithInheritedNonVoidMethodVoidParameters))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestTypeWithVoidMethodVoidParameters),
                            typeof(TestTypeWithVoidMethodVoidParameters),
                            typeof(ITestTypeWithVoidMethodVoidParameters))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestTypeWithVoidMethodWithParameters),
                            typeof(TestTypeWithVoidMethodWithParameters),
                            typeof(ITestTypeWithVoidMethodWithParameters))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestTypeWithVoidMethodWithRefParameters),
                            typeof(TestTypeWithVoidMethodWithRefParameters),
                            typeof(ITestTypeWithVoidMethodWithRefParameters))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestTypeWithVoidMethodWithOutParameters),
                            typeof(TestTypeWithVoidMethodWithOutParameters),
                            typeof(ITestTypeWithVoidMethodWithOutParameters))
                    };
                }
            }

            public static IEnumerable<object[]> EmitGenericTypeCases
            {
                get
                {
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestGenericType<>),
                            typeof(TestGenericType<>),
                            typeof(ITestGenericType<int>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestGenericTypeWithOutModifier<>),
                            typeof(TestGenericTypeWithOutModifier<>),
                            typeof(ITestGenericTypeWithOutModifier<int>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestGenericTypeWithInModifier<>),
                            typeof(TestGenericTypeWithInModifier<>),
                            typeof(ITestGenericTypeWithInModifier<int>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestGenericTypeWithGenericGetProperty<>),
                            typeof(TestGenericTypeWithGenericGetProperty<>),
                            typeof(ITestGenericTypeWithGenericGetProperty<int>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestGenericTypeWithGenericGetSetProperty<>),
                            typeof(TestGenericTypeWithGenericGetSetProperty<>),
                            typeof(ITestGenericTypeWithGenericGetSetProperty<int>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestGenericTypeWithVoidMethodWithGenericParameters<>),
                            typeof(TestGenericTypeWithVoidMethodWithGenericParameters<>),
                            typeof(ITestGenericTypeWithVoidMethodWithGenericParameters<int>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestGenericTypeWithVoidMethodWithGenericGenericParameters<>),
                            typeof(TestGenericTypeWithVoidMethodWithGenericGenericParameters<>),
                            typeof(ITestGenericTypeWithVoidMethodWithGenericGenericParameters<int>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestGenericTypeWithGenericReturnMethodVoidParameters<>),
                            typeof(TestGenericTypeWithGenericReturnMethodVoidParameters<>),
                            typeof(ITestGenericTypeWithGenericReturnMethodVoidParameters<int>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestGenericTypeWithVoidMethodWithIndependentGenericParameters<>),
                            typeof(TestGenericTypeWithVoidMethodWithIndependentGenericParameters<>),
                            typeof(ITestGenericTypeWithVoidMethodWithIndependentGenericParameters<int>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestGenericTypeWithIndependentGenericReturnMethodVoidParameters<>),
                            typeof(TestGenericTypeWithIndependentGenericReturnMethodVoidParameters<>),
                            typeof(ITestGenericTypeWithIndependentGenericReturnMethodVoidParameters<int>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestGenericTypeWithVoidMethodWithIndependentGenericParametersWithClassConstraints<>),
                            typeof(TestGenericTypeWithVoidMethodWithIndependentGenericParametersWithClassConstraints<>),
                            typeof(ITestGenericTypeWithVoidMethodWithIndependentGenericParametersWithClassConstraints<int>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestGenericTypeWithVoidMethodWithIndependentGenericParametersWithStructConstraints<>),
                            typeof(TestGenericTypeWithVoidMethodWithIndependentGenericParametersWithStructConstraints<>),
                            typeof(ITestGenericTypeWithVoidMethodWithIndependentGenericParametersWithStructConstraints<int>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestGenericTypeWithVoidMethodWithIndependentGenericParametersWithNewConstraints<>),
                            typeof(TestGenericTypeWithVoidMethodWithIndependentGenericParametersWithNewConstraints<>),
                            typeof(ITestGenericTypeWithVoidMethodWithIndependentGenericParametersWithNewConstraints<int>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestGenericTypeWithVoidMethodWithIndependentGenericParametersWithBaseTypeConstraints<>),
                            typeof(TestGenericTypeWithVoidMethodWithIndependentGenericParametersWithBaseTypeConstraints<>),
                            typeof(ITestGenericTypeWithVoidMethodWithIndependentGenericParametersWithBaseTypeConstraints<int>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestGenericTypeWithVoidMethodWithIndependentGenericParametersWithBaseInterfaceConstraints<>),
                            typeof(TestGenericTypeWithVoidMethodWithIndependentGenericParametersWithBaseInterfaceConstraints<>),
                            typeof(ITestGenericTypeWithVoidMethodWithIndependentGenericParametersWithBaseInterfaceConstraints<int>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestGenericTypeWithVoidMethodWithIndependentGenericParametersWithGenericInterfaceConstraints<>),
                            typeof(TestGenericTypeWithVoidMethodWithIndependentGenericParametersWithGenericInterfaceConstraints<>),
                            typeof(ITestGenericTypeWithVoidMethodWithIndependentGenericParametersWithGenericInterfaceConstraints<int>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestGenericTypeWithMultipleParameters<,>),
                            typeof(TestGenericTypeWithMultipleParameters<,>),
                            typeof(ITestGenericTypeWithMultipleParameters<int, int>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestGenericTypeWithMultipleParametersAndDifferentConstraints<,>),
                            typeof(TestGenericTypeWithMultipleParametersAndDifferentConstraints<,>),
                            typeof(ITestGenericTypeWithMultipleParametersAndDifferentConstraints<object, int>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestGenericTypeWithClassConstraints<>),
                            typeof(TestGenericTypeWithClassConstraints<>),
                            typeof(ITestGenericTypeWithClassConstraints<object>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestGenericTypeWithStructConstraints<>),
                            typeof(TestGenericTypeWithStructConstraints<>),
                            typeof(ITestGenericTypeWithStructConstraints<int>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestGenericTypeWithNewConstraints<>),
                            typeof(TestGenericTypeWithNewConstraints<>),
                            typeof(ITestGenericTypeWithNewConstraints<int>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestGenericTypeWithBaseTypeConstraints<>),
                            typeof(TestGenericTypeWithBaseClassConstraints<>),
                            typeof(ITestGenericTypeWithBaseTypeConstraints<TestGenericType>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestGenericTypeWithInterfaceConstraints<>),
                            typeof(TestGenericTypeWithInterfaceConstraints<>),
                            typeof(ITestGenericTypeWithInterfaceConstraints<ITestGenericType>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestGenericTypeWithGenericInterfaceConstraint<,>),
                            typeof(TestGenericTypeWithGenericInterfaceConstraint<,>),
                            typeof(ITestGenericTypeWithGenericInterfaceConstraint<ITestGenericType<int>, int>))
                    };
                }
            }
        }

        private class TestType : ITestType
        {
        }

        private class TestTypeWithGetProperty : ITestTypeWithGetProperty
        {
            public int Value => throw new NotImplementedException();
        }

        private class TestTypeWithInheritedProperty : ITestTypeWithInheritedProperty
        {
            public int Value => throw new NotImplementedException();
        }

        private class TestTypeWithRefGetProperty : ITestTypeWithRefGetProperty
        {
            public ref int Value => throw new NotImplementedException();
        }

        private class TestTypeWithGetSetProperty : ITestTypeWithGetSetProperty
        {
            public int Value
            {
                get => throw new NotImplementedException();
                set => throw new NotImplementedException();
            }
        }

        private class TestTypeWithEvent : ITestTypeWithEvent
        {
            public event EventHandler<EventArgs> Event
            {
                add => throw new NotImplementedException();
                remove => throw new NotImplementedException();
            }
        }

        private class TestTypeWithInheritedEvent : ITestTypeWithInheritedEvent
        {
            public event EventHandler<EventArgs> Event
            {
                add => throw new NotImplementedException();
                remove => throw new NotImplementedException();
            }
        }

        private class TestTypeWithNonVoidMethodVoidParameters : ITestTypeWithNonVoidMethodVoidParameters
        {
            public int Method()
            {
                throw new NotImplementedException();
            }
        }

        private class TestTypeWithInheritedNonVoidMethodVoidParameters : ITestTypeWithInheritedNonVoidMethodVoidParameters
        {
            public int Method()
            {
                throw new NotImplementedException();
            }
        }

        private class TestTypeWithVoidMethodVoidParameters : ITestTypeWithVoidMethodVoidParameters
        {
            public void Method()
            {
                throw new NotImplementedException();
            }
        }

        private class TestTypeWithVoidMethodWithParameters : ITestTypeWithVoidMethodWithParameters
        {
            public void Method(
                int parameter)
            {
                throw new NotImplementedException();
            }
        }

        private class TestTypeWithVoidMethodWithRefParameters : ITestTypeWithVoidMethodWithRefParameters
        {
            public void Method(
                ref int parameter)
            {
                throw new NotImplementedException();
            }
        }

        private class TestTypeWithVoidMethodWithOutParameters : ITestTypeWithVoidMethodWithOutParameters
        {
            public void Method(
                out int parameter)
            {
                throw new NotImplementedException();
            }
        }

        private class TestGenericType<T> : ITestGenericType<T>
        {
        }

        private class TestGenericTypeWithMultipleParameters<T, K> : ITestGenericTypeWithMultipleParameters<T, K>
        {
        }

        private class TestGenericTypeWithMultipleParametersAndDifferentConstraints<T, K> : ITestGenericTypeWithMultipleParametersAndDifferentConstraints<T, K>
            where T : class
            where K : struct
        {
        }

        private class TestGenericTypeWithClassConstraints<T> : ITestGenericTypeWithClassConstraints<T>
            where T : class
        {
        }

        private class TestGenericTypeWithStructConstraints<T> : ITestGenericTypeWithStructConstraints<T>
            where T : struct
        {
        }

        private class TestGenericTypeWithBaseClassConstraints<T> : ITestGenericTypeWithBaseTypeConstraints<T>
            where T : TestGenericType
        {
        }

        private class TestGenericTypeWithInterfaceConstraints<T> : ITestGenericTypeWithInterfaceConstraints<T>
            where T : ITestGenericType
        {
        }

        private class TestGenericTypeWithGenericInterfaceConstraint<T, K> : ITestGenericTypeWithGenericInterfaceConstraint<T, K>
            where T : ITestGenericType<K>
        {
        }

        private class TestGenericTypeWithOutModifier<T> : ITestGenericTypeWithOutModifier<T>
        {
        }

        private class TestGenericTypeWithInModifier<T> : ITestGenericTypeWithInModifier<T>
        {
        }

        private class TestGenericTypeWithGenericGetProperty<T> : ITestGenericTypeWithGenericGetProperty<T>
        {
            public T Value => throw new NotImplementedException();
        }

        private class TestGenericTypeWithGenericGetSetProperty<T> : ITestGenericTypeWithGenericGetSetProperty<T>
        {
            public T Value
            {
                get => throw new NotImplementedException();
                set => throw new NotImplementedException();
            }
        }

        private class TestGenericTypeWithVoidMethodWithGenericParameters<T> : ITestGenericTypeWithVoidMethodWithGenericParameters<T>
        {
            public void Method(
                T parameter)
            {
                throw new NotImplementedException();
            }
        }

        private class TestGenericTypeWithVoidMethodWithGenericGenericParameters<T> : ITestGenericTypeWithVoidMethodWithGenericGenericParameters<T>
        {
            public void Method(
                Action<T> parameter)
            {
                throw new NotImplementedException();
            }
        }

        private class TestGenericTypeWithGenericReturnMethodVoidParameters<T> : ITestGenericTypeWithGenericReturnMethodVoidParameters<T>
        {
            public T Method()
            {
                throw new NotImplementedException();
            }
        }

        private class TestGenericTypeWithIndependentGenericReturnMethodVoidParameters<T> : ITestGenericTypeWithIndependentGenericReturnMethodVoidParameters<T>
        {
            public K Method<K>()
            {
                throw new NotImplementedException();
            }
        }

        private class TestGenericTypeWithVoidMethodWithIndependentGenericParameters<T> : ITestGenericTypeWithVoidMethodWithIndependentGenericParameters<T>
        {
            public void Method<K>(
                K parameter)
            {
                throw new NotImplementedException();
            }
        }

        private class TestGenericTypeWithVoidMethodWithIndependentGenericParametersWithClassConstraints<T>
            : ITestGenericTypeWithVoidMethodWithIndependentGenericParametersWithClassConstraints<T>
        {
            public void Method<K>(
                K parameter)
                where K : class
            {
                throw new NotImplementedException();
            }
        }

        private class TestGenericTypeWithVoidMethodWithIndependentGenericParametersWithStructConstraints<T>
            : ITestGenericTypeWithVoidMethodWithIndependentGenericParametersWithStructConstraints<T>
        {
            public void Method<K>(
                K parameter)
                where K : struct
            {
                throw new NotImplementedException();
            }
        }

        private class TestGenericTypeWithVoidMethodWithIndependentGenericParametersWithNewConstraints<T>
            : ITestGenericTypeWithVoidMethodWithIndependentGenericParametersWithNewConstraints<T>
        {
            public void Method<K>(
                K parameter)
                where K : new()
            {
                throw new NotImplementedException();
            }
        }

        private class TestGenericTypeWithVoidMethodWithIndependentGenericParametersWithBaseTypeConstraints<T>
            : ITestGenericTypeWithVoidMethodWithIndependentGenericParametersWithBaseTypeConstraints<T>
        {
            public void Method<K>(
                K parameter)
                where K : TestGenericType
            {
                throw new NotImplementedException();
            }
        }

        private class TestGenericTypeWithVoidMethodWithIndependentGenericParametersWithBaseInterfaceConstraints<T>
            : ITestGenericTypeWithVoidMethodWithIndependentGenericParametersWithBaseInterfaceConstraints<T>
        {
            public void Method<K>(
                K parameter)
                where K : ITestGenericType
            {
                throw new NotImplementedException();
            }
        }

        private class TestGenericTypeWithVoidMethodWithIndependentGenericParametersWithGenericInterfaceConstraints<T>
            : ITestGenericTypeWithVoidMethodWithIndependentGenericParametersWithGenericInterfaceConstraints<T>
        {
            public void Method<K, X>(
                K parameter)
                where K : ITestGenericType<X>
            {
                throw new NotImplementedException();
            }
        }

        private class TestGenericTypeWithNewConstraints<T> : ITestGenericTypeWithNewConstraints<T>
            where T : new()
        {
        }

        [Theory]
        [MemberData(nameof(Cases.EmitTypeCases), MemberType = typeof(Cases))]
        [MemberData(nameof(Cases.EmitGenericTypeCases), MemberType = typeof(Cases))]
        public static void Should_generate_instance_proxy_with_dependency_injection_target_resolution_For_simple_type(
            Cases.Case @case)
        {
            // Arrange
            var implType = @case.ImplementationType;
            var proxyType = Proxynator.CreateInstanceProxy(@case.ServiceType);

            if (@case.ServiceType.IsGenericTypeDefinition)
            {
                implType = @case.ImplementationType.MakeGenericType(@case.RequestType.GetGenericArguments());
                proxyType = proxyType.MakeGenericType(@case.RequestType.GetGenericArguments());
            }

            // Act
            var source = Activator.CreateInstance(implType);

            var ctor = proxyType.GetConstructor(
                new[]
                {
                    @case.RequestType
                });

            var proxy = ctor.Invoke(
                new object[]
                {
                    source
                });

            // Assert
            foreach (var mi in proxyType.GetInterfaceMap(@case.RequestType).TargetMethods)
            {
                var method = mi;

                if (method.IsGenericMethodDefinition)
                {
                    // TODO: Implement generic argument resolution mechanism
                    continue;
                }

                var parameters = method.GetParameters();
                var values = parameters
                   .Select(
                        pi =>
                        {
                            var t = pi.ParameterType;
                            if (t.IsByRef)
                            {
                                t = t.GetElementType();
                            }

                            return t.IsValueType
                                ? Activator.CreateInstance(t)
                                : null;
                        })
                   .ToArray();

                Assert.Throws<NotImplementedException>(
                    () =>
                    {
                        var action = Expression.Lambda<Action>(
                                Expression.Call(
                                    Expression.Constant(proxy),
                                    method,
                                    values.Select<object, Expression>(
                                        (
                                            v,
                                            i) =>
                                        {
                                            var t = parameters[i].ParameterType;
                                            if (t.IsByRef)
                                            {
                                                t = t.GetElementType();
                                            }

                                            return Expression.Convert(Expression.Constant(v), t);
                                        })))
                           .Compile();

                        action();
                    });
            }
        }

        [Theory]
        [MemberData(nameof(Cases.EmitTypeCases), MemberType = typeof(Cases))]
        [MemberData(nameof(Cases.EmitGenericTypeCases), MemberType = typeof(Cases))]
        public static void Should_generate_dependency_injection_proxy_with_dependency_injection_target_resolution_For_open_generic_type(
            Cases.Case @case)
        {
            // Arrange
            var arrangeCollection = (IServiceCollection) new ServiceCollection();
            arrangeCollection.Add(new ServiceDescriptor(@case.ServiceType, @case.ImplementationType, ServiceLifetime.Singleton));

            var arrangeServices = (IServiceProvider) arrangeCollection.BuildServiceProvider();
            arrangeServices = new TestServiceProvider(arrangeServices);

            var proxyType = Proxynator.CreateDependencyInjectionProxy(typeof(ITestServiceProvider), @case.ServiceType, @case.ImplementationType);
            if (@case.ServiceType.IsGenericTypeDefinition)
            {
                proxyType = proxyType.MakeGenericType(@case.RequestType.GetGenericArguments());
            }

            // Act
            var ctor = proxyType.GetConstructor(
                new[]
                {
                    typeof(ITestServiceProvider)
                });

            var proxy = ctor.Invoke(
                new object[]
                {
                    arrangeServices
                });

            // Assert
            foreach (var mi in proxyType.GetInterfaceMap(@case.RequestType).TargetMethods)
            {
                var method = mi;

                if (method.IsGenericMethodDefinition)
                {
                    // TODO: Implement generic argument resolution mechanism
                    continue;
                }

                var parameters = method.GetParameters();
                var values = parameters
                   .Select(
                        pi =>
                        {
                            var t = pi.ParameterType;
                            if (t.IsByRef)
                            {
                                t = t.GetElementType();
                            }

                            return t.IsValueType
                                ? Activator.CreateInstance(t)
                                : null;
                        })
                   .ToArray();

                Assert.Throws<NotImplementedException>(
                    () =>
                    {
                        var action = Expression.Lambda<Action>(
                                Expression.Call(
                                    Expression.Constant(proxy),
                                    method,
                                    values.Select<object, Expression>(
                                        (
                                            v,
                                            i) =>
                                        {
                                            var t = parameters[i].ParameterType;
                                            if (t.IsByRef)
                                            {
                                                t = t.GetElementType();
                                            }

                                            return Expression.Convert(Expression.Constant(v), t);
                                        })))
                           .Compile();

                        action();
                    });
            }
        }
    }
}