using System;
using System.Reflection;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Extensions
{
    public static class TypeExtensions
    {
        private const BindingFlags NonPublicInstance = BindingFlags.NonPublic | BindingFlags.Instance;
        public static ConstructorInfo GetNonPublicConstructor(
            this Type t)
        {
            return t.GetConstructor(NonPublicInstance, null, Type.EmptyTypes, null);
        }
    }
}
