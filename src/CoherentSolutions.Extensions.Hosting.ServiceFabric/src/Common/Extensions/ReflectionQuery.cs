using System;
using System.Reflection;
using System.Threading;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Extensions
{
    public static partial class ReflectionQuery
    {
        public static Root Query(
            this Type @this)
        {
            return new Root(@this);
        }

        public static T Public<T>(
            this T @this)
            where T : Bindable
        {
            ((IBindable)@this).Public();
            return @this;
        }

        public static T NonPublic<T>(
            this T @this)
            where T : Bindable
        {
            ((IBindable)@this).NonPublic();
            return @this;
        }

        public static T Static<T>(
            this T @this)
            where T : Bindable
        {
            ((IBindable)@this).Static();
            return @this;
        }

        public static T Instance<T>(
            this T @this)
            where T : Bindable
        {
            ((IBindable)@this).Instance();
            return @this;
        }

        public static Lazy<T> GetLazy<T>(
            this IResult<T> @this)
        {
            return new Lazy<T>(() => @this.Get());
        }
    }
}