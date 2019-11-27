using System;
using System.Fabric.Description;
using System.Reflection;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Extensions;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest.Objects.Factories
{
    public sealed class EndpointResourceDescriptionAccessor : Accessor<EndpointResourceDescription>
    {
        private static readonly Lazy<PropertyInfo> port;

        static EndpointResourceDescriptionAccessor()
        {
            port = typeof(EndpointResourceDescription).QueryProperty("Port");
        }

        public string Name
        {
            get => this.Instance.Name;
            set => this.Instance.Name = value;
        }

        public EndpointProtocol Protocol
        {
            get => this.Instance.Protocol;
            set => this.Instance.Protocol = value;
        }

        public EndpointType EndpointType
        {
            get => this.Instance.EndpointType;
            set => this.Instance.EndpointType = value;
        }

        public string Certificate
        {
            get => this.Instance.Certificate;
            set => this.Instance.Certificate = value;
        }

        public int Port
        {
            get => this.Instance.Port;
            set => port.Value.SetValue(this.Instance, value);
        }

        public string UriScheme
        {
            get => this.Instance.UriScheme;
            set => this.Instance.UriScheme = value;
        }

        public string PathSuffix
        {
            get => this.Instance.PathSuffix;
            set => this.Instance.PathSuffix = value;
        }

        public string CodePackageName
        {
            get => this.Instance.CodePackageName;
            set => this.Instance.CodePackageName = value;
        }

        public string IpAddressOrFqdn
        {
            get => this.Instance.IpAddressOrFqdn;
            set => this.Instance.IpAddressOrFqdn = value;
        }

        public EndpointResourceDescriptionAccessor(
            EndpointResourceDescription instance)
            : base(instance)
        {
        }
    }
}