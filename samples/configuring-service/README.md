# Defining Service

This sample demonstrates how to configure a `StatefulService` that exposes one external endpoint.

## What is inside?

**App.sfproj**

The application project is configured to have one `StatefulService` with `Singletone` partitioning schema and `MinReplicaSetSize` and `TargetReplicaSetSize` set to `1`.

**Service.csproj**

The service project implements one `StatefulService` that exposes one external endpoint `ServiceEndpoint` accessible by `/api/value` path. 

Files:
* `src/Controllers/ApiController.cs` - a controller for `ServiceEndpoint` request handling.
* `src/Startup.cs` - a startup configuration for `IWebHost` used to power `ServiceEndpoint`.
* `Program.cs` - an entry point and configuration of `StatefulService`.

## Key points

**Program.cs**

The **CoherentSolutions.Extensions.Hosting.ServiceFabric** is implemented as a set of extension methods for ASP.NET Core `HostBuilder` class. The entry point is always started with the new instance of `HostBuilder` and calls to `Build()` and `Run()` methods. 

``` csharp
new HostBuilder()
    .Build()
    .Run();
```

Service configuration is inside the `DefineStatefulService(...)` method.

``` csharp
new HostBuilder()
    .DefineStatefulService(serviceBuilder => { })
    .Build()
    .Run();
```

The endpoint configuration is done inside `DefineAspNetCoreListener(...)` method.

``` csharp
new HostBuilder()
    .DefineStatefulService(
        serviceBuilder =>
        {
            serviceBuilder
                .DefineAspNetCoreListener(listenerBuilder => { });
        })
    .Build()
    .Run();
```

## How to use?

**Visual Studio**

1. Press `F5` and deploy the application.
2. Invoke `GetValue` controller action by navigating to `ServiceEndpoint` and appending `/api/value` at the end.

The request output should be: `Value from ApiController`. 

## Conclusion

For more information please check this [wiki article][1] and explore the source code! 

If you have a suggestion or found an issue please consider [reporting it][2].

[1]: https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/wiki/Defining-Services
[2]: https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/issues