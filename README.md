# CoherentSolutions.Extensions.Hosting.ServiceFabric

[![Build Status](https://dev.azure.com/coherent-oss/oss-pipelines/_apis/build/status/coherentsolutionsinc.aspnetcore-service-fabric-hosting?branchName=master)](https://dev.azure.com/coherent-oss/oss-pipelines/_build/latest?definitionId=2&branchName=master)
[![nuget package](https://img.shields.io/badge/nuget-1.5.1-blue.svg)](https://www.nuget.org/packages/CoherentSolutions.Extensions.Hosting.ServiceFabric/1.5.1)

## About the Project

**CoherentSolutions.Extensions.Hosting.ServiceFabric** is an extension to existing [HostBuilder](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-2.1). The idea is to simplify configuration of [Reliable Services](https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-reliable-services-introduction) by removing unnecessary code and improving separation of concerns. 

## Getting Started

As usual, the easiest way to get started is to code &rarr; a new Reliable Service!

> **NOTE**
>
> This section doesn't present and explains all features / aspects of the **CoherentSolutions.Extensions.Hosting.ServiceFabric**. The complete documentation is available on [project wiki][1]. 

In this section we **will**: 
1. Configure one stateful service 
2. Configure three endpoints (_by configuring three listeners: ASP.NET Core, Remoting and Generic_) 
3. Configure one background job (_by configuring a delegate to run on `RunAsync` lifecycle event_).

### Initial setup

All programs start from _entry point_ and so do Reliable Services. When using **CoherentSolutions.Extensions.Hosting.ServiceFabric** application entry point starts with new instance of the [HostBuilder](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-2.1) class followed by calls to `Build()` and `Run()` methods.

``` csharp
private static void Main(string[] args)
{
    new HostBuilder()
        .Build()
        .Run();
}
```

Reliable Service's configuration is done within configuration action accepted by `DefineStatefulService(...)` or `DefineStatelessService(...)` extension methods.

> **NOTE**
>
> _You can find more details on [defining services](https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/wiki/defining-services) wiki page._

``` csharp
private static void Main(string[] args)
{
    new HostBuilder()
        .DefineStatefulService(serviceBuilder => { })
        .Build()
        .Run();
}
```

The first step when configuring a service (_no matter stateful or stateless_) using **CoherentSolutions.Extensions.Hosting.ServiceFabric** is to "link" configurable service to one of the [ServiceTypes](https://olegkarasik.wordpress.com/2018/10/03/service-fabric-handbook/#object-model-service-type-and-service) declared in the `ServiceManifest.xml`. 

``` xml
<ServiceManifest Name="ServicePkg" Version="1.0.0">
  <ServiceTypes>
    <StatefulServiceType ServiceTypeName="ServiceType" HasPersistedState="true" />
  </ServiceTypes>
</ServiceManifest>
```

The "link" is create using `UseServiceType(...)` method.

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

This code is now ready to run. **CoherentSolutions.Extensions.Hosting.ServiceFabric** will do all the plumbing required to start the service.

Unfortunatelly it's current implementation is empty and does nothing.

### Configuring Endpoints

Reliable Services can expose endpoints. This exposure is represented in form of listeners configured on replica startup. The **CoherentSolutions.Extensions.Hosting.ServiceFabric** provides a simple way to configure: ASP.NET Core, Remoting listeners and Generic listeners. 

All listeners are configured using the same flow as was demonstrated for services.
 
> **NOTE**
>
> _You can find more details on [defining listeners](https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/wiki/defining-listeners) wiki page._

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
                            listenerBuilder.UseEndpoint("ServiceEndpoint");
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
                                .UseEndpoint("ServiceEndpoint2");
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
                                .UseImplementation<ApiServiceImpl>();
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
                    .DefineDelegate(delegateBuilder => { });
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

This project is licensed under the MIT License - see the [LICENSE.md][5] for details.

## See Also

Besides this project [Coherent Solutions][4] also maintains a few more open source projects and workshops:

**Projects**

* [service-fabric-run-tests][6] - is a Docker image designed to run Service Fabric oriented unit-tests (.NET Core) on Linux.
* [CoherentSolutions.Extensions.Configuration.AnyWhere][7] - is an extension to [Microsoft.Extensions.Configuration](https://www.nuget.org/packages/Microsoft.Extensions.Configuration). This extension allows application to configure it's configuration sources using environment variables.

**Workshops**

* [Building Self-Driving Car Architecture with Robot Operating System][8] - is a workshop to introduce attendee to [Robot Operating System][9] (ROS) design and create simple self-driving car architecture.

[1]:  https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/wiki "wiki: Home"
[2]:  https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/wiki/Version-History "wiki: Version History"
[3]:  CONTRIBUTING.md "Contributing"
[4]:  https://www.coherentsolutions.com/ "Coherent Solutions Inc."
[5]:  https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/blob/master/LICENSE.md "License"
[6]:  https://github.com/coherentsolutionsinc/service-fabric-run-tests
[7]:  https://github.com/coherentsolutionsinc/anywhere-configuration "AnyWhere Configuration"
[8]:  https://github.com/coherentsolutionsinc/issoft-insights-2019-sdc-carla-ros
[9]:  http://wiki.ros.org/
