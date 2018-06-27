## About the Project

**CoherentSolutions.Extensions.Hosting.ServiceFabric** changes a way how you setup entry point for your **Reliable Services**. The main idea was to remove unnecessary code, improve separation of concern and make the process much easier and convenient. 

## Getting Started

To understand it right let's compare how things done with and without **CoherentSolutions.Extensions.Hosting.ServiceFabric**. 

> **NOTE**
>
> Please note that current section doesn't contain explanation of all the aspects of the **CoherentSolutions.Extensions.Hosting.ServiceFabric** package. The full information can be found on [project wiki][1].

### Setting up Reliable Service **without** CoherentSolutions.Extensions.Hosting.ServiceFabric

#### Initial Setup

When a new Service Fabric project is created. Visual Studio automatically creates a few things:

1. A class that is inherited from `StatefulService` or `StatelessService`.
    ``` csharp
    internal sealed class Service : StatefulService
    {
        public Service(StatefulServiceContext context) : base(context) { }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new ServiceReplicaListener[]
            {
                new ServiceReplicaListener(serviceContext =>
                    new KestrelCommunicationListener(serviceContext, (url, listener) =>
                    {
                        ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting Kestrel on {url}");

                        return new WebHostBuilder()
                                    .UseKestrel()
                                    .ConfigureServices(
                                        services => services
                                            .AddSingleton<StatefulServiceContext>(serviceContext)
                                            .AddSingleton<IReliableStateManager>(this.StateManager))
                                    .UseContentRoot(Directory.GetCurrentDirectory())
                                    .UseStartup<Startup>()
                                    .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.UseUniqueServiceUrl)
                                    .UseUrls(url)
                                    .Build();
                    }))
            };
        }
    }
    ```
2. A class that represents an ETW event source.
    ``` csharp
    [EventSource(Name = "MyCompany-App-Service")]
    internal sealed class ServiceEventSource : EventSource
    {
        ... /* skip the body here */
    }
    ```
3. The program entry point is the following:
    ``` csharp
    private static void Main()
    {
        try
        {
            ServiceRuntime.RegisterServiceAsync("ServiceType", context => new Service(context)).GetAwaiter().GetResult();

            ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(Service).Name);

            Thread.Sleep(Timeout.Infinite);
        }
        catch (Exception e)
        {
            ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
            throw;
        }
    }
    ```

#### What if we introduce additional **remoting** listener?

Define the interface:

``` csharp
internal interface IServiceRemoting : IService
{
    Task<string> GetVersion();
}
```

Implement it by `Service` class and add new `FabricTransportServiceRemotingListener` to list of listeners.

``` csharp
internal sealed class Service : StatefulService, IServiceRemoting
{
    public Service(StatefulServiceContext context) : base(context) { ... }

    protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
    {
        return return new ServiceReplicaListener[]
            {
                ...
                new ServiceReplicaListener(serviceContext =>
                    new FabricTransportServiceRemotingListener(serviceContext, this))
            };
    }

    public Task<string> GetVersion()
    {
        return Task.FromResult(string.Empty);
    }
}
```

#### What if we introduce background job?

``` csharp
internal sealed class Service : StatefulService, IServiceRemoting
{
    public Service(StatefulServiceContext context) : base(context) { ... }

    protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners() { ... }

    protected override Task RunAsync(
        CancellationToken cancellationToken)
    {
        return Task.FromResult(string.Empty);
    }

    public Task<string> GetVersion() { ... }
}
```

#### What if we need to share some common objects or dependencies between all of these things?

This should be implemented by hand. There is no build-in support for such things.

### Setting up Reliable Service **with** CoherentSolutions.Extensions.Hosting.ServiceFabric

#### Initial Setup

The first thing to notice is that there is no need to `Service` and `ServiceEventSource` classes anymore.

All initial setup can be replaced with the following code in the entry point:

``` csharp
private static void Main(string[] args)
{
    new HostBuilder()
        .DefineStatefulService(
            serviceBuilder =>
            {
                serviceBuilder
                    .UseServiceType("ServiceType")
                    .DefineAspNetCoreListener(
                        listenerBuilder =>
                        {
                            listenerBuilder
                                .UseEndpointName("ServiceEndpoint")
                                .UseUniqueServiceUrlIntegration()
                                .ConfigureWebHost(webHostBuilder => { webHostBuilder.UseStartup<Startup>(); });
                        });
            })
        .Build()
        .Run();
}
```

#### What if we introduce additional **remoting** listener?

Define the interface:

``` csharp
internal interface IServiceRemoting : IService
{
    Task<string> GetVersion();
}
```

Define a separate class `ServiceRemotingImpl` that implements `IServiceRemoting`.

``` csharp
internal sealed class ServiceRemotingImpl : IServiceRemoting
{
    public ServiceRemotingImpl(StatefulServiceContext context) : base(context) { ... }

    public Task<string> GetVersion()
    {
        return Task.FromResult(string.Empty);
    }
}
```

Define additional listener in entry point:

``` csharp
private static void Main(string[] args)
{
    new HostBuilder()
        .DefineStatefulService(
            serviceBuilder =>
            {
                serviceBuilder
                    .UseServiceType(...)
                    .DefineAspNetCoreListener(...)
                    .DefineRemotingListener(
                        listenerBuilder =>
                        {
                            listenerBuilder
                                .UseEndpointName("ServiceEndpoint2")
                                .UseImplementation<ServiceRemotingImpl>()
                        });
            })
        .Build()
        .Run();
}
```

#### What if we introduce background job?

Background jobs in **CoherentSolutions.Extensions.Hosting.ServiceFabric** are represented as **delegates**. The delegate can be represented by any `Action<...>` or `Func<..., Task>`.

``` csharp
private static void Main(string[] args)
{
    new HostBuilder()
        .DefineStatefulService(
            serviceBuilder =>
            {
                serviceBuilder
                    .UseServiceType(...)
                    .DefineAspNetCoreListener(...)
                    .DefineRemotingListener(...)
                    .DefineDelegate(
                        delegateBuilder =>
                        {
                            delegateBuilder.UseDelegate(...)
                        })
            })
        .Build()
        .Run();
}
```

#### What if we need to share some common objects or dependencies between all of these things?

That is pretty easy. There are several way of doing this but this is the most obvious.

``` csharp
private static void Main(string[] args)
{
    new HostBuilder()
        .ConfigureServices(
            services =>
            {
                /* register services */
            })
        .DefineStatefulService(
            serviceBuilder =>
            {
                serviceBuilder
                    .UseServiceType(...)
                    .DefineAspNetCoreListener(...)
                    .DefineRemotingListener(...)
                    .DefineDelegate(
                        delegateBuilder =>
                        {
                            delegateBuilder.UseDelegate(...)
                        })
            })
        .Build()
        .Run();
}
```

The trick here is that all components are linked and **CoherentSolutions.Extensions.Hosting.ServiceFabric** has special mechanics to manage hierarchical services registration.

### Documentation

For project documentation please refer to [project wiki][1].

## What's new?

For information on past releases please refer to [version history][2] page.

## Contributing

For additional information on how to contribute to this project, please see [CONTRIBUTING.md][3].

## Authors

This project is owned and maintained by [Coherent Solutions][4].

## License

This project is licensed under the MS-PL License - see the [LICENSE.md][5] for details.

[1]:  https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/wiki "wiki: Home"
[2]:  https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/wiki/VersionHistory "wiki: Version History"
[3]:  CONTRIBUTING.md "Contributing"
[4]:  https://www.coherentsolutions.com/ "Coherent Solutions Inc."
[5]:  https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/blob/master/LICENSE.md "License"