# CoherentSolutions.Extensions.Hosting.ServiceFabric

![build & test](https://codebuild.us-east-1.amazonaws.com/badges?uuid=eyJlbmNyeXB0ZWREYXRhIjoiUnE1c1A2RGNaNDVMUFBLaFhPNkxDeUxkVXZBT1lGT1JCcm9RUnZWWmxDSmFXMnB5TDk5UHBOT1FDSUpBNXM1NW8zUGRKbmlqQVgwdGVnRStVa0luOTRRPSIsIml2UGFyYW1ldGVyU3BlYyI6ImVpN3hVTDd0UTh6RzJMeFQiLCJtYXRlcmlhbFNldFNlcmlhbCI6MX0%3D&branch=master)
[![nuget package](https://img.shields.io/badge/nuget-1.3.0-blue.svg)](https://www.nuget.org/packages/CoherentSolutions.Extensions.Hosting.ServiceFabric/1.3.0)

## About the Project

**CoherentSolutions.Extensions.Hosting.ServiceFabric** is an extension to existing [HostBuilder](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-2.1). The idea is to simplify configuration of [Reliable Services](https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-reliable-services-introduction) by removing unnecessary code and improving separation of concerns. 

## Getting Started

As usual, the easiest way to get started is to code something -> let's start from a new Reliable Service!

> **NOTE**
>
> Please note that current section doesn't explain all the aspects of the **CoherentSolutions.Extensions.Hosting.ServiceFabric**. The complete documentation is available on [project wiki][1]. 

In this section we would: 
1. Configure one stateful service 
2. Configure three endpoints (by configuring three listeners: aspnetcore, remoting and generic) 
3. Configure one background job (by configuring a delegate to run in `RunAsync`).

### Initial setup

Any program starts with the entry point and so does reliable services. When using **CoherentSolutions.Extensions.Hosting.ServiceFabric** the entry point setup starts with the new instance of the [HostBuilder](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-2.1) class and calls to `Build()` and `Run()` methods.

``` csharp
private static void Main(string[] args)
{
    new HostBuilder()
        .Build()
        .Run();
}
```

The service configuration starts with a call to `DefineStatefulService(...)` extension method. This method accepts an action where all service configuration is done.

 _You can find more details on [defining services](https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/wiki/defining-services) wiki page._

``` csharp
private static void Main(string[] args)
{
    new HostBuilder()
        .DefineStatefulService(serviceBuilder => { })
        .Build()
        .Run();
}
```

The first step in configuration of any service (stateful or stateless) in **CoherentSolutions.Extensions.Hosting.ServiceFabric** is to link service configuration to one of the service service types defined in the `ServiceManifest.xml`. 

``` xml
<ServiceManifest Name="ServicePkg" Version="1.0.0">
  <ServiceTypes>
    <StatefulServiceType ServiceTypeName="ServiceType" HasPersistedState="true" />
  </ServiceTypes>
</ServiceManifest>
```

This link is create by using `UseServiceType(...)` method.

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

This code is now ready to run but unfortunately it quite useless.

### Configuring Endpoints

Reliable Services can expose endpoints. This exposure is represented in form of service listeners configured when replica is build. The **CoherentSolutions.Extensions.Hosting.ServiceFabric** provides a simple way to configure: ASP.NET Core listeners, Service Fabric Remoting listeners and Generic listeners. 

In general listeners are configured using the same approach as was deomnstrated for services.
 
 _You can find more details on [defining listeners](https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/wiki/defining-listeners) wiki page._

#### ASP.NET Core

ASP.NET Core listener configuration starts with a call to `.DefineAspNetCoreListener(...)` method. This method accepts an action where all listener specific configuration is done.

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

The listener should be linked to one of the endpoint resources defined in the `ServiceManifest.xml`. 

``` xml
<ServiceManifest Name="ServicePkg" Version="1.0.0">
  <Resources>
    <Endpoints>
      <Endpoint Name="ServiceEndpoint" />
    </Endpoints>
  </Resources>
</ServiceManifest>
```

The linkage is done using `UseEndpoint(...)` method.

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

The listener is an infrastructure wrapper around the configuration process of `IWebHost`. The `IWebHost` configuration is done in `ConfigureWebHost(...)` method.

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

#### Remoting

Remoting listener has slightly different configuration curse than ASP.NET Core listener. The configuration starts from the call to `DefineRemotingListener(...)` and `UseEndpoint(...)` methods. 

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
                        });
            })
        .Build()
        .Run();
}
```

The remoting implementation consists from two parts: _remoting interface_ and _remoting implementation_. 

_remoting interface_ defines the API surface...

``` csharp
public interface IApiService : IService
{
    Task<string> GetVersionAsync();
}
```

... while _remoting class_ implements the API.

``` csharp
public class ApiServiceImpl : IApiService
{
    public Task<string> GetVersionAsync()
    {
        return Task.FromResult("1.0");
    }
}
```

_Remoting implementations_ is configured using `UseImplementation<T>(...)` method.

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

#### Generic Implementation

Generic listener allows custom listeners to be configured using the same approach as `DefineRemotingListener(...)` and `DefineAspNetCoreListener(...)`. 

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
                    .DefineGenericListener(...)
                        listenerBuilder =>
                        {
                            listenerBuilder
                                .UseEndpoint("ServiceEndpoint3")
                                .UseCommunicationListener(
                                    (context, name, provider) =>
                                    {
                                        return /* ICommunicationListener */;
                                    })
                        });
            })
        .Build()
        .Run();
}
```

### Configuring a Background Job

In **CoherentSolutions.Extensions.Hosting.ServiceFabric** background jobs and event handlers are represented in form of **Delegates**. The **Delegate** is configured using `DefineDefine(...)` method. 

 _You can find more details on [defining delegates](https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/wiki/defining-delegates) wiki page._

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
                    .DefineDelegate(delegateBuilder => { })
            })
        .Build()
        .Run();
}
```
The action to execute is configured by calling `UseDelegate(...)` method with accepts any `Action<...>` or `Func<..., Task>` compatible delegate.

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

## Documentation

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
