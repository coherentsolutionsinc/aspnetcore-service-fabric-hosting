using System;
using System.Reflection;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Extensions
{
    public static partial class ReflectionQuery
    {
        public class Method : Bindable, IResult<MethodInfo>
        {
            private readonly Type target;

            private readonly string name;

            private Type[] parameters;

            public Method(
                Type target,
                string name)
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    throw new ArgumentException("message", nameof(name));
                }

                this.target = target ?? throw new ArgumentNullException(nameof(target));
                this.name = name;
            }

            public Method Params(
                params Type[] parameters)
            {
                this.parameters = parameters;

                return this;
            }

            public MethodInfo Get()
            {
                if (string.IsNullOrWhiteSpace(this.name))
                {
                    throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
                }

                var m = this.target.GetMethod(this.name, this.BindingFlags, null, this.parameters ?? Type.EmptyTypes, null);
                if (m is null)
                {
                    throw new MissingMethodException(this.target.FullName, this.name);
                }

                return m;
            }
        }
    }
}