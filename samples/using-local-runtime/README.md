# Using Local Runtime

This sample demonstrates how to use **local runtime** feature to debug `StatelessService` locally (without deploying it to Service Fabric Cluster).

## What is inside?

**App.sfproj**

The application project is configured to have one `StatelessService` with one `Singleton` partitioning schema. 

**Service.csproj**

The service project implements one `StatelessService` that exposes one external endpoint `ServiceEndpoint` accessible by `/api/print` path. 

Besides external endpoint service also has one configuration package `Config` and one data package `Data`.

Files:
* `PackageRoot/ServiceManifest.xml` - a manifest for current service package.
* `PackageRoot/Config` - a configuration package.
* `PackageRoot/Config/Settings.xml` - a settings part of `Config` configuration package.
* `PackageRoot/Data` - a data package.
* `PackageRoot/Data/Data-Sample.xml` - a sample data file inside `Data` data package.
* `src/Controllers/ApiController.cs` - a controller for `ServiceEndpoint` request handling.
* `src/Startup.cs` - a startup configuration for `IWebHost` used to power `ServiceEndpoint`.
* `Program.cs` - an entry point and configuration of `StatefulService`.

## Key points

**Properties/launchSettings.json**

Usage of **local runtime** doesn't require any code changes or 'special' method calls. The 'to use' or 'not to use' decision is made based on the existence and value of `CS_EHSF_RUNTIME` environment variable.

> **SUPPORTED VALUES**
> 
> * `Default` - instructs infrastructure to use **default runtime** (Service Fabric runtime).
> * `Local` - instructs infrastructure to use **local runtime**.

The value of `CS_EHSF_RUNTIME` variable can be defined inside `launchSettings.json` file of `Service.csproj` project: 

``` json
{
  "profiles": {
    "WebService": {
      "commandName": "Project",
      "launchBrowser": true,
      "launchUrl": "api/print",
      "environmentVariables": {
        "CS_EHSF_RUNTIME": "Local",
        "ASPNETCORE_ENVIRONMENT": "Development"
      },
      "applicationUrl": "http://localhost:50564/"
    }
  }
}
```

**Mechanics**

When using **local runtime** all runtime related information is outputted into console:

```
info : fabric:/ApplicationName/ServiceType-1[0]
      ServiceEndpoint: http://<host>:<port>/8541ea28-e5e8-4a21-989f-9fd058ba7dac/1
```

`ServiceEndpoint` URI depends on the endpoint configuration defined in `ServiceManifest.xml` (`Port` is supported) and whether `UseUniqueServiceUrlIntegration` method was used inside `DefineAspNetCoreListener`.

## How to use?

**Visual Studio**

1. Set `Service.csproj` as startup project.
2. Press `F5` and deploy the application.
3. Invoke `Print` controller action by navigating to `ServiceEndpoint` and appending `/api/print` at the end.

The request output should be: 

```
Service Context:
- ServiceName: fabric:/ApplicationName/ServiceType-1
- ServiceTypeName: ServiceType
- PartitionId: 16a4eb44-c32d-4448-8cc2-e9d07e8a5374
- ListenAddress: <Machine name>
- PublishAddress: <Machine name>
- ReplicaOrInstanceId: 1
- TraceId: {16a4eb44-c32d-4448-8cc2-e9d07e8a5374}:1

Node Context:
- NodeName: <Machine name>
- IPAddressOrFQDN: <Machine name>
- NodeId: 10000000000000000
- NodeInstanceId: 1
- NodeType: <Machine name>

Code Activation Context:
- ApplicationName: fabric:/ApplicationName
- ApplicationTypeName: ApplicationTypeName
- CodePackageName: Code
- CodePackageVersion: 1.0.0
- ContextId: b72f74cf-f755-4883-bc22-16b807747419
- LogDirectory: <Temp>\b72f74cf-f755-4883-bc22-16b807747419\Log
- TempDirectory: <Temp>\b72f74cf-f755-4883-bc22-16b807747419\Temp
- WorkDirectory: <Temp>\b72f74cf-f755-4883-bc22-16b807747419\Work

Code Packages:
- Package: Code
  * Package: <Project>\samples\using-local-runtime\Service\bin\x64\Debug\netcoreapp2.2
  * Package: ServicePkg
  * Package: 1.0.0

Data Packages:
- Package: Data
  * Package: <Project>\samples\using-local-runtime\Service\PackageRoot\Data
  * Package: ServicePkg
  * Package: 1.0.0
  * Content:
    + <Project>\samples\using-local-runtime\Service\PackageRoot\Data\Data-Sample.xml

Configuration Packages:
- Package: Config
  * Package: <Project>\samples\using-local-runtime\Service\PackageRoot\Config
  * Package: ServicePkg
  * Package: 1.0.0
  * Settings:
      + Test/TestParam = TestParamValue

Endpoints:
- ServiceEndpoint

Service Types:
- ServiceType/Stateless

Environment Variables from Environment:
- Fabric_ApplicationName = fabric:/ApplicationName
- Fabric_ServiceName = fabric:/ApplicationName/ServiceType-1
- Fabric_NodeId = 10000000000000000
- Fabric_CodePackageName = Code
- Fabric_Endpoint_ServiceEndpoint = 0
- Fabric_NodeName = <Machine name>
- Fabric_Folder_App_Work = <Temp>\b72f74cf-f755-4883-bc22-16b807747419\Work
- Fabric_IsContainerHost = False
- Fabric_NodeIPOrFQDN = <Machine name>
- Fabric_Folder_App_Temp = <Temp>\b72f74cf-f755-4883-bc22-16b807747419\Temp
- Fabric_Folder_App_Log = <Temp>\b72f74cf-f755-4883-bc22-16b807747419\Log
- Fabric_ServicePackageActivationId = b72f74cf-f755-4883-bc22-16b807747419

Environment Variables from IConfiguration:
- Fabric_ServicePackageActivationId = b72f74cf-f755-4883-bc22-16b807747419
- Fabric_ServiceName = fabric:/ApplicationName/ServiceType-1
- Fabric_NodeName = <Machine name>
- Fabric_NodeIPOrFQDN = <Machine name>
- Fabric_NodeId = 10000000000000000
- Fabric_IsContainerHost = False
- Fabric_Folder_App_Work = <Temp>\b72f74cf-f755-4883-bc22-16b807747419\Work
- Fabric_Folder_App_Temp = <Temp>\b72f74cf-f755-4883-bc22-16b807747419\Temp
- Fabric_Folder_App_Log = <Temp>\b72f74cf-f755-4883-bc22-16b807747419\Log
- Fabric_Endpoint_ServiceEndpoint = 0
- Fabric_CodePackageName = Code
- Fabric_ApplicationName = fabric:/ApplicationName
```

## Conclusion

For more information please check this [wiki article][1] and explore the source code! 

If you have a suggestion or found an issue please consider [reporting it][2].

[1]: https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/wiki/Using-Local-Runtime
[2]: https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/issues