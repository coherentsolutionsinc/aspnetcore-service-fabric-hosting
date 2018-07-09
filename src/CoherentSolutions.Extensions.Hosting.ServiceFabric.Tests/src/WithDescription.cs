namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests
{
    public class WithDescription<T>
    {
        public readonly T Target;

        public readonly string Description;

        public WithDescription(
            T target,
            string description)
        {
            this.Target = target;
            this.Description = description;
        }

        public override string ToString()
        {
            return this.Description;
        }
    }

    public static class WithDescriptionExtensions
    {
        public static WithDescription<T> WithDescription<T>(
            this T target,
            string description)
        {
            return new WithDescription<T>(target, description);
        }
    }
}