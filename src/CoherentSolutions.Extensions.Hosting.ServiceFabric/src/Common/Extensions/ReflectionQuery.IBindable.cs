namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Extensions
{
    public static partial class ReflectionQuery
    {
        public interface IBindable
        {
            void Public();

            void NonPublic();

            void Static();

            void Instance();
        }

        public static T Public<T>(
            this T @this)
            where T : IBindable
        {
            ((IBindable)@this).Public();
            return @this;
        }

        public static T NonPublic<T>(
            this T @this)
            where T : IBindable
        {
            ((IBindable)@this).NonPublic();
            return @this;
        }

        public static T Static<T>(
            this T @this)
            where T : IBindable
        {
            ((IBindable)@this).Static();
            return @this;
        }

        public static T Instance<T>(
            this T @this)
            where T : IBindable
        {
            ((IBindable)@this).Instance();
            return @this;
        }
    }
}