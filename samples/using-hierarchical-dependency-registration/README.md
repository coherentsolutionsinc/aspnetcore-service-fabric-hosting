# Hierarchical Services Registration

This sample demonstrates how to use service registration downstreaming without violating object lifetime constraints.

## What is inside?

**App.sfproj**

The application project is configured to have one `StatefulService` with `Singletone` partitioning schema and `MinReplicaSetSize` and `TargetReplicaSetSize` set to `1`.

**Service.csproj**

The service project implements one `StatefulService` that exposes two external endpoints `FirstEndpoint` and `SecondEndpoint` accessible by `/api/value` path. 

Files:
* `src/Controllers/ApiController.cs` - a controller for `ServiceEndpoint` request handling.
* `src/IPersonalService.cs` - an interface of the personal service. The instance of personal service should be different for each endpoint.
* `src/ISharedService.cs` - an interface of the shared service. The instance of shared service should be the same for both endpoints.
* `src/PersonalService.cs` - an implementation of `IPersonalService.cs`. The implementation returns a current instance hashcode as the implementation of `GetPersonalValue(...)` method.
* `src/SharedService.cs` - an implementation of `ISharedService.cs`. The implementation returns a current instance hashcode as the implementation of `GetSharedValue(...)` method.
* `src/Startup.cs` - a startup configuration for `IWebHost` used to power both `FirstEndpoint` and `SecondEndpoint`.
* `Program.cs` - an entry point and configuration of `StatefulService`.

## Key points

**Program.cs**

The major key point is to demonstrate that service registered on the higher level are automatically downstreamed to the service and its endpoints. 

In `ConfigureServices(...)` method of `HostBuilder` class there is a configuration of two services: `SharedService` and `PersonalService`. The `SharedService` is configured as singletone service and `PersonalService` is configured as transient service.

``` csharp
new HostBuilder()
    .ConfigureServices(
        services =>
        {
            // Singleton
            services.AddSingleton<ISharedService, SharedService>();

            // Transient
            services.AddTransient<IPersonalService, PersonalService>();
        })
    .DefineStatefulService(
        serviceHostBuilder =>
        {
            serviceHostBuilder
                .DefineAspNetCoreListener(
                    listenerBuilder =>
                    {
                        listenerBuilder
                            .UseEndpoint("FirstEndpoint")
                            .ConfigureWebHost(webHostBuilder => { webHostBuilder.UseStartup<Startup>(); });
                    })
                .DefineAspNetCoreListener(
                    listenerBuilder =>
                    {
                        listenerBuilder
                            .UseEndpoint("SecondEndpoint")
                            .ConfigureWebHost(webHostBuilder => { webHostBuilder.UseStartup<Startup>(); });
                    });
        })
    .Build()
    .Run();
```

Both of these dependencies are automatically downstreamed to `service - endpoints - webhosts` and become available for consumption in controller.

``` csharp
[Route("api")]
public class ApiController : ControllerBase
{
    private readonly IPersonalService personalService;
    private readonly ISharedService sharedService;

    public ApiController(
        ISharedService sharedService,
        IPersonalService personalService)
    {
        this.sharedService = sharedService;
        this.personalService = personalService;
    }

    [HttpGet]
    [Route("value")]
    public Task<string> GetValue()
    {
        return Task.FromResult(
            $"Shared: {this.sharedService.GetSharedValue()}; Personal: {this.personalService.GetPersonalValue()}");
    }
}
```

The advantage of hierarchical dependency injection is that the singleton registration of `SharedService` results in the same instance resolved in both endpoints. At the same time `PersonalService` is resolved as different instances because it was registered with transient lifetime configuration.

## How to use?

1. Press `F5` and deploy the application.
2. Invoke `GetValue` controller action by navigating to `FirstEndpoint` and appending `/api/value` at the end.
3. Invoke `GetValue` controller action by navigating to `SecondsEndpoint` and appending `/api/value` at the end.

The result output should look like:

First Endpoint | Second Endpoint
--- | ---
Shared: Hash: **45937921**; Personal: Hash: 15599987 | Shared: Hash: **45937921**; Personal: Hash: 3649268

> **Developer's comment:**
>
> It is important that "Shared: Hash:" values should **be equal** but "Personal: Hash:" **shouldn't**.

## Conclusion

For more information please check this [wiki article][1] and explore the source code! 

If you have a suggestion or found an issue please consider [reporting it][2].

[1]: https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/wiki/Hierarchical-Services-Registration
[2]: https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/issues