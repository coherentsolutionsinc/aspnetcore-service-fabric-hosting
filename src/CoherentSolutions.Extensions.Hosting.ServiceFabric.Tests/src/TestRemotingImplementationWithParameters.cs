namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests
{
    public class TestRemotingImplementationWithParameters : ITestRemotingService
    {
        public ITestDependency Dependency { get; }

        public TestRemotingImplementationWithParameters(
            ITestDependency dependency)
        {
            this.Dependency = dependency;
        }
    }
}