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
                this.target = target ?? throw new ArgumentNullException(nameof(target));
                this.name = name ?? throw new ArgumentNullException(nameof(name));
            }

            public Method Params(
                params Type[] parameters)
            {
                this.parameters = parameters;

                return this;
            }

            public MethodInfo Get()
            {
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