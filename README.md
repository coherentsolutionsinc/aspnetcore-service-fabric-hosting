## About the project

**CoherentSolutions.Extensions.Hosting.ServiceFabric** is a simple project that changes a way how you setup your Reliable Services. This package integrates with [HostBuilder][1] and allows to configure **Reliable Services** with ease using set of builder classes.

## Getting Started

* Read about [HostBuilder][1]
* Install [CoherentSolutions.Extensions.Hosting.ServiceFabric][2] package
* Configure a service
    ``` csharp
    new HostBuilder()
        .DefineStatefulService(
            serviceBuilder =>
            {
                // Configure stateful service
            })
        .Build()
        .Run();
    ```
* Enjoy... :)

If you need more - we have both documentation and samples to get started.

### Documentation

The project documentation can be found on [wiki](https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/wiki).

#### Building Blocks

* [Defining Services][3] - this document describes how to define one or more services.
* [Defining Delegates][4] - this document describes how to define delegates used to execute service's background jobs.
* [Defining Listeners][5] - this document describes how to define listener used to handle service's requests.

#### Infrastructure Support

* [Using Logging][6] - this document describes how to use build-in logging capabilities and how it is integrated with the default logging pipeline and ETW.
* [Using Dependency Injection][7] - this document describes how to use build-in dependency injection capabilities and how it is integrated with the default services registration flow.

### Samples

* [Defining Service][8] - this sample demonstrates how to configure simple service.
* [Defining Multiple Service][9] - this sample demonstrates how to configure multiple services at once.
* [Hierarchical Services Registration][10] - this sample demonstrates how to use hierarchical service registrations to share registration across service listeners.

## Integration

**CoherentSolutions.Extensions.Hosting.ServiceFabric** supports:
* Configuration of Stateful or Stateless service without a need to implement any infrastructure classes (like `EventSource`, descendant from `StatefulService` or `StatelessService` etc.)
* Integration with well known packages and components:
    * **Microsoft.Extensions.Logging**i.e. you can use `ILogger` and all of the events / scopes will be redirected to dedicated Event Source.
    * **Microsoft.Extensions.DependencyInjection**i.e. you can request and register dependencies using standard dependency injection container.

## What's new?

For information on past releases please refer to wiki [Version History][11] page.

## Contributing

For additional information on how to contribute to this project, please see [CONTRIBUTING.md][12].

## Authors

This project is owned and maintained by [Coherent Solutions][13].

## License

This project is licensed under the MS-PL License - see the [LICENSE.md][14] for details.

[1]:  https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-2.1 "docs.microsoft.com HostBuilder"
[2]:  https://www.nuget.org/packages/CoherentSolutions.Extensions.Hosting.ServiceFabric "NuGet package"
[3]:  https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/wiki/Defining-Services
[4]:  https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/wiki/Defining-Delegates
[5]:  https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/wiki/Defining-Listeners
[6]:  https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/wiki/Logging
[7]:  https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/wiki/Dependency-Injection
[8]:  https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/tree/master/samples/defining-service
[9]:  https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/tree/master/samples/defining-multiple-services
[10]: https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/tree/master/samples/hierarchical-services-registration
[11]:  https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/wiki/VersionHistory "wiki: Version History"
[12]:  CONTRIBUTING.md "Contributing"
[13]:  https://www.coherentsolutions.com/ "Coherent Solutions Inc."
[14]:  https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/blob/master/LICENSE.md "License"
