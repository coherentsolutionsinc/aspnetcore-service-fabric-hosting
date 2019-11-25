using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Extensions
{
    public static partial class ReflectionQuery
    {
        public static Lazy<ConstructorInfo> QueryConstructor(
            this Type @this,
            bool @public = true)
        {
            var query = @this.Query().Constructor();
            query = @public
                ? query.Public()
                : query.NonPublic();

            return query.Instance().GetLazy();
        }

        public static Lazy<PropertyInfo> QueryProperty(
            this Type @this,
            string name,
            bool @public = true)
        {
            var query = @this.Query().Property(name);
            query = @public 
                ? query.Public() 
                : query.NonPublic();

            return query.Instance().GetLazy();
        }
    }
}
