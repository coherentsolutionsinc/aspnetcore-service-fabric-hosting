# About the project

**CoherentSolutions.AspNetCore.ServiceFabric.Hosting** is a simple project to basically simplifies two things:

1. Configuration of stateful or stateless reliable service (based on the [IWebHost][1])
2. Configuration of **aspnetcore** application that also should be stateful or stateless reliable service by providing support for shared configuration hybrid execution that allow application to run as self-hosted **aspnetcore** application or as reliable service without future code modifications.
    
    
    _This can be useful for those who performs a migration to Service Fabric and would like to retain the ability for self-hosted execution (in development, continues integration etc.)_

More information can be found in [wiki][12]

## Eager to start? 

Install [NuGet][11] package and start configure your Service Fabric services or see [detailed guide][10] to understand how to modify an existing ASP.NET Core application for executing inside Service Fabric!

## Documentation

For questions please see [FAQ.md][1].

For basic scenarios please refer to [BASIC_SCENARIOS.md][2].

For advanced scenarios please refer to [ADVANCED_SCENARIOS.md][3].

For implementation details please see [IMPLEMENTATION_DETAILS.ms][4].

## Future & Past

For our plans on future releases please refer to wiki [Project Roadmap][5] page.

For information on past releases please refer to wiki [Version History][6] page.

## Contributing

For additional information on how to contribute to this project, please see [CONTRIBUTING.md][7].

## Authors

This project is owned and maintained by [Coherent Solutions][8].

## License

This project is licensed under the MS-PL License - see the [LICENSE.md][9] for details.

[1]:  docs/FAQ.md "Frequently Asked Questions"
[2]:  docs/BASIC_SCENARIOS.md "Basic scenarios"
[3]:  docs/ADVANCED_SCENARIOS.md "Advanced scenarios"
[4]:  docs/IMPLEMENTATION_DETAILS.md "Implementation details"
[5]:  https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/wiki/Roadmap "Project roadmap"
[6]:  https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/wiki/VersionHistory "Version History"
[7]:  CONTRIBUTING.md "Contributing"
[8]:  https://www.coherentsolutions.com/ "Coherent Solutions Inc."
[9]:  https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/blob/master/LICENSE.md "License"
[10]: docs/BASIC_SCENARIOS.md#modify-existing-aspnet-core-application-for-execution-inside-service-fabric-as-reliable-service "Modify existing ASP.NET Core application for execution inside Service Fabric as Reliable Service"
[11]: https://www.nuget.org/packages/CoherentSolutions.AspNetCore.ServiceFabric.Hosting/0.5.1-alpha "NuGet package"
[12]: https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/wiki "Project wiki"

