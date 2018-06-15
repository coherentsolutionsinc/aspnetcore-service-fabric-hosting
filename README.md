# About the project

**CoherentSolutions.AspNetCore.ServiceFabric.Hosting** is a simple project that changes a way how you setup your Reliable Services. This package integrates with [HostBuilder][3] and allows to configure **Reliable Services** with ease using set of builder classes.

The **CoherentSolutions.AspNetCore.ServiceFabric.Hosting** supports:
* Configuration of Stateful or Stateless service without a need to implement any infrastructure classes (like `EventSource`, descendant from `StatefulService` or `StatelessService` etc.)
* Integration with well known packages and components:
    * **Microsoft.Extensions.Logging** i.e. you can use `ILogger` and all of the events / scopes will be redirected to dedicated Event Source. Please see [article][1] for details.
    * **Microsoft.Extensions.DependencyInjection** i.e. you can request and register dependencies using standard approach. Please see [article][2] for details.

## How to start?

* Read about [HostBuilder][1]
* Install [CoherentSolutions.AspNetCore.ServiceFabric.Hosting][11] package
* Enjoy... :)

## Documentation

All project related information can be found on [wiki][12] (_in progress_).

There are also samples gallery:
* [service-configuration][2] - this sample demonstrated how to configure stateful service using `HostBuilder` and **CoherentSolutions.AspNetCore.ServiceFabric.Hosting**.
* [hierarchical services registration][3] - this sample demonstrates how **hierarchical services registration** feature works by configuring stateful service with **aspnetcore** and **remoting** listeners that share common services.

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
[2]:   
[3]:
[5]:  https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/wiki/Roadmap "wiki: Project roadmap"
[6]:  https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/wiki/VersionHistory "wiki: Version History"
[7]:  CONTRIBUTING.md "Contributing"
[8]:  https://www.coherentsolutions.com/ "Coherent Solutions Inc."
[9]:  https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/blob/master/LICENSE.md "License"
[11]: https://www.nuget.org/packages/CoherentSolutions.AspNetCore.ServiceFabric.Hosting "NuGet package"
[12]: https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/wiki "Project wiki"

