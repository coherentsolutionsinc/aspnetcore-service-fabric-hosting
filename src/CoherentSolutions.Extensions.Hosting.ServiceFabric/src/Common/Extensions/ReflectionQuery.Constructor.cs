using System;
using System.Reflection;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Extensions
{
    public static partial class ReflectionQuery
    {
        public class ConstructorMap : Bindable, IResult<ConstructorInfo>
        {
            private readonly Type target;

            public ConstructorMap(
                Type target)
            {
                this.target = target ?? throw new ArgumentNullException(nameof(target));
            }

            public ConstructorInfo Get()
            {
                var c = this.target.GetConstructor(this.BindingFlags, null, Type.EmptyTypes, null);
                if (c is null)
                {
                    throw new MissingMemberException(
                        this.target.Name, 
                        $"The type: '{this.target.FullName}' doesn't have parameterless constructor.");
                }

                return c;
            }
        }
    }
}