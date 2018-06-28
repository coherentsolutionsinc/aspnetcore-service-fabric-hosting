## About the Project

**CoherentSolutions.Extensions.Hosting.ServiceFabric** changes a way how you setup entry point for your **Reliable Services**. The main idea is to remove unnecessary code, improve separation of concern and make the process much easier and convenient. 

## Getting Started

The easiest way to get started with **CoherentSolutions.Extensions.Hosting.ServiceFabric** is to setup **Reliable Service**!

> **NOTE**
>
> Please note that current section doesn't contain explanation of all the aspects of the **CoherentSolutions.Extensions.Hosting.ServiceFabric** package. The full information can be found on [project wiki][1].

In the **Getting Started** section we would setup entry point for Stateful Service with one **aspnetcore listener** and one **remoting listener**. The service should also execute background job and use shared services in all the listed components.

### Initial Setup

The setup process always starts with the usage of [HostBuilder](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-2.1) in the entry point.

``` csharp
private static void Main(string[] args)
{
    new HostBuilder()
        .Build()
        .Run();
}
```

The configuration of any services starts with the call to `DefineStatefulService(...)` or `DefineStatelessService(...)` method. The method access a delegate with single argument: `serviceBuilder`.

``` csharp
private static void Main(string[] args)
{
    new HostBuilder()
        .DefineStatefulService(serviceBuilder => { })
        .Build()
        .Run();
}
```

> Starting from this point all configuration of the service is done through `serviceBuilder`.

Now service configuration has to be bound to one of the `ServiceType`'s defined in the `ServiceManifest.xml`. This can be done by calling `.UseServiceType(...)` method with the name of `ServiceType`.

``` csharp
private static void Main(string[] args)
{
    new HostBuilder()
        .DefineStatefulService(
            serviceBuilder =>
            {
                serviceBuilder.UseServiceType("ServiceType");
            })
        .Build()
        .Run();
}
```

The initial service setup is done.

### Setting up **aspnetcore listener**

The process of setting up **aspnetcore listener** is very similar to service setup. The configuration of **aspnetcore listener** starts with a call to `.DefineAspNetCoreListener(...)` method. This method access a delegate with single argument: `listenerBuilder`.

``` csharp
private static void Main(string[] args)
{
    new HostBuilder()
        .DefineStatefulService(
            serviceBuilder =>
            {
                serviceBuilder
                    .UseServiceType("ServiceType")
                    .DefineAspNetCoreListener(listenerBuilder => { });
            })
        .Build()
        .Run();
}
```

> Starting from this point all configuration of the **aspnetcore listener** is done through `listenerBuilder`.

Now listener configuration has to be bound to one of the endpoint resources's defined in the `ServiceManifest.xml`. This can be done by calling `.UseEndpointName(...)` method with the name of `ServiceType`.

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