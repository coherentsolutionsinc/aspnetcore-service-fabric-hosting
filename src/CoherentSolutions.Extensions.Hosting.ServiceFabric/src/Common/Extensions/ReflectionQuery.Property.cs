using System;
using System.Reflection;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Extensions
{
    public static partial class ReflectionQuery
    {
        public class Property : Bindable, IResult<PropertyInfo>
        {
            public readonly Type target;

            public readonly string name;

            public Property(
                Type target,
                string name)
            {
                this.target = target ?? throw new ArgumentNullException(nameof(target));
                this.name = name ?? throw new ArgumentNullException(nameof(name));
            }

            public PropertyInfo Get()
            {
                var p = this.target.GetProperty(this.name, this.BindingFlags);
                if (p is null)
                {
                    throw new MissingMemberException(this.target.FullName, this.name);
                }

                return p;
            }
        }
    }
}