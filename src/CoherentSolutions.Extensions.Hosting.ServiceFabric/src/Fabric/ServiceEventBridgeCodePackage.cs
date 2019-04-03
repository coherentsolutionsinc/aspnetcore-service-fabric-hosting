using System;
using System.Fabric;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class ServiceEventBridgeCodePackage
    {
        public ServiceEventBridgeCodePackage(
            ICodePackageActivationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.CodePackageAddedEvent += this.OnContextOnCodePackageAddedEvent;
            context.CodePackageModifiedEvent += this.OnContextOnCodePackageModifiedEvent;
            context.CodePackageRemovedEvent += this.OnContextOnCodePackageRemovedEvent;
            context.ConfigurationPackageAddedEvent += this.OnContextOnConfigurationPackageAddedEvent;
            context.ConfigurationPackageModifiedEvent += this.OnContextOnConfigurationPackageModifiedEvent;
            context.ConfigurationPackageRemovedEvent += this.OnContextOnConfigurationPackageRemovedEvent;
            context.DataPackageAddedEvent += this.OnContextOnDataPackageAddedEvent;
            context.DataPackageModifiedEvent += this.OnContextOnDataPackageModifiedEvent;
            context.DataPackageRemovedEvent += this.OnContextOnDataPackageRemovedEvent;
        }

        public event EventHandler<NotifyAsyncEventArgs<IServiceEventPayloadOnPackageAdded<CodePackage>>> OnCodePackageAdded;

        public event EventHandler<NotifyAsyncEventArgs<IServiceEventPayloadOnPackageModified<CodePackage>>> OnCodePackageModified;

        public event EventHandler<NotifyAsyncEventArgs<IServiceEventPayloadOnPackageRemoved<CodePackage>>> OnCodePackageRemoved;

        public event EventHandler<NotifyAsyncEventArgs<IServiceEventPayloadOnPackageAdded<ConfigurationPackage>>> OnConfigPackageAdded;

        public event EventHandler<NotifyAsyncEventArgs<IServiceEventPayloadOnPackageModified<ConfigurationPackage>>> OnConfigPackageModified;

        public event EventHandler<NotifyAsyncEventArgs<IServiceEventPayloadOnPackageRemoved<ConfigurationPackage>>> OnConfigPackageRemoved;

        public event EventHandler<NotifyAsyncEventArgs<IServiceEventPayloadOnPackageAdded<DataPackage>>> OnDataPackageAdded;

        public event EventHandler<NotifyAsyncEventArgs<IServiceEventPayloadOnPackageModified<DataPackage>>> OnDataPackageModified;

        public event EventHandler<NotifyAsyncEventArgs<IServiceEventPayloadOnPackageRemoved<DataPackage>>> OnDataPackageRemoved;

        private void OnContextOnCodePackageAddedEvent(
            object sender,
            PackageAddedEventArgs<CodePackage> args)
        {
            this.OnCodePackageAdded.NotifyAsync(
                    this,
                    new ServiceEventPayloadOnPackageAdded<CodePackage>(args.Package))
               .GetAwaiter()
               .GetResult();
        }

        private void OnContextOnCodePackageModifiedEvent(
            object sender,
            PackageModifiedEventArgs<CodePackage> args)
        {
            this.OnCodePackageModified.NotifyAsync(
                    this,
                    new ServiceEventPayloadOnPackageModified<CodePackage>(args.OldPackage, args.NewPackage))
               .GetAwaiter()
               .GetResult();
        }

        private void OnContextOnCodePackageRemovedEvent(
            object sender,
            PackageRemovedEventArgs<CodePackage> args)
        {
            this.OnCodePackageRemoved.NotifyAsync(
                    this,
                    new ServiceEventPayloadOnPackageRemoved<CodePackage>(args.Package))
               .GetAwaiter()
               .GetResult();
        }

        private void OnContextOnConfigurationPackageAddedEvent(
            object sender,
            PackageAddedEventArgs<ConfigurationPackage> args)
        {
            this.OnConfigPackageAdded.NotifyAsync(
                    this,
                    new ServiceEventPayloadOnPackageAdded<ConfigurationPackage>(args.Package))
               .GetAwaiter()
               .GetResult();
        }

        private void OnContextOnConfigurationPackageModifiedEvent(
            object sender,
            PackageModifiedEventArgs<ConfigurationPackage> args)
        {
            this.OnConfigPackageModified.NotifyAsync(
                    this,
                    new ServiceEventPayloadOnPackageModified<ConfigurationPackage>(args.OldPackage, args.NewPackage))
               .GetAwaiter()
               .GetResult();
        }

        private void OnContextOnConfigurationPackageRemovedEvent(
            object sender,
            PackageRemovedEventArgs<ConfigurationPackage> args)
        {
            this.OnConfigPackageRemoved.NotifyAsync(
                    this,
                    new ServiceEventPayloadOnPackageRemoved<ConfigurationPackage>(args.Package))
               .GetAwaiter()
               .GetResult();
        }

        private void OnContextOnDataPackageAddedEvent(
            object sender,
            PackageAddedEventArgs<DataPackage> args)
        {
            this.OnDataPackageAdded.NotifyAsync(
                    this,
                    new ServiceEventPayloadOnPackageAdded<DataPackage>(args.Package))
               .GetAwaiter()
               .GetResult();
        }

        private void OnContextOnDataPackageModifiedEvent(
            object sender,
            PackageModifiedEventArgs<DataPackage> args)
        {
            this.OnDataPackageModified.NotifyAsync(
                    this,
                    new ServiceEventPayloadOnPackageModified<DataPackage>(args.OldPackage, args.NewPackage))
               .GetAwaiter()
               .GetResult();
        }

        private void OnContextOnDataPackageRemovedEvent(
            object sender,
            PackageRemovedEventArgs<DataPackage> args)
        {
            this.OnDataPackageRemoved.NotifyAsync(
                    this,
                    new ServiceEventPayloadOnPackageRemoved<DataPackage>(args.Package))
               .GetAwaiter()
               .GetResult();
        }
    }
}