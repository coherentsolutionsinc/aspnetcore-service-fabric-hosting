using System;
using System.Fabric;
using System.Reflection;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Extensions;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.Services
{
    public class ServicePartitionInformationAccessor<TPartitionInformation>
        where TPartitionInformation : ServicePartitionInformation
    {
        private static readonly Lazy<PropertyInfo> id;

        public TPartitionInformation Instance
        {
            get;
        }

        public Guid Id
        {
            get => this.Instance.Id;
            set => id.Value.SetValue(this.Instance, value);
        }

        static ServicePartitionInformationAccessor()
        {
            id = typeof(ServicePartitionInformation).QueryProperty(nameof(ServicePartitionInformation.Id));
        }

        public ServicePartitionInformationAccessor(
            TPartitionInformation instance)
        {
            this.Instance = instance ?? throw new ArgumentNullException(nameof(instance));
        }
    }
}