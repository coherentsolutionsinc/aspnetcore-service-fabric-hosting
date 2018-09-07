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
            IEnumerable<Type> requestWrappedTypes,
            IEnumerable<Type> requestBodyTypes = null)
        {
            return null;
        }

        public IServiceRemotingResponseMessageBodySerializer CreateResponseMessageSerializer(
            Type serviceInterfaceType,
            IEnumerable<Type> responseWrappedTypes,
            IEnumerable<Type> responseBodyTypes = null)
        {
            return null;
        }
    }
}