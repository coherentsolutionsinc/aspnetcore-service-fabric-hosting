# About the project

**CoherentSolutions.Extensions.Hosting.ServiceFabric** is a simple project that changes a way how you setup your Reliable Services. This package integrates with [HostBuilder][3] and allows to configure **Reliable Services** with ease using set of builder classes.

The **CoherentSolutions.Extensions.Hosting.ServiceFabric** supports:
* Configuration of Stateful or Stateless service without a need to implement any infrastructure classes (like `EventSource`, descendant from `StatefulService` or `StatelessService` etc.)
* Integration with well known packages and components:
    * **Microsoft.Extensions.Logging** i.e. you can use `ILogger` and all of the events / scopes will be redirected to dedicated Event Source. Please see [article][1] for details.
    * **Microsoft.Extensions.DependencyInjection** i.e. you can request and register dependencies using standard approach. Please see [article][2] for details.

## How to start?

* Read about [HostBuilder][1]
* Install [CoherentSolutions.Extensions.Hosting.ServiceFabric][11] package
* Configure a service
    ``` csharp
    new HostBuilder()
        .ConfigureStatefulService(
            serviceBuilder =>
            {
                // Configure stateful service
            })
        .Build()
        .Run();
    ```
* Enjoy... :)

## Documentation

All project related information can be found on [wiki][12] (_in progress_).

You are also encouraged to check sample projects:
* [Configure Service][2] - this sample demonstrated how to configure a simple stateful service with one **aspnetcore** listener using `HostBuilder` and **CoherentSolutions.Extensions.Hosting.ServiceFabric**.
* [Dependency Injection][3] - this sample demonstrates how to use dependency injection support provided by **CoherentSolutions.Extensions.Hosting.ServiceFabric** to share services registrations between **aspnetcore** and **remoting** listeners of simple stateful service.

## Future & Past

For our plans on future releases please refer to wiki [Project Roadmap][5] page.

For information on past releases please refer to wiki [Version History][6] page.

## Contributing

For additional information on how to contribute to this project, please see [CONTRIBUTING.md][7].

## Authors

This project is owned and maintained by [Coherent Solutions][8].

## License

This project is licensed under the MS-PL License - see the [LICENSE.md][9] for details.

[1]:  https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-2.1 "docs.microsoft.com HostBuilder"
[2]:  https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/tree/mastersamples/configure-service "sample: Configure Services"
[3]:  https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/tree/master/samples/dependency-injection "sample: Dependency Injection"
[5]:  https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/wiki/Roadmap "wiki: Project roadmap"
[6]:  https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/wiki/VersionHistory "wiki: Version History"
[7]:  CONTRIBUTING.md "Contributing"
[8]:  https://www.coherentsolutions.com/ "Coherent Solutions Inc."
[9]:  https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/blob/master/LICENSE.md "License"
[11]: https://www.nuget.org/packages/CoherentSolutions.Extensions.Hosting.ServiceFabric "NuGet package"
[12]: https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/wiki "Project wiki"

