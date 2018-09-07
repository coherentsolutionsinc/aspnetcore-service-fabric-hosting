using System;
using System.Collections.Generic;

using Microsoft.ServiceFabric.Services.Remoting.V2;

using ServiceFabric.Mocks.RemotingV2;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Mocks
{
    public class MockServiceRemotingMessageSerializationProvider : IServiceRemotingMessageSerializationProvider
    {
        public IServiceRemotingMessageBodyFactory CreateMessageBodyFactory()
        {
            return new MockServiceRemotingMessageBodyFactory();
        }

        public IServiceRemotingRequestMessageBodySerializer CreateRequestMessageSerializer(
            Type serviceInterfaceType,
            IEnumerable<Type> requestWrappedTypes,
            IEnumerable<Type> requestBodyTypes = null)
        {
            return new MockServiceRemotingRequestMessageBodySerializer();
        }

        public IServiceRemotingResponseMessageBodySerializer CreateResponseMessageSerializer(
            Type serviceInterfaceType,
            IEnumerable<Type> responseWrappedTypes,
            IEnumerable<Type> responseBodyTypes = null)
        {
            return new MockServiceRemotingResponseMessageBodySerializer();
        }
    }
}