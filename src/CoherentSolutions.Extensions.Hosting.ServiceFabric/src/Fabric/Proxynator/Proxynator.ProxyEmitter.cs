using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Proxynator
{
    public static partial class Proxynator
    {
        private partial class DependencyInjectionProxyEmitter : ProxyEmitter
        {
            private readonly Type providerType;

            private readonly Type implementationType;

            public DependencyInjectionProxyEmitter(
                Type providerType,
                Type interfaceType,
                Type implementationType)
                : base(interfaceType)
            {
                this.providerType = providerType
                 ?? throw new ArgumentNullException(nameof(providerType));

                this.implementationType = implementationType;
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

                var itemType = fieldBuilder.FieldType;
                var enumerableType = typeof(IEnumerable<>).MakeGenericType(itemType);
                var arrayType = itemType.MakeArrayType();

                var array = il.DeclareLocal(arrayType);
                var index = il.DeclareLocal(typeof(int));
                var type = il.DeclareLocal(typeof(Type));
                var isGenericType = il.DeclareLocal(typeof(bool));
                var areEqual = il.DeclareLocal(typeof(bool));

                var forStart = il.DefineLabel();
                var forNextIndex = il.DefineLabel();
                var forIndexBoundry = il.DefineLabel();
                var isntGenericType = il.DefineLabel();
                var exitCtor = il.DefineLabel();

                // Call object.ctor()
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));

                // var array = ((IEnumerable<T>)services.GetService(typeof(IEnumerable<T>))).ToArray();
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldtoken, enumerableType);
                il.Emit(OpCodes.Call, typeof(Type).GetMethod(nameof(Type.GetTypeFromHandle)));
                il.Emit(OpCodes.Callvirt, typeof(IServiceProvider).GetMethod(nameof(IServiceProvider.GetService)));
                il.Emit(OpCodes.Castclass, enumerableType);
                il.Emit(OpCodes.Call, typeof(Enumerable).GetMethod(nameof(Enumerable.ToArray)).MakeGenericMethod(itemType));
                il.Emit(OpCodes.Stloc, array);

                // var index = 0;
                il.Emit(OpCodes.Ldc_I4_0);
                il.Emit(OpCodes.Stloc, index);

                // goto forIndexBoundry;
                il.Emit(OpCodes.Br_S, forIndexBoundry);

                // forStart:
                il.MarkLabel(forStart);

                // var t = array[index].GetType();
                il.Emit(OpCodes.Ldloc, array);
                il.Emit(OpCodes.Ldloc, index);
                il.Emit(OpCodes.Ldelem_Ref);
                il.Emit(OpCodes.Callvirt, typeof(object).GetMethod(nameof(object.GetType)));
                il.Emit(OpCodes.Stloc, type);

                // var isGenericType = t.IsGenericType;
                il.Emit(OpCodes.Ldloc, type);
                il.Emit(OpCodes.Callvirt, typeof(Type).GetProperty(nameof(Type.IsGenericType)).GetMethod);
                il.Emit(OpCodes.Stloc, isGenericType);

                // if (!isGenericType) goto isntGenericType;
                il.Emit(OpCodes.Ldloc, isGenericType);
                il.Emit(OpCodes.Brfalse_S, isntGenericType);

                // t = t.GetGenericTypeDefinition();
                il.Emit(OpCodes.Ldloc, type);
                il.Emit(OpCodes.Callvirt, typeof(Type).GetMethod(nameof(Type.GetGenericTypeDefinition)));
                il.Emit(OpCodes.Stloc, type);

                // isntGenericType:
                il.MarkLabel(isntGenericType);

                // var areEqual = t == T;
                il.Emit(OpCodes.Ldloc, type);
                il.Emit(OpCodes.Ldtoken, this.implementationType);
                il.Emit(OpCodes.Call, typeof(Type).GetMethod(nameof(Type.GetTypeFromHandle)));
                il.Emit(OpCodes.Call, typeof(Type).GetMethod("op_Equality"));
                il.Emit(OpCodes.Stloc, areEqual);

                // if (!areEqual) goto forBoundryCheck;
                il.Emit(OpCodes.Ldloc, areEqual);
                il.Emit(OpCodes.Brfalse_S, forNextIndex);

                // this.field = (T)array[index];
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc, array);
                il.Emit(OpCodes.Ldloc, index);
                il.Emit(OpCodes.Ldelem_Ref);
                il.Emit(OpCodes.Stfld, fieldBuilder);

                // goto exitCtor;
                il.Emit(OpCodes.Br_S, exitCtor);

                // forNextIndex:
                il.MarkLabel(forNextIndex);

                // ++index
                il.Emit(OpCodes.Ldloc, index);
                il.Emit(OpCodes.Ldc_I4_1);
                il.Emit(OpCodes.Add);
                il.Emit(OpCodes.Stloc, index);

                // forIndexBoundry:
                il.MarkLabel(forIndexBoundry);

                // if (index < array.Length) goto forStart;
                il.Emit(OpCodes.Ldloc, index);
                il.Emit(OpCodes.Ldloc, array);
                il.Emit(OpCodes.Ldlen);
                il.Emit(OpCodes.Conv_I4);
                il.Emit(OpCodes.Clt);
                il.Emit(OpCodes.Brtrue_S, forStart);

                // exitCtor:
                il.MarkLabel(exitCtor);

                // return
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

                // object.ctor();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));

                // this.field = argument;
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Stfld, fieldBuilder);

                // return
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
                    "target",
                    this.interfaceType.IsGenericType
                        ? this.interfaceType.MakeGenericType(Utils.CreateGenericParametersFrom(typeBuilder, this.interfaceType))
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