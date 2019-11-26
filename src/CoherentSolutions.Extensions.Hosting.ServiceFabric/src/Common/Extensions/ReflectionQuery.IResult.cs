using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Extensions
{
    public static partial class ReflectionQuery
    {
        public interface IResult<T>
        {
            T Get();
        }

        public static Lazy<T> GetLazy<T>(
            this IResult<T> @this)
        {
            return new Lazy<T>(() => @this.Get());
        }
    }
}