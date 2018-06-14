# About this Sample

This sample demonstrates how to use `HybridHostBuilder` to configure Stateful Service with **aspnetcore** and **remoting** listeners that share same singleton `IManagementService` instance. 

This sample emphasis usage of [_hierarchical service registration_][1] feature.

At the end of the sample there is a verification code that sends two request: one to **aspnetcore** listener and one to **remoting** listener. 

You can `debug` the sample to see how it works.

[1]: https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/wiki/Hierarchical-Services-Registration