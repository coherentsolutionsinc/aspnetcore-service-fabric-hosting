using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Tools;

using Microsoft.Extensions.DependencyInjection;

using Xunit;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Objects
{
    public static class OpenGenericEmitTests
    {
        public interface ITestOpenGeneric
        {
        }

        public interface ITestOpenGeneric<T>
        {
        }

        public interface ITestOpenGenericWithOutModifier<out T>
        {
        }

        public interface ITestOpenGenericWithInModifier<in T>
        {
        }

        public interface ITestOpenGenericWithGetProperty<T>
        {
            int Value { get; }
        }

        public interface ITestOpenGenericWithRefGetProperty<T>
        {
            ref int Value { get; }
        }

        public interface ITestOpenGenericWithGetSetProperty<T>
        {
            int Value { get; set; }
        }

        public interface ITestOpenGenericWithNonVoidMethodVoidParameters<T>
        {
            int Method();
        }

        public interface ITestOpenGenericWithVoidMethodVoidParameters<T>
        {
            void Method();
        }

        public interface ITestOpenGenericWithVoidMethodWithParameters<T>
        {
            void Method(
                int parameter);
        }

        public interface ITestOpenGenericWithVoidMethodWithRefParameters<T>
        {
            void Method(
                ref int parameter);
        }

        public interface ITestOpenGenericWithVoidMethodWithOutParameters<T>
        {
            void Method(
                out int parameter);
        }

        //TODO: Method parameter modifiers, in etc.

        public interface ITestOpenGenericWithGenericGetProperty<T>
        {
            T Value { get; }
        }

        public interface ITestOpenGenericWithGenericGetSetProperty<T>
        {
            T Value { get; set; }
        }

        public interface ITestOpenGenericWithGenericReturnMethodVoidParameters<T>
        {
            T Method();
        }

        public interface ITestOpenGenericWithVoidMethodWithGenericParameters<T>
        {
            void Method(
                T parameter);
        }

        public interface ITestOpenGenericWithVoidMethodWithGenericGenericParameters<T>
        {
            void Method(
                Action<T> parameter);
        }

        public interface ITestOpenGenericWithIndependentGenericReturnMethodVoidParameters<T>
        {
            K Method<K>();
        }

        public interface ITestOpenGenericWithVoidMethodWithIndependentGenericParameters<T>
        {
            void Method<K>(
                K parameter);
        }

        public interface ITestOpenGenericWithVoidMethodWithIndependentGenericParametersWithClassConstraints<T>
        {
            void Method<K>(
                K parameter)
                where K : class;
        }

        public interface ITestOpenGenericWithVoidMethodWithIndependentGenericParametersWithStructConstraints<T>
        {
            void Method<K>(
                K parameter)
                where K : struct;
        }

        public interface ITestOpenGenericWithVoidMethodWithIndependentGenericParametersWithNewConstraints<T>
        {
            void Method<K>(
                K parameter)
                where K : new();
        }

        public interface ITestOpenGenericWithVoidMethodWithIndependentGenericParametersWithBaseTypeConstraints<T>
        {
            void Method<K>(
                K parameter)
                where K : TestOpenGeneric;
        }

        public interface ITestOpenGenericWithVoidMethodWithIndependentGenericParametersWithBaseInterfaceConstraints<T>
        {
            void Method<K>(
                K parameter)
                where K : ITestOpenGeneric;
        }

        public interface ITestOpenGenericWithVoidMethodWithIndependentGenericParametersWithGenericInterfaceConstraints<T>
        {
            void Method<K, X>(
                K parameter)
                where K : ITestOpenGeneric<X>;
        }

        public interface ITestOpenGenericWithMultipleParameters<T, K>
        {
        }

        public interface ITestOpenGenericWithMultipleParametersAndDifferentConstraints<T, K>
            where T : class
            where K : struct
        {
        }

        public interface ITestOpenGenericWithClassConstraints<T>
            where T : class
        {
        }

        public interface ITestOpenGenericWithStructConstraints<T>
            where T : struct
        {
        }

        public interface ITestOpenGenericWithNewConstraints<T>
            where T : new()
        {
        }

        public interface ITestOpenGenericWithBaseTypeConstraints<T>
            where T : TestOpenGeneric
        {
        }

        public interface ITestOpenGenericWithInterfaceConstraints<T>
            where T : ITestOpenGeneric
        {
        }

        public interface ITestOpenGenericWithGenericInterfaceConstraint<T, K>
            where T : ITestOpenGeneric<K>
        {
        }

        public class TestOpenGeneric
        {
        }

        public class TestOpenGeneric<T> : ITestOpenGeneric<T>
        {
        }

        public class TestOpenGenericWithMultipleParameters<T, K> : ITestOpenGenericWithMultipleParameters<T, K>
        {
        }

        public class TestOpenGenericWithMultipleParametersAndDifferentConstraints<T, K> : ITestOpenGenericWithMultipleParametersAndDifferentConstraints<T, K>
            where T : class
            where K : struct
        {
        }

        public class TestOpenGenericWithClassConstraints<T> : ITestOpenGenericWithClassConstraints<T>
            where T : class
        {
        }

        public class TestOpenGenericWithStructConstraints<T> : ITestOpenGenericWithStructConstraints<T>
            where T : struct
        {
        }

        public class TestOpenGenericWithBaseClassConstraints<T> : ITestOpenGenericWithBaseTypeConstraints<T>
            where T : TestOpenGeneric
        {
        }

        public class TestOpenGenericWithInterfaceConstraints<T> : ITestOpenGenericWithInterfaceConstraints<T>
            where T : ITestOpenGeneric
        {
        }

        public class TestOpenGenericWithGenericInterfaceConstraint<T, K> : ITestOpenGenericWithGenericInterfaceConstraint<T, K>
            where T : ITestOpenGeneric<K>
        {
        }

        private class TestOpenGenericWithOutModifier<T> : ITestOpenGenericWithOutModifier<T>
        {
        }

        private class TestOpenGenericWithInModifier<T> : ITestOpenGenericWithInModifier<T>
        {
        }

        private class TestOpenGenericWithGetProperty<T> : ITestOpenGenericWithGetProperty<T>
        {
            public int Value => throw new NotImplementedException();
        }

        private class TestOpenGenericWithRefGetProperty<T> : ITestOpenGenericWithRefGetProperty<T>
        {
            public ref int Value => throw new NotImplementedException();
        }

        private class TestOpenGenericWithGetSetProperty<T> : ITestOpenGenericWithGetSetProperty<T>
        {
            public int Value
            {
                get => throw new NotImplementedException();
                set => throw new NotImplementedException();
            }
        }

        private class TestOpenGenericWithNonVoidMethodVoidParameters<T> : ITestOpenGenericWithNonVoidMethodVoidParameters<T>
        {
            public int Method()
            {
                throw new NotImplementedException();
            }
        }

        private class TestOpenGenericWithVoidMethodVoidParameters<T> : ITestOpenGenericWithVoidMethodVoidParameters<T>
        {
            public void Method()
            {
                throw new NotImplementedException();
            }
        }

        private class TestOpenGenericWithVoidMethodWithParameters<T> : ITestOpenGenericWithVoidMethodWithParameters<T>
        {
            public void Method(
                int parameter)
            {
                throw new NotImplementedException();
            }
        }

        private class TestOpenGenericWithVoidMethodWithRefParameters<T> : ITestOpenGenericWithVoidMethodWithRefParameters<T>
        {
            public void Method(
                ref int parameter)
            {
                throw new NotImplementedException();
            }
        }

        private class TestOpenGenericWithVoidMethodWithOutParameters<T> : ITestOpenGenericWithVoidMethodWithOutParameters<T>
        {
            public void Method(
                out int parameter)
            {
                throw new NotImplementedException();
            }
        }

        private class TestOpenGenericWithGenericGetProperty<T> : ITestOpenGenericWithGenericGetProperty<T>
        {
            public T Value => throw new NotImplementedException();
        }

        private class TestOpenGenericWithGenericGetSetProperty<T> : ITestOpenGenericWithGenericGetSetProperty<T>
        {
            public T Value
            {
                get => throw new NotImplementedException();
                set => throw new NotImplementedException();
            }
        }

        private class TestOpenGenericWithVoidMethodWithGenericParameters<T> : ITestOpenGenericWithVoidMethodWithGenericParameters<T>
        {
            public void Method(
                T parameter)
            {
                throw new NotImplementedException();
            }
        }

        private class TestOpenGenericWithVoidMethodWithGenericGenericParameters<T> : ITestOpenGenericWithVoidMethodWithGenericGenericParameters<T>
        {
            public void Method(
                Action<T> parameter)
            {
                throw new NotImplementedException();
            }
        }

        private class TestOpenGenericWithGenericReturnMethodVoidParameters<T> : ITestOpenGenericWithGenericReturnMethodVoidParameters<T>
        {
            public T Method()
            {
                throw new NotImplementedException();
            }
        }

        private class TestOpenGenericWithIndependentGenericReturnMethodVoidParameters<T> : ITestOpenGenericWithIndependentGenericReturnMethodVoidParameters<T>
        {
            public K Method<K>()
            {
                throw new NotImplementedException();
            }
        }

        private class TestOpenGenericWithVoidMethodWithIndependentGenericParameters<T> : ITestOpenGenericWithVoidMethodWithIndependentGenericParameters<T>
        {
            public void Method<K>(
                K parameter)
            {
                throw new NotImplementedException();
            }
        }

        private class TestOpenGenericWithVoidMethodWithIndependentGenericParametersWithClassConstraints<T>
            : ITestOpenGenericWithVoidMethodWithIndependentGenericParametersWithClassConstraints<T>
        {
            public void Method<K>(
                K parameter)
                where K : class
            {
                throw new NotImplementedException();
            }
        }

        private class TestOpenGenericWithVoidMethodWithIndependentGenericParametersWithStructConstraints<T>
            : ITestOpenGenericWithVoidMethodWithIndependentGenericParametersWithStructConstraints<T>
        {
            public void Method<K>(
                K parameter)
                where K : struct
            {
                throw new NotImplementedException();
            }
        }

        private class TestOpenGenericWithVoidMethodWithIndependentGenericParametersWithNewConstraints<T>
            : ITestOpenGenericWithVoidMethodWithIndependentGenericParametersWithNewConstraints<T>
        {
            public void Method<K>(
                K parameter)
                where K : new()
            {
                throw new NotImplementedException();
            }
        }

        private class TestOpenGenericWithVoidMethodWithIndependentGenericParametersWithBaseTypeConstraints<T>
            : ITestOpenGenericWithVoidMethodWithIndependentGenericParametersWithBaseTypeConstraints<T>
        {
            public void Method<K>(
                K parameter)
                where K : TestOpenGeneric
            {
                throw new NotImplementedException();
            }
        }

        private class TestOpenGenericWithVoidMethodWithIndependentGenericParametersWithBaseInterfaceConstraints<T>
            : ITestOpenGenericWithVoidMethodWithIndependentGenericParametersWithBaseInterfaceConstraints<T>
        {
            public void Method<K>(
                K parameter)
                where K : ITestOpenGeneric
            {
                throw new NotImplementedException();
            }
        }

        private class TestOpenGenericWithVoidMethodWithIndependentGenericParametersWithGenericInterfaceConstraints<T>
            : ITestOpenGenericWithVoidMethodWithIndependentGenericParametersWithGenericInterfaceConstraints<T>
        {
            public void Method<K, X>(
                K parameter)
                where K : ITestOpenGeneric<X>
            {
                throw new NotImplementedException();
            }
        }

        private class TestOpenGenericWithNewConstraints<T> : ITestOpenGenericWithNewConstraints<T>
            where T : new()
        {
        }

        private static class Cases
        {
            public class Case
            {
                public Type ServiceType { get; }

                public Type ImplementationType { get; }

                public Type RequestType { get; }

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
                    return $"{this.ImplementationType.Name}";
                }
            }

            public static IEnumerable<object[]> EmitOpenGenericCases
            {
                get
                {
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestOpenGeneric<>),
                            typeof(TestOpenGeneric<>),
                            typeof(ITestOpenGeneric<int>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestOpenGenericWithOutModifier<>),
                            typeof(TestOpenGenericWithOutModifier<>),
                            typeof(ITestOpenGenericWithOutModifier<int>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestOpenGenericWithInModifier<>),
                            typeof(TestOpenGenericWithInModifier<>),
                            typeof(ITestOpenGenericWithInModifier<int>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestOpenGenericWithGetProperty<>),
                            typeof(TestOpenGenericWithGetProperty<>),
                            typeof(ITestOpenGenericWithGetProperty<int>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestOpenGenericWithRefGetProperty<>),
                            typeof(TestOpenGenericWithRefGetProperty<>),
                            typeof(ITestOpenGenericWithRefGetProperty<int>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestOpenGenericWithGetSetProperty<>),
                            typeof(TestOpenGenericWithGetSetProperty<>),
                            typeof(ITestOpenGenericWithGetSetProperty<int>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestOpenGenericWithNonVoidMethodVoidParameters<>),
                            typeof(TestOpenGenericWithNonVoidMethodVoidParameters<>),
                            typeof(ITestOpenGenericWithNonVoidMethodVoidParameters<int>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestOpenGenericWithVoidMethodVoidParameters<>),
                            typeof(TestOpenGenericWithVoidMethodVoidParameters<>),
                            typeof(ITestOpenGenericWithVoidMethodVoidParameters<int>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestOpenGenericWithVoidMethodWithParameters<>),
                            typeof(TestOpenGenericWithVoidMethodWithParameters<>),
                            typeof(ITestOpenGenericWithVoidMethodWithParameters<int>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestOpenGenericWithVoidMethodWithRefParameters<>),
                            typeof(TestOpenGenericWithVoidMethodWithRefParameters<>),
                            typeof(ITestOpenGenericWithVoidMethodWithRefParameters<int>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestOpenGenericWithVoidMethodWithOutParameters<>),
                            typeof(TestOpenGenericWithVoidMethodWithOutParameters<>),
                            typeof(ITestOpenGenericWithVoidMethodWithOutParameters<int>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestOpenGenericWithGenericGetProperty<>),
                            typeof(TestOpenGenericWithGenericGetProperty<>),
                            typeof(ITestOpenGenericWithGenericGetProperty<int>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestOpenGenericWithGenericGetSetProperty<>),
                            typeof(TestOpenGenericWithGenericGetSetProperty<>),
                            typeof(ITestOpenGenericWithGenericGetSetProperty<int>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestOpenGenericWithVoidMethodWithGenericParameters<>),
                            typeof(TestOpenGenericWithVoidMethodWithGenericParameters<>),
                            typeof(ITestOpenGenericWithVoidMethodWithGenericParameters<int>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestOpenGenericWithVoidMethodWithGenericGenericParameters<>),
                            typeof(TestOpenGenericWithVoidMethodWithGenericGenericParameters<>),
                            typeof(ITestOpenGenericWithVoidMethodWithGenericGenericParameters<int>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestOpenGenericWithGenericReturnMethodVoidParameters<>),
                            typeof(TestOpenGenericWithGenericReturnMethodVoidParameters<>),
                            typeof(ITestOpenGenericWithGenericReturnMethodVoidParameters<int>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestOpenGenericWithVoidMethodWithIndependentGenericParameters<>),
                            typeof(TestOpenGenericWithVoidMethodWithIndependentGenericParameters<>),
                            typeof(ITestOpenGenericWithVoidMethodWithIndependentGenericParameters<int>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestOpenGenericWithIndependentGenericReturnMethodVoidParameters<>),
                            typeof(TestOpenGenericWithIndependentGenericReturnMethodVoidParameters<>),
                            typeof(ITestOpenGenericWithIndependentGenericReturnMethodVoidParameters<int>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestOpenGenericWithVoidMethodWithIndependentGenericParametersWithClassConstraints<>),
                            typeof(TestOpenGenericWithVoidMethodWithIndependentGenericParametersWithClassConstraints<>),
                            typeof(ITestOpenGenericWithVoidMethodWithIndependentGenericParametersWithClassConstraints<int>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestOpenGenericWithVoidMethodWithIndependentGenericParametersWithStructConstraints<>),
                            typeof(TestOpenGenericWithVoidMethodWithIndependentGenericParametersWithStructConstraints<>),
                            typeof(ITestOpenGenericWithVoidMethodWithIndependentGenericParametersWithStructConstraints<int>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestOpenGenericWithVoidMethodWithIndependentGenericParametersWithNewConstraints<>),
                            typeof(TestOpenGenericWithVoidMethodWithIndependentGenericParametersWithNewConstraints<>),
                            typeof(ITestOpenGenericWithVoidMethodWithIndependentGenericParametersWithNewConstraints<int>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestOpenGenericWithVoidMethodWithIndependentGenericParametersWithBaseTypeConstraints<>),
                            typeof(TestOpenGenericWithVoidMethodWithIndependentGenericParametersWithBaseTypeConstraints<>),
                            typeof(ITestOpenGenericWithVoidMethodWithIndependentGenericParametersWithBaseTypeConstraints<int>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestOpenGenericWithVoidMethodWithIndependentGenericParametersWithBaseInterfaceConstraints<>),
                            typeof(TestOpenGenericWithVoidMethodWithIndependentGenericParametersWithBaseInterfaceConstraints<>),
                            typeof(ITestOpenGenericWithVoidMethodWithIndependentGenericParametersWithBaseInterfaceConstraints<int>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestOpenGenericWithVoidMethodWithIndependentGenericParametersWithGenericInterfaceConstraints<>),
                            typeof(TestOpenGenericWithVoidMethodWithIndependentGenericParametersWithGenericInterfaceConstraints<>),
                            typeof(ITestOpenGenericWithVoidMethodWithIndependentGenericParametersWithGenericInterfaceConstraints<int>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestOpenGenericWithMultipleParameters<,>),
                            typeof(TestOpenGenericWithMultipleParameters<,>),
                            typeof(ITestOpenGenericWithMultipleParameters<int, int>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestOpenGenericWithMultipleParametersAndDifferentConstraints<,>),
                            typeof(TestOpenGenericWithMultipleParametersAndDifferentConstraints<,>),
                            typeof(ITestOpenGenericWithMultipleParametersAndDifferentConstraints<object, int>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestOpenGenericWithClassConstraints<>),
                            typeof(TestOpenGenericWithClassConstraints<>),
                            typeof(ITestOpenGenericWithClassConstraints<object>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestOpenGenericWithStructConstraints<>),
                            typeof(TestOpenGenericWithStructConstraints<>),
                            typeof(ITestOpenGenericWithStructConstraints<int>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestOpenGenericWithNewConstraints<>),
                            typeof(TestOpenGenericWithNewConstraints<>),
                            typeof(ITestOpenGenericWithNewConstraints<int>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestOpenGenericWithBaseTypeConstraints<>),
                            typeof(TestOpenGenericWithBaseClassConstraints<>),
                            typeof(ITestOpenGenericWithBaseTypeConstraints<TestOpenGeneric>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestOpenGenericWithInterfaceConstraints<>),
                            typeof(TestOpenGenericWithInterfaceConstraints<>),
                            typeof(ITestOpenGenericWithInterfaceConstraints<ITestOpenGeneric>))
                    };
                    yield return new object[]
                    {
                        new Case(
                            typeof(ITestOpenGenericWithGenericInterfaceConstraint<,>),
                            typeof(TestOpenGenericWithGenericInterfaceConstraint<,>),
                            typeof(ITestOpenGenericWithGenericInterfaceConstraint<ITestOpenGeneric<int>, int>))
                    };
                }
            }
        }

        [Theory]
        [MemberData(nameof(Cases.EmitOpenGenericCases), MemberType = typeof(Cases))]
        private static void Should_generate_open_generic_proxy_with_dependency_injection_target_resolution_For_open_generic_type(
            Cases.Case @case)
        {
            // Arrange
            var arrangeRootCollection = (IServiceCollection) new ServiceCollection();
            arrangeRootCollection.Add(new ServiceDescriptor(@case.ServiceType, @case.ImplementationType, ServiceLifetime.Singleton));

            var arrangeRootServices = arrangeRootCollection.BuildServiceProvider();

            var arrangeCollection = (IServiceCollection) new ServiceCollection();
            arrangeCollection.Add(new ServiceDescriptor(typeof(IOpenGenericTargetServiceProvider), new OpenGenericTargetServiceProvider(arrangeRootServices)));
            arrangeCollection.Add(new ServiceDescriptor(@case.ServiceType, OpenGenericProxyEmitter.Emit(@case.ServiceType), ServiceLifetime.Singleton));

            var arrangeServices = arrangeCollection.BuildServiceProvider();

            // Act
            var obj = arrangeServices.GetService(@case.RequestType);

            // Assert
            Assert.NotNull(obj);

            var type = obj.GetType();

            foreach (var mi in type.GetInterfaceMap(@case.RequestType).TargetMethods)
            {
                var method = mi;

                if (method.IsGenericMethod)
                {
                    // Skip generic method invocation.
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
                                    Expression.Constant(obj),
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