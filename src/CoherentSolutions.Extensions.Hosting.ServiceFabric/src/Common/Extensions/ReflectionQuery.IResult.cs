namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Extensions
{
    public static partial class ReflectionQuery
    {
        public interface IResult<T>
        {
            T Get();
        }
    }
}