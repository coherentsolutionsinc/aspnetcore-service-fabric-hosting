using System;
using System.Reflection;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Extensions
{
    public static partial class ReflectionQuery
    {
        public struct Root
        {
            public readonly Type target;

            public Root(
                Type target)
            {
                this.target = target ?? throw new ArgumentNullException(nameof(target));
            }

            public ConstructorMap Constructor()
            {
                return new ConstructorMap(this.target);
            }

            public Property Property(string name)
            {
                return new Property(this.target, name);
            }

            public Method Method(string name)
            {
                return new Method(this.target, name);
            }
        }
    }
}