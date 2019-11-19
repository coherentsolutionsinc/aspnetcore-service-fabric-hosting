using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Extensions
{
    public static partial class ReflectionQuery
    {
        public static Lazy<PropertyInfo> QueryProperty(
            this Type @this,
            string name)
        {
            return @this.Query().Property(name).Public().Instance().GetLazy();
        }
    }
}
