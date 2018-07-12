using System;
using System.Collections.Generic;

using Microsoft.ServiceFabric.Services.Remoting.V2;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests
{
    public class TestRemotingSerializationProviderWithParameters : IServiceRemotingMessageSerializationProvider
    {
        public ITestDependency Dependency { get; }

        public TestRemotingSerializationProviderWithParameters(
            ITestDependency dependency)
        {
            this.Dependency = dependency;
        }

        public IServiceRemotingMessageBodyFactory CreateMessageBodyFactory()
        {
            return null;
        }

        public IServiceRemotingRequestMessageBodySerializer CreateRequestMessageSerializer(
            Type serviceInterfaceType,
            IEnumerable<Type> requestBodyTypes)
        {
            return null;
        }

        public IServiceRemotingResponseMessageBodySerializer CreateResponseMessageSerializer(
            Type serviceInterfaceType,
            IEnumerable<Type> responseBodyTypes)
        {
            return null;
        }
    }
}