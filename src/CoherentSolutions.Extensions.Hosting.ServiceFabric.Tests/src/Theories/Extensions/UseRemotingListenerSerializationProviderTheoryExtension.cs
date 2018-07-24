using System;
using System.Collections.Generic;

using Microsoft.ServiceFabric.Services.Remoting.V2;

using Moq;

using ServiceFabric.Mocks.RemotingV2;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public class UseRemotingListenerSerializationProviderTheoryExtension : IUseRemotingListenerSerializerTheoryExtension
    {
        private sealed class SerializationProvider : IServiceRemotingMessageSerializationProvider
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

        public UseRemotingListenerSerializationProviderTheoryExtension()
        {
            this.Factory = Tools.GetRemotingSerializerFunc<SerializationProvider>();
        }

        public UseRemotingListenerSerializationProviderTheoryExtension Setup<T>()
            where T : IServiceRemotingMessageSerializationProvider
        {
            this.Factory = Tools.GetRemotingSerializerFunc<T>();

            return this;
        }

        public UseRemotingListenerSerializationProviderTheoryExtension Setup(
            Func<IServiceProvider, IServiceRemotingMessageSerializationProvider> factory)
        {
            this.Factory = factory
             ?? throw new ArgumentNullException(nameof(factory));

            return this;
        }
    }
}