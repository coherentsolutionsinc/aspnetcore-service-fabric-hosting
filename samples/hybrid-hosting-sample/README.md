# About this Sample

This application demonstrates how to use `HybridHostBuilder` to configure AspNetCore application to run both as Web App and Service Fabric reliable service. `HybridHostBuilder` was designed to reduce code duplication, simplify configuration and provide a single place for entry point configuration. 

## Usage

Current sample solutions consists from:

* App/ - contains Service Fabric Application project
* WebService/ - contains AspNetCore application project that was modified to support [hybrid execution][1]

### Run Sample as Web App

Please follow the instructions below to run AspNetCore application self-hosted in Kestrel server.

**Command line**

1. Navigate to `WebService/` directory
2. Execute `dotnet run`
3. Open http://localhost:50564/api/me in browser.
4. In the browser windows you should receive "Hello! I am running inside 'Web Host'." message.

**Visual Studio**

1. Open `HybridHostingSample.sln` solution
2. Set `WebService` project as a startup project
3. Press 'F5'. You should have http://localhost:50564/api/me opened in browser with "Hello! I am running inside 'Web Host'." message.

### Run Sample as Service Fabric reliable service

Please follow the instructions below to run AspNetCore application in Service Fabric cluster.

> **Important**
>
> In order to run the application as Service Fabric Reliable Service you need to have configured **Service Fabric Cluster** either locally or in the cloud.

**Visual Studio**

1. Open `HybridHostingSample.sln` solution
2. Right click on the `App` project and select `Publish...`
3. Select the appropriate destination cluster and click `Publish`
4. In Service Fabric explorer navigate to `fabric:/App/WebService` and copy the URI from replica's `Web Service Endpoint` endpoint.
5. Open `{web-service-endpoint-uri}/api/me` in browser.
6. In the browser windows you should receive "Hello! I am running inside 'Service Fabric'." message.

[1]: https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/blob/master/docs/BASIC_SCENARIOS.md#modify-existing-aspnet-core-application-for-execution-inside-service-fabric-as-reliable-service