using System;
using System.Collections.Generic;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Extensions
{
    public static class EnumExtensions
    {
        public static IEnumerable<T> GetBitFlags<T>(
            this T value)
            where T : struct, Enum
        {
            var current = 1UL;
            var top = Convert.ToUInt64(value);
            while (current <= top)
            {
                if ((top & current) > 0)
                {
                    yield return (T) Enum.ToObject(typeof(T), current);
                }

                current <<= 1;
            }
        }
    }
}