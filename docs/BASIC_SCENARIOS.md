# BASIC SCENARIOS

## Table of Contents:
* [Modify existing ASP.NET Core application for execution inside Service Fabric as Reliable Service](#modify-existing-asp.net-core-application-for-execution-inside-service-fabric-as-reliable-service)

## Modify existing ASP.NET Core application for execution inside Service Fabric as Reliable Service

This scenario explains how to modify an existing ASP.NET Core project to be both ASP.NET Core project and Reliable Service project.

### Preparing App.sln

* Open solution with ASP.NET Core application in Visual Studio *(DummyAspNetCore)*
* Create new Service Fabric Application project *(DummyApp)*
* Create new Service Fabric Stateful or Stateless ASP.NET Core project *(DummyAppService)*

  ![Create new Service Fabric Application & Service projects][1]

* Copy *DummyAppService\PackageRoot* directory to *DummyAspNetCore\PackageRoot* directory
* Replace *DummyAppService* service reference in *DummyApp* with reference to *DummyAspNetCore*

  ![Copy PackageRoot directory][2]

* Install NuGet package
  ```
  Install-Package CoherentSolutions.AspNetCore.ServiceFabric.Hosting
  ```

### Updating Program.cs

In *Program.cs* reference *CoherentSolutions.AspNetCore.ServiceFabric.Hosting* types.

``` csharp
using CoherentSolutions.AspNetCore.ServiceFabric.Hosting
``` 

Then split `BuildWebHost()` into `BuildWebHost()` and `BuildWebHostBuilder()`.

``` csharp
public static IWebHost BuildWebHost(string[] args) 
{
  return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseWebRoot(WEB_ROOT)
                .Build();
}
```

``` csharp
/*
  This method would never be actually called.

  Its is to integrate with Entity Framework Core command line interface.

  dotnet ef update database
*/
private static IWebHostBuilder BuildWebHost(string[] args) 
{
  return BuildWebHostBuilder(args).Build()
}

private static IWebHostBuilder BuildWebHostBuilder(string[] args) 
{
  return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseWebRoot(WEB_ROOT);
}
```

Update `Main()` to use `HostBuilder()`.

``` csharp
public static void Main(string[] args)
{
  BuildWebHost(args).Run();
}
```

``` csharp
public static void Main(string[] args)
{
  new HostBuilder()
    .UseWebHostBuilder(() => BuildWebHostBuilder(args))
    .ConfigureDefaultWebHost()
    .Build()
    .Run();
}
```

Initialize *Stateful* or *Stateless* service using `ConfigureStatelessServiceHost()` or `ConfigureStatefulServiceHost()`.

``` csharp
public static void Main(string[] args)
{
  new HostBuilder()
    /*
      Register a IWebHostBuilder factory to use for both 
      inside and outside Azure Service Fabric.
    */
    .UseWebHostBuilder(() => BuildWebHostBuilder(args))
    /*
      ConfigureWebHost() or ConfigureDefaultWebHost() on the HostBuilder is required to 
      setup self-hosted execution without it `Run()` method would throw an exception
      when executing outside of Azure Service Fabric.
    */
    .ConfigureDefaultWebHost()
    /*
      Configure self as Stateless service.

      NOTE: ConfigureStatelessServiceHost and ConfigureStatefulServiceHost are 
            mutually exclusive.
    */
    .ConfigureStatelessServiceHost( // ConfigureStatefulServiceHost
      serviceHostBuilder =>
      {
        serviceHostBuilder
          /*
            Set service type name (should be same as defined in the ServiceManifest.xml)
          */
          .UseServiceName("DummyAppServiceType")
          /*
            Define listener that uses Kestrel server and would use
            configuration of "ServiceEndpoint" endpoint defined in ServiceManifest.xml.
          */
          .DefineAspNetCoreListener(
            listenerBuilder =>
            {
              listenerBuilder
                .UseKestrel()
                .UseEndpointName("ServiceEndpoint");
            });
      })
    .Build()
    .Run();
}
```

[1]: images/bs-modify-existing-create-sf-project.gif "Create new Service Fabric Application & Service projects"
[2]: images/bs-modify-existing-copy-package-dir-and-replace-ref.gif "Copy PackageRoot directory"