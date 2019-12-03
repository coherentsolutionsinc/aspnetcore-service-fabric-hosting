using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Proxynator
{
    public static partial class Proxynator
    {
        private abstract partial class ProxyEmitter
        {
            private static partial class Utils
            {
                public static void GetInterfaceMethodsAndProperties(
                    Type t,
                    out IReadOnlyList<MethodInfo> methodInfos,
                    out IReadOnlyList<PropertyInfo> propertieInfos)
                {
                    if (t == null)
                    {
                        throw new ArgumentNullException(nameof(t));
                    }

                    var interfaces = new Stack<Type>();
                    interfaces.Push(t);

                    var methods = new List<MethodInfo>();
                    var properties = new List<PropertyInfo>();

                    do
                    {
                        var current = interfaces.Pop();

                        foreach (var @interface in current.GetInterfaces())
                        {
                            interfaces.Push(@interface);
                        }

                        methods.AddRange(current.GetMethods(BindingFlags.Public | BindingFlags.Instance));
                        properties.AddRange(current.GetProperties(BindingFlags.Public | BindingFlags.Instance));
                    }
                    while (interfaces.Count > 0);

                    methods.RemoveAll(
                        methodInfo =>
                        {
                            return properties.Any(
                                propertyInfo => propertyInfo.GetMethod == methodInfo || propertyInfo.SetMethod == methodInfo);
                        });

                    methodInfos = methods;
                    propertieInfos = properties;
                }

                public static PropertyBuilder CreateProxyProperty(
                    TypeBuilder typeBuilder,
                    FieldBuilder fieldBuilder,
                    PropertyInfo propertyInfo)
                {
                    if (typeBuilder == null)
                    {
                        throw new ArgumentNullException(nameof(typeBuilder));
                    }

                    if (propertyInfo == null)
                    {
                        throw new ArgumentNullException(nameof(propertyInfo));
                    }

                    var interfacePropertyParameters = propertyInfo.GetIndexParameters();

                    var implementationProperty = typeBuilder.DefineProperty(
                        propertyInfo.Name,
                        propertyInfo.Attributes,
                        CallingConventions.Standard,
                        propertyInfo.PropertyType,
                        propertyInfo.GetRequiredCustomModifiers(),
                        propertyInfo.GetOptionalCustomModifiers(),
                        interfacePropertyParameters.Select(pi => pi.ParameterType).ToArray(),
                        interfacePropertyParameters.Select(pi => pi.GetRequiredCustomModifiers()).ToArray(),
                        interfacePropertyParameters.Select(pi => pi.GetOptionalCustomModifiers()).ToArray());

                    if (propertyInfo.GetMethod != null)
                    {
                        implementationProperty.SetGetMethod(CreateProxyMethod(typeBuilder, fieldBuilder, propertyInfo.GetMethod));
                    }

                    if (propertyInfo.SetMethod != null)
                    {
                        implementationProperty.SetSetMethod(CreateProxyMethod(typeBuilder, fieldBuilder, propertyInfo.SetMethod));
                    }

                    return implementationProperty;
                }

                public static MethodBuilder CreateProxyMethod(
                    TypeBuilder typeBuilder,
                    FieldBuilder fieldBuilder,
                    MethodInfo methodInfo)
                {
                    if (typeBuilder == null)
                    {
                        throw new ArgumentNullException(nameof(typeBuilder));
                    }

                    if (fieldBuilder == null)
                    {
                        throw new ArgumentNullException(nameof(fieldBuilder));
                    }

                    if (methodInfo == null)
                    {
                        throw new ArgumentNullException(nameof(methodInfo));
                    }

                    var parameters = methodInfo.GetParameters();

                    var methodBuilder = typeBuilder.DefineMethod(
                        methodInfo.Name,
                        methodInfo.Attributes & ~MethodAttributes.Abstract,
                        CallingConventions.Standard,
                        methodInfo.ReturnType,
                        methodInfo.ReturnParameter?.GetRequiredCustomModifiers(),
                        methodInfo.ReturnParameter?.GetOptionalCustomModifiers(),
                        parameters.Select(pi => pi.ParameterType).ToArray(),
                        parameters.Select(pi => pi.GetRequiredCustomModifiers()).ToArray(),
                        parameters.Select(pi => pi.GetOptionalCustomModifiers()).ToArray());

                    if (methodInfo.IsGenericMethod)
                    {
                        CreateGenericParametersFrom(methodBuilder, methodInfo);
                    }

                    var il = methodBuilder.GetILGenerator();

                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldfld, fieldBuilder);

                    for (var i = 0; i < parameters.Length; ++i)
                    {
                        il.Emit(OpCodes.Ldarg, i + 1);
                    }

                    il.Emit(OpCodes.Callvirt, methodInfo);
                    il.Emit(OpCodes.Ret);

                    return methodBuilder;
                }

                public static GenericTypeParameterBuilder[] CreateGenericParametersFrom(
                    TypeBuilder typeBuilder,
                    Type typeInfo)
                {
                    if (typeBuilder == null)
                    {
                        throw new ArgumentNullException(nameof(typeBuilder));
                    }

                    if (typeInfo == null)
                    {
                        throw new ArgumentNullException(nameof(typeInfo));
                    }

                    var arguments = typeInfo.GetGenericArguments();
                    var parameters = typeBuilder.DefineGenericParameters(arguments.Select(i => i.Name).ToArray()).ToArray();

                    CopyGenericParametersInformation(parameters, arguments);

                    return parameters;
                }

                public static GenericTypeParameterBuilder[] CreateGenericParametersFrom(
                    MethodBuilder methodBuilder,
                    MethodInfo methodInfo)
                {
                    if (methodBuilder == null)
                    {
                        throw new ArgumentNullException(nameof(methodBuilder));
                    }

                    if (methodInfo == null)
                    {
                        throw new ArgumentNullException(nameof(methodInfo));
                    }

                    var arguments = methodInfo.GetGenericArguments();
                    var parameters = methodBuilder.DefineGenericParameters(arguments.Select(i => i.Name).ToArray()).ToArray();

                    CopyGenericParametersInformation(parameters, arguments);

                    return parameters;
                }

                private static void CopyGenericParametersInformation(
                    IEnumerable<GenericTypeParameterBuilder> parameters,
                    IEnumerable<Type> arguments)
                {
                    if (parameters == null)
                    {
                        throw new ArgumentNullException(nameof(parameters));
                    }

                    if (arguments == null)
                    {
                        throw new ArgumentNullException(nameof(arguments));
                    }

                    foreach (var zip in parameters.Zip(
                        arguments,
                        (
                            parameter,
                            argument) => (parameter, argument)))
                    {
                        CopyGenericParameterInformation(zip.parameter, zip.argument);
                    }
                }

                private static void CopyGenericParameterInformation(
                    GenericTypeParameterBuilder parameterTo,
                    Type parameterFrom)
                {
                    if (parameterTo == null)
                    {
                        throw new ArgumentNullException(nameof(parameterTo));
                    }

                    if (parameterFrom == null)
                    {
                        throw new ArgumentNullException(nameof(parameterFrom));
                    }

                    if (!parameterFrom.IsGenericParameter)
                    {
                        throw new ArgumentException($"{parameterFrom.FullName} isn't a generic parameter.");
                    }

                    // We need to eliminate co-contra variant attributes 
                    // because they can be specified only on interfaces.
                    const GenericParameterAttributes AttributesMask = ~(GenericParameterAttributes.Contravariant | GenericParameterAttributes.Covariant);

                    // Copy attributes: class, struct, new() etc.
                    parameterTo.SetGenericParameterAttributes(parameterFrom.GenericParameterAttributes & AttributesMask);

                    var constraints = parameterFrom.GetGenericParameterConstraints();
                    var baseTypeConstraint = constraints.SingleOrDefault(t => t.IsClass);
                    var interfaceConstraint = constraints.Where(t => t.IsInterface).ToArray();

                    if (baseTypeConstraint != null)
                    {
                        parameterTo.SetBaseTypeConstraint(baseTypeConstraint);
                    }

                    if (interfaceConstraint.Length != 0)
                    {
                        parameterTo.SetInterfaceConstraints(interfaceConstraint);
                    }
                }
            }
        }
    }
}