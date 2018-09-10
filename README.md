# CoherentSolutions.Extensions.Hosting.ServiceFabric

![build & test](https://codebuild.us-east-1.amazonaws.com/badges?uuid=eyJlbmNyeXB0ZWREYXRhIjoiUnE1c1A2RGNaNDVMUFBLaFhPNkxDeUxkVXZBT1lGT1JCcm9RUnZWWmxDSmFXMnB5TDk5UHBOT1FDSUpBNXM1NW8zUGRKbmlqQVgwdGVnRStVa0luOTRRPSIsIml2UGFyYW1ldGVyU3BlYyI6ImVpN3hVTDd0UTh6RzJMeFQiLCJtYXRlcmlhbFNldFNlcmlhbCI6MX0%3D&branch=master)
[![nuget package](https://img.shields.io/badge/nuget-1.2.1-blue.svg)](https://www.nuget.org/packages/CoherentSolutions.Extensions.Hosting.ServiceFabric/1.2.1)

## About the Project

**CoherentSolutions.Extensions.Hosting.ServiceFabric** changes a way how you develop **Reliable Services**. The main idea is simplify development process as much as possible by removing unnecessary code, improving separation of concern and making the whole process much easier and convenient by introducing set of friendly builders. 

## Getting Started

As usual the easiest way to get started is to code something. Let's create a new **reliable service**!

> **NOTE**
>
> Please note that current section doesn't contain explanation of all the aspects of the **CoherentSolutions.Extensions.Hosting.ServiceFabric**. The full documentation can be found on [project wiki][1]. 

In the **Getting Started** section we would develop Stateful Service with one **aspnetcore listener** (Kestrel), one **remoting listener** and one background job.

Let's go!

All programs start with the entry point so the development of **reliable service** does too. When using **CoherentSolutions.Extensions.Hosting.ServiceFabric** the entry point setup starts from new instance of the [HostBuilder](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-2.1) class and default calls to `Build()` and `Run()`.

``` csharp
private static void Main(string[] args)
{
    new HostBuilder()
        .Build()
        .Run();
}
```

Configuration of any service starts by calling `DefineStatefulService(...)` or `DefineStatelessService(...)` method. This method creates a new **service** building block ...

``` csharp
private static void Main(string[] args)
{
    new HostBuilder()
        .DefineStatefulService(serviceBuilder => { })
        .Build()
        .Run();
}
```

... that should be bound to the service type from the `ServiceManifest.xml`. This can be done by calling `.UseServiceType(...)` method with the name of service type.

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

Now when the initial setup is done we can start continue configuring **aspnetcore listener**. This is done by calling `.DefineAspNetCoreListener(...)` method. 

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

This method creates new **listener** building block that gets bound to endpoint resource from `ServiceManifest.xml` by calling `.UseEndpoint(...)` method with the name of endpoint resource.

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

Using `.ConfigureWebHost(...)` method we can configure underlying `IWebHost`.

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

Configuration of **remoting listener** requires _implementation interface_ that defines remoting methods accessible to client and _implementation class_ that implements the logic behind _implementation interface_.


Simple _implementation interface_ ...

``` csharp
public interface IApiService : IService
{
    Task<string> GetVersionAsync();
}
```

... and its _implementation class_

``` csharp
public class ApiServiceImpl : IApiService
{
    public Task<string> GetVersionAsync()
    {
        return Task.FromResult("1.0");
    }
}
```

Configuration of **remoting listener** is done by `.DefineRemotingListener(...)` method where listener is bound to endpoint resource ... 

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

... and remoting implementation is set by calling `.UseImplementation<T>(...)` method.

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
                                .UseEndpoint("ServiceEndpoint2")
                                .UseImplementation<ApiServiceImpl>()
                        });
            })
        .Build()
        .Run();
}
```

The listeners are successfully configured. All is left is a background job.


Background jobs in **CoherentSolutions.Extensions.Hosting.ServiceFabric** are represented as **delegates** which are configured by calling `.DefineDefine(...)` where using `.UseDelegate(...)` method we can set any `Action<...>` or `Func<..., Task>` to be executed as background job.

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
[2]:  https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/wiki/Version-History "wiki: Version History"
[3]:  CONTRIBUTING.md "Contributing"
[4]:  https://www.coherentsolutions.com/ "Coherent Solutions Inc."
[5]:  https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/blob/master/LICENSE.md "License"
