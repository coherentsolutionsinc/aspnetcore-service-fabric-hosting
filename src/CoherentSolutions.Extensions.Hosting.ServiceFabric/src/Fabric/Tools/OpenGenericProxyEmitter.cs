using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Tools
{
    public static class OpenGenericProxyEmitter
    {
        private class EmitContext
        {
            private readonly Type shapeType;

            private TypeBuilder proxyType;

            private Type proxyGenericType;

            private FieldBuilder proxyTargetField;

            public EmitContext(
                Type shapeType)
            {
                this.shapeType = shapeType;
            }

            public void Emit()
            {
                this.EmitProxyType();
                this.EmitProxyGenericTypeParameters();
                this.EmitProxyTargetField();
                this.EmitProxyConstructor();
                this.EmitProxyTargetProperty();
                this.EmitProxyMethods();
                this.EmitProxyEquals();
                this.EmitProxyGetHashCode();
                this.EmitProxyToString();
            }

            public Type Create()
            {
                return this.proxyType.CreateTypeInfo();
            }

            private void EmitProxyType()
            {
                var interfaces = new[]
                {
                    this.shapeType,
                    typeof(IOpenGenericProxy)
                };
                this.proxyType = module.DefineType($"_{Guid.NewGuid():N}", TypeAttributes.Class, null, interfaces);
            }

            private static void ConfigureAs(
                GenericTypeParameterBuilder proxy,
                Type shape)
            {
                const GenericParameterAttributes AttributesMask = ~(GenericParameterAttributes.Contravariant | GenericParameterAttributes.Covariant);
                proxy.SetGenericParameterAttributes(shape.GenericParameterAttributes & AttributesMask);

                var shapeGenericArgumentConstraints = shape.GetGenericParameterConstraints();
                var shapeGenericArgumentBaseTypeConstraint = shapeGenericArgumentConstraints.SingleOrDefault(t => t.IsClass);
                var shapeGenericArgumentInterfaceConstraint = shapeGenericArgumentConstraints.Where(t => t.IsInterface).ToArray();
                if (shapeGenericArgumentBaseTypeConstraint != null)
                {
                    proxy.SetBaseTypeConstraint(shapeGenericArgumentBaseTypeConstraint);
                }

                if (shapeGenericArgumentInterfaceConstraint.Length != 0)
                {
                    proxy.SetInterfaceConstraints(shapeGenericArgumentInterfaceConstraint);
                }
            }

            private void EmitProxyGenericTypeParameters()
            {
                var shapeGenericArguments = this.shapeType.GetGenericArguments();
                var proxyGenericParameters = this.proxyType.DefineGenericParameters(
                        shapeGenericArguments.Select(i => i.Name).ToArray())
                   .ToArray();

                for (var i = 0; i < shapeGenericArguments.Length; ++i)
                {
                    ConfigureAs(proxyGenericParameters[i], shapeGenericArguments[i]);
                }

                this.proxyGenericType = this.shapeType.MakeGenericType(proxyGenericParameters);
            }

            private void EmitProxyTargetField()
            {
                this.proxyTargetField = this.proxyType.DefineField(
                    "target",
                    this.proxyGenericType,
                    FieldAttributes.Private | FieldAttributes.InitOnly);
            }

            private void EmitProxyConstructor()
            {
                var ctor = this.proxyType.DefineConstructor(
                    MethodAttributes.Public,
                    CallingConventions.Standard,
                    new[]
                    {
                        typeof(IOpenGenericTargetServiceProvider)
                    });

                var il = ctor.GetILGenerator();

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldtoken, this.proxyGenericType);
                il.Emit(OpCodes.Call, typeof(Type).GetMethod(nameof(Type.GetTypeFromHandle)));
                il.Emit(OpCodes.Callvirt, typeof(IServiceProvider).GetMethod(nameof(IServiceProvider.GetService)));
                il.Emit(OpCodes.Castclass, this.proxyGenericType);
                il.Emit(OpCodes.Stfld, this.proxyTargetField);
                il.Emit(OpCodes.Ret);
            }

            private void EmitProxyTargetProperty()
            {
                var method = this.proxyType.DefineMethod(
                    $"get_{nameof(IOpenGenericProxy.Target)}",
                    MethodAttributes.Public
                  | MethodAttributes.Final
                  | MethodAttributes.HideBySig
                  | MethodAttributes.SpecialName
                  | MethodAttributes.Virtual
                  | MethodAttributes.NewSlot,
                    CallingConventions.Standard,
                    typeof(object),
                    null);

                var il = method.GetILGenerator();

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, this.proxyTargetField);
                il.Emit(OpCodes.Ret);

                var property = this.proxyType.DefineProperty(
                    nameof(IOpenGenericProxy.Target),
                    PropertyAttributes.None,
                    CallingConventions.Standard,
                    typeof(object),
                    null);

                property.SetGetMethod(method);
            }

            private void EmitProxyMethods()
            {
                var interfaces = new Stack<Type>();
                interfaces.Push(this.shapeType);

                var shapeMethods = new List<MethodInfo>();
                var shapeProperties = new List<PropertyInfo>();

                var methodsMap = new Dictionary<MethodInfo, MethodBuilder>();
                do
                {
                    var @interface = interfaces.Pop();

                    foreach (var implInterface in @interface.GetInterfaces())
                    {
                        interfaces.Push(implInterface);
                    }

                    shapeMethods.AddRange(@interface.GetMethods(BindingFlags.Public | BindingFlags.Instance));
                    shapeProperties.AddRange(@interface.GetProperties(BindingFlags.Public | BindingFlags.Instance));
                }
                while (interfaces.Count > 0);

                foreach (var shapeMethod in shapeMethods)
                {
                    var shapeMethodParameters = shapeMethod.GetParameters();
                    var proxyMethod = this.proxyType.DefineMethod(
                        shapeMethod.Name,
                        shapeMethod.Attributes & ~MethodAttributes.Abstract,
                        CallingConventions.Standard,
                        shapeMethod.ReturnType,
                        shapeMethod.ReturnParameter?.GetRequiredCustomModifiers(),
                        shapeMethod.ReturnParameter?.GetOptionalCustomModifiers(),
                        shapeMethodParameters.Select(pi => pi.ParameterType).ToArray(),
                        shapeMethodParameters.Select(pi => pi.GetRequiredCustomModifiers()).ToArray(),
                        shapeMethodParameters.Select(pi => pi.GetOptionalCustomModifiers()).ToArray());

                    if (shapeMethod.IsGenericMethod)
                    {
                        var shapeGenericArguments = shapeMethod.GetGenericArguments();
                        var proxyGenericParameters = proxyMethod.DefineGenericParameters(
                                shapeGenericArguments.Select(i => i.Name).ToArray())
                           .ToArray();

                        for (var i = 0; i < shapeGenericArguments.Length; ++i)
                        {
                            ConfigureAs(proxyGenericParameters[i], shapeGenericArguments[i]);
                        }
                    }

                    var il = proxyMethod.GetILGenerator();

                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldfld, this.proxyTargetField);

                    for (var i = 0; i < shapeMethodParameters.Length; ++i)
                    {
                        il.Emit(OpCodes.Ldarg, i + 1);
                    }

                    il.Emit(OpCodes.Callvirt, shapeMethod);
                    il.Emit(OpCodes.Ret);

                    methodsMap[shapeMethod] = proxyMethod;
                }

                foreach (var shapeProperty in shapeProperties)
                {
                    var shapePropertyParameters = shapeProperty.GetIndexParameters();
                    var proxyProperty = this.proxyType.DefineProperty(
                        shapeProperty.Name,
                        shapeProperty.Attributes,
                        CallingConventions.Standard,
                        shapeProperty.PropertyType,
                        shapeProperty.GetRequiredCustomModifiers(),
                        shapeProperty.GetOptionalCustomModifiers(),
                        shapePropertyParameters.Select(pi => pi.ParameterType).ToArray(),
                        shapePropertyParameters.Select(pi => pi.GetRequiredCustomModifiers()).ToArray(),
                        shapePropertyParameters.Select(pi => pi.GetOptionalCustomModifiers()).ToArray());

                    var getMethod = shapeProperty.GetGetMethod();
                    if (getMethod != null)
                    {
                        proxyProperty.SetGetMethod(methodsMap[getMethod]);
                    }

                    var setMethod = shapeProperty.GetSetMethod();
                    if (setMethod != null)
                    {
                        proxyProperty.SetSetMethod(methodsMap[setMethod]);
                    }
                }
            }

            private void EmitProxyEquals()
            {
                var method = this.proxyType.DefineMethod(
                    nameof(object.Equals),
                    MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                    CallingConventions.Standard,
                    typeof(bool),
                    new[]
                    {
                        typeof(object)
                    });

                var il = method.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, this.proxyTargetField);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Callvirt, typeof(object).GetMethod(nameof(object.Equals), BindingFlags.Instance | BindingFlags.Public));
                il.Emit(OpCodes.Ret);
            }

            private void EmitProxyGetHashCode()
            {
                var method = this.proxyType.DefineMethod(
                    nameof(this.GetHashCode),
                    MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                    CallingConventions.Standard,
                    typeof(int),
                    null);

                var il = method.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, this.proxyTargetField);
                il.Emit(OpCodes.Callvirt, typeof(object).GetMethod(nameof(this.GetHashCode)));
                il.Emit(OpCodes.Ret);
            }

            private void EmitProxyToString()
            {
                var method = this.proxyType.DefineMethod(
                    nameof(this.ToString),
                    MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                    CallingConventions.Standard,
                    typeof(string),
                    null);

                var il = method.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, this.proxyTargetField);
                il.Emit(OpCodes.Callvirt, typeof(object).GetMethod(nameof(this.ToString)));
                il.Emit(OpCodes.Ret);
            }
        }

        private static readonly AssemblyBuilder assembly;

        private static readonly ModuleBuilder module;

        private static readonly ConcurrentDictionary<Type, Lazy<Type>> types;

        static OpenGenericProxyEmitter()
        {
            assembly = AssemblyBuilder.DefineDynamicAssembly(
                new AssemblyName("CoherentSolutions.Extensions.Hosting.ServiceFabric.OpenGenericProxies"),
                AssemblyBuilderAccess.Run);

            module = assembly.DefineDynamicModule("Root");

            types = new ConcurrentDictionary<Type, Lazy<Type>>();
        }

        public static Type Emit(
            Type t)
        {
            return types
               .GetOrAdd(
                    t,
                    new Lazy<Type>(
                        () =>
                        {
                            var emit = new EmitContext(t);
                            emit.Emit();

                            return emit.Create();
                            //foreach (var sourceProperty in sourceType.GetProperties())
                            //{
                            //    var currentPropertyAttrs = PropertyAttributes.None;
                            //    var currentProperty = type.DefineProperty(
                            //        sourceProperty.Name,
                            //        currentPropertyAttrs,
                            //        CallingConventions.Standard,
                            //        sourceProperty.PropertyType,
                            //        null);

                            //    var currentPropertyGetMethod = type.DefineMethod(
                            //        $"_{Guid.NewGuid():N}",
                            //        MethodAttributes.Private
                            //      | MethodAttributes.Final
                            //      | MethodAttributes.HideBySig
                            //      | MethodAttributes.SpecialName
                            //      | MethodAttributes.NewSlot
                            //      | MethodAttributes.Virtual,
                            //        CallingConventions.Standard,
                            //        sourceProperty.PropertyType,
                            //        null);

                            //    var currentPropertyGetIlGenerator = currentPropertyGetMethod.GetILGenerator();
                            //    currentPropertyGetIlGenerator.Emit(OpCodes.Ldarg_0);
                            //    currentPropertyGetIlGenerator.Emit(OpCodes.Ldfld, typeField);
                            //    currentPropertyGetIlGenerator.Emit(OpCodes.Ret);

                            //    currentProperty.SetGetMethod(currentPropertyGetMethod);
                            //}
                        }))
               .Value;
        }
    }
}