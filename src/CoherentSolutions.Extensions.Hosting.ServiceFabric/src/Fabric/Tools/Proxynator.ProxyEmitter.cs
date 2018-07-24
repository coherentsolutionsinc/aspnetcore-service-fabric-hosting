using System;
using System.Reflection;
using System.Reflection.Emit;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Tools
{
    public static partial class Proxynator
    {
        private partial class DependencyInjectionProxyEmitter : ProxyEmitter
        {
            private readonly Type providerType;

            public DependencyInjectionProxyEmitter(
                Type providerType,
                Type interfaceType)
                : base(interfaceType)
            {
                this.providerType = providerType
                 ?? throw new ArgumentNullException(nameof(providerType));
            }

            protected override void EmitConstructor(
                TypeBuilder typeBuilder,
                FieldBuilder fieldBuilder)
            {
                var ctor = typeBuilder.DefineConstructor(
                    MethodAttributes.Public,
                    CallingConventions.Standard,
                    new[]
                    {
                        this.providerType
                    });

                var il = ctor.GetILGenerator();

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldtoken, fieldBuilder.FieldType);
                il.Emit(OpCodes.Call, typeof(Type).GetMethod(nameof(Type.GetTypeFromHandle)));
                il.Emit(OpCodes.Callvirt, typeof(IServiceProvider).GetMethod(nameof(IServiceProvider.GetService)));
                il.Emit(OpCodes.Castclass, fieldBuilder.FieldType);
                il.Emit(OpCodes.Stfld, fieldBuilder);
                il.Emit(OpCodes.Ret);
            }
        }

        private partial class InstanceProxyEmitter : ProxyEmitter
        {
            public InstanceProxyEmitter(
                Type interfaceType)
                : base(interfaceType)
            {
            }

            protected override void EmitConstructor(
                TypeBuilder typeBuilder,
                FieldBuilder fieldBuilder)
            {
                var ctor = typeBuilder.DefineConstructor(
                    MethodAttributes.Public,
                    CallingConventions.Standard,
                    new[]
                    {
                        fieldBuilder.FieldType
                    });

                var il = ctor.GetILGenerator();

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Stfld, fieldBuilder);
                il.Emit(OpCodes.Ret);
            }
        }

        private abstract partial class ProxyEmitter
        {
            private static partial class Utils
            {
            }

            private static readonly AssemblyBuilder assemblyBuilder;

            private static readonly ModuleBuilder moduleBuilder;

            private readonly Type interfaceType;

            static ProxyEmitter()
            {
                assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
                    new AssemblyName("CoherentSolutions.Extensions.Hosting.ServiceFabric.Proxynated"),
                    AssemblyBuilderAccess.Run);

                moduleBuilder = assemblyBuilder.DefineDynamicModule("Root");
            }

            public ProxyEmitter(
                Type interfaceType)
            {
                this.interfaceType = interfaceType ?? throw new ArgumentNullException(nameof(interfaceType));
            }

            protected abstract void EmitConstructor(
                TypeBuilder typeBuilder,
                FieldBuilder fieldBuilder);

            public Type Emit()
            {
                this.Initialize(out var typeBuilder, out var fieldBuilder);

                this.EmitConstructor(typeBuilder, fieldBuilder);
                this.EmitProxynatedInterface(typeBuilder, fieldBuilder);
                this.EmitImplementationInterface(typeBuilder, fieldBuilder);
                this.EmitObjectInterface(typeBuilder, fieldBuilder);

                return typeBuilder.CreateTypeInfo();
            }

            private void Initialize(
                out TypeBuilder typeBuilder,
                out FieldBuilder fieldBuilder)
            {
                typeBuilder = moduleBuilder.DefineType(
                    $"_{Guid.NewGuid():N}",
                    TypeAttributes.Class,
                    null,
                    new[]
                    {
                        this.interfaceType,
                        typeof(IProxynatorProxy)
                    });

                fieldBuilder = typeBuilder.DefineField(
                    "implementation",
                    this.interfaceType.IsGenericType
                        ? this.interfaceType
                           .MakeGenericType(Utils.CreateGenericParametersFrom(typeBuilder, this.interfaceType))
                        : this.interfaceType,
                    FieldAttributes.Private | FieldAttributes.InitOnly);
            }

            private void EmitProxynatedInterface(
                TypeBuilder typeBuilder,
                FieldBuilder fieldBuilder)
            {
                var method = typeBuilder.DefineMethod(
                    $"get_{nameof(IProxynatorProxy.Target)}",
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
                il.Emit(OpCodes.Ldfld, fieldBuilder);
                il.Emit(OpCodes.Ret);

                var property = typeBuilder.DefineProperty(
                    nameof(IProxynatorProxy.Target),
                    PropertyAttributes.None,
                    CallingConventions.Standard,
                    typeof(object),
                    null);

                property.SetGetMethod(method);
            }

            private void EmitImplementationInterface(
                TypeBuilder typeBuilder,
                FieldBuilder fieldBuilder)
            {
                Utils.GetInterfaceMethodsAndProperties(
                    this.interfaceType,
                    out var methodInfos,
                    out var propertyInfos);

                foreach (var propertyInfo in propertyInfos)
                {
                    Utils.CreateProxyProperty(typeBuilder, fieldBuilder, propertyInfo);
                }

                foreach (var methodInfo in methodInfos)
                {
                    Utils.CreateProxyMethod(typeBuilder, fieldBuilder, methodInfo);
                }
            }

            private void EmitObjectInterface(
                TypeBuilder typeBuilder,
                FieldBuilder fieldBuilder)
            {
                Utils.CreateProxyMethod(
                    typeBuilder,
                    fieldBuilder,
                    typeof(object).GetMethod(nameof(this.GetHashCode), BindingFlags.Instance | BindingFlags.Public));
                Utils.CreateProxyMethod(
                    typeBuilder,
                    fieldBuilder,
                    typeof(object).GetMethod(nameof(object.Equals), BindingFlags.Instance | BindingFlags.Public));
                Utils.CreateProxyMethod(
                    typeBuilder,
                    fieldBuilder,
                    typeof(object).GetMethod(nameof(this.ToString), BindingFlags.Instance | BindingFlags.Public));
            }
        }
    }
}