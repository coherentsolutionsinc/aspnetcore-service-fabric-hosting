namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Extensions
{
    public static partial class ReflectionQuery
    {
        private interface IBindable
        {
            void Public();

            void NonPublic();

            void Static();

            void Instance();
        }
    }
}