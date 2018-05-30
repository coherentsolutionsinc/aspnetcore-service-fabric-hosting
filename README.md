# CoherentSolutions.AspNetCore.ServiceFabric.Hosting

**CoherentSolutions.AspNetCore.ServiceFabric.Hosting** is a tiny library that is intended to simplify implementation of ASP.NET Core apps that need to have an ability to run both inside and outside of Service Fabric.

It may be of use when you need to:
1. Run ASP.NET Core application as *self-hosted / containerized / etc.* in *dev / test* environments and run the same application in Service Fabric as Reliable Service (Stateless or Stateful)
2. Reduce initialization code duplication and have clear separation between self-hosted and Service Fabric - specific initialization code

## Eager to start? 

See [Modify existing ASP.NET Core application for execution inside Service Fabric as Reliable Service][10] for a quick start!

## Documentation

For questions please see [FAQ.md][1].

For basic scenarios please refer to [BASIC_SCENARIOS.md][2].

For advanced scenarios please refer to [ADVANCED_SCENARIOS.md][3].

For implementation details please see [IMPLEMENTATION_DETAILS.ms][4].

## Future & Past

For our plans on future releases please see [ROADMAP.md][5].

For information on past releases please refer to [RELEASE_NOTES.md][6].

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
[5]:  ROADMAP.md "Project roadmap"
[6]:  RELEASE_NOTES.md "Release notes"
[7]:  CONTRIBUTING.md "Contributing"
[8]:  https://www.coherentsolutions.com/ "Coherent Solutions Inc."
[9]:  https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/blob/master/LICENSE.md "License"
[10]: docs/BASIC_SCENARIOS.md#modify-existing-asp.net-core-application-for-execution-inside-service-fabric-as-reliable-service "Modify existing ASP.NET Core application for execution inside Service Fabric as Reliable Service"

