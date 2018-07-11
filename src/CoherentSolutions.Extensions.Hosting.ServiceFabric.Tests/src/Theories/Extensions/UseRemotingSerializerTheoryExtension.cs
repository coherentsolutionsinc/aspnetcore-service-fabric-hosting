using System;
using System.Collections.Generic;

using Microsoft.ServiceFabric.Services.Remoting.V2;

using Moq;

using ServiceFabric.Mocks.RemotingV2;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public class UseRemotingSerializerTheoryExtension : IUseRemotingSerializerTheoryExtension
    {
        private sealed class RemotingSerializer : IServiceRemotingMessageSerializationProvider
        {
            public IServiceRemotingMessageBodyFactory CreateMessageBodyFactory()
            {
                return new MockServiceRemotingMessageBodyFactory();
            }

            public IServiceRemotingRequestMessageBodySerializer CreateRequestMessageSerializer(
                Type serviceInterfaceType,
                IEnumerable<Type> requestBodyTypes)
            {
                return new Mock<IServiceRemotingRequestMessageBodySerializer>().Object;
            }

            public IServiceRemotingResponseMessageBodySerializer CreateResponseMessageSerializer(
                Type serviceInterfaceType,
                IEnumerable<Type> responseBodyTypes)
            {
                return new Mock<IServiceRemotingResponseMessageBodySerializer>().Object;
            }
        }

        public Func<IServiceProvider, IServiceRemotingMessageSerializationProvider> Factory { get; private set; }

        public UseRemotingSerializerTheoryExtension()
        {
            this.Factory = Tools.GetRemotingSerializerFunc<RemotingSerializer>();
        }

        public UseRemotingSerializerTheoryExtension Setup<T>()
            where T : IServiceRemotingMessageSerializationProvider
        {
            this.Factory = Tools.GetRemotingSerializerFunc<T>();

            return this;
        }

        public UseRemotingSerializerTheoryExtension Setup(
            Func<IServiceProvider, IServiceRemotingMessageSerializationProvider> factory)
        {
            this.Factory = factory
             ?? throw new ArgumentNullException(nameof(factory));

            return this;
        }
    }
}