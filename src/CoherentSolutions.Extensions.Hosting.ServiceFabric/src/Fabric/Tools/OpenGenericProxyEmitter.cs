using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Tools
{
    public static class OpenGenericProxyEmitter
    {
        private class EmitContext
        {
            private readonly Type shapeType;

            private TypeBuilder proxyType;

            private GenericTypeParameterBuilder[] proxyGenericTypeParameters;

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

            private void EmitProxyGenericTypeParameters()
            {
                var shapeGenericArguments = this.shapeType.GetGenericArguments();

                this.proxyGenericTypeParameters = this.proxyType.DefineGenericParameters(
                        Enumerable.Range(0, shapeGenericArguments.Length)
                           .Select(i => $"_{i}")
                           .ToArray())
                   .ToArray();

                this.proxyGenericType = this.shapeType.MakeGenericType(this.proxyGenericTypeParameters);
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

        public static bool CanProxy(
            Type t)
        {
            return t.IsInterface
             && (t.IsPublic || t.IsNestedPublic)
             && t.GetMethods().Length == 0
             && t.GetInterfaces().All(i => i.GetMethods().Length == 0)
             && t.GetProperties().All(p => p.GetIndexParameters().Length == 0 && !p.CanWrite)
             && t.GetGenericArguments().All(a => a.GetGenericParameterConstraints().Length == 0);
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