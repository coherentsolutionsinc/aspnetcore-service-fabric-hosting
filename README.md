## About the Project

**CoherentSolutions.Extensions.Hosting.ServiceFabric** changes a way how you setup entry point for your **Reliable Services**. The main idea is to remove unnecessary code, improve separation of concern and make the process much easier and convenient. 

## Getting Started

The easiest way to get started with **CoherentSolutions.Extensions.Hosting.ServiceFabric** is to setup **Reliable Service**!

> **NOTE**
>
> Please note that current section doesn't contain explanation of all the aspects of the **CoherentSolutions.Extensions.Hosting.ServiceFabric** package. The full information can be found on [project wiki][1].

In the **Getting Started** section we would setup entry point for Stateful Service that has:
* One **aspnetcore listener** (Kestrel)
* One **remoting listener**
* One background job

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

The configuration of any service starts by calling `DefineStatefulService(...)` or `DefineStatelessService(...)` method. The method accepts a delegate with single argument: `serviceBuilder`.

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

Now service configuration has to be bound to one of the service type's defined in the `ServiceManifest.xml`. This can be done by calling `.UseServiceType(...)` method with the name of service type.

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

The process of setting up **aspnetcore listener** is very similar to service setup. The configuration of **aspnetcore listener** starts by calling `.DefineAspNetCoreListener(...)` method. This method accepts a delegate with single argument: `listenerBuilder`.

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

Now listener configuration has to be bound to one of the endpoint resources's defined in the `ServiceManifest.xml`. This can be done by calling `.UseEndpoint(...)` method with the name of endpoint resource.

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
                            listenerBuilder.UseEndpoint("ServiceEndpoint")
                        });
            })
        .Build()
        .Run();
}
```

The configuration of underlying `IWebHost` is done using `.ConfigureWebHost(...)` method.

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
                                .UseEndpoint("ServiceEndpoint")
                                .ConfigureWebHost(webHostBuilder => { webHostBuilder.UseStartup<Startup>(); });
                        });
            })
        .Build()
        .Run();
}
```

### Setting up **remoting listener**

Usage of **remoting listener** requires _implementation interface_ that defines remoting methods accessible to client and _implementation class_ that implements the logic behind _implementation interface_.

Example of _implementation interface_

``` csharp
public interface IApiService : IService
{
    Task<string> GetVersionAsync();
}
```

Example of _implementation class_

``` csharp
public class ApiServiceImpl : IApiService
{
    public Task<string> GetVersionAsync()
    {
        return Task.FromResult("1.0");
    }
}
```

The configuration of **remoting listener** starts by calling `.DefineRemotingListener(...)` method. This method accepts a delegate with single argument: `listenerBuilder`. Similarly to **aspnetcore listener** configuration **remoting listener** configuration has to be bound to one of the endpoint resources's defined in the `ServiceManifest.xml`. This can be done by calling `.UseEndpoint(...)` method with the name of endpoint resource.

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
                            listenerBuilder.UseEndpoint("ServiceEndpoint2");
                        });
            })
        .Build()
        .Run();
}
```

Remoting implementation is configured by calling `.UseImplementation<T>(...)` method.

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
                                .UseImplementation<ApiServiceImpl>()
                        });
            })
        .Build()
        .Run();
}
```

### Setting up background job

Background jobs in **CoherentSolutions.Extensions.Hosting.ServiceFabric** are represented as **delegates**. The configuration of **delegate** starts by calling `.DefineDefine(...)` method. This method accepts a delegate with single argument: `delegateBuilder`. 

The background job is configured by calling `.UseDelegate(...)` method that accepts any `Action<...>` or `Func<..., Task>`.

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
                            delegateBuilder.UseDelegate(async () => await Database.MigrateDataAsync());
                        })
            })
        .Build()
        .Run();
}
```

### Conclusion

That is it :)

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