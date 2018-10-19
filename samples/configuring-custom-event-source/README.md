# Custom Event Source

This sample demonstrates how to configure a custom implementation of `EventSource` to be used by **CoherentSolutions.Extensions.Hosting.ServiceFabric** infrastructure and make the `EventSource` consumable through focused interface.

## What is inside?

**App.sfproj**

The application project is configured to have one `StatefulService` with `Singletone` partitioning schema and `MinReplicaSetSize` and `TargetReplicaSetSize` set to `1`.

**Service.csproj**

The service project implements one `StatefulService` that exposes one external endpoint `ServiceEndpoint` accessible by `/api/value` path. 

Files:
* `src/Controllers/ApiController.cs` - a controller for `ServiceEndpoint` request handling.
* `src/IApiServiceEventSource.cs` - a focused interface for event writing.
* `src/ServiceEventSource.cs` - a custom `EventSource` implementation.
* `src/Startup.cs` - a startup configuration for `IWebHost` used to power `ServiceEndpoint`.
* `Program.cs` - an entry point and configuration for `StatefulService`.

## Key points

**Program.cs**

The custom event source configuration is done using the `SetupEventSource` extension method:

``` csharp
new HostBuilder()
    .DefineStatefulService(
        serviceBuilder =>
        {
            serviceBuilder
                .SetupEventSource(
                    eventSourceBuilder =>
                    {
                        eventSourceBuilder
                            .UseImplementation(() => ServiceEventSource.Current);
                    })
        })
    .Build()
    .Run();
```

This statement registers the static instance of `ServiceEventSource` to be an implementation of `EventSource`. 

**src/ServiceEventSource.cs**

Registration of `ServiceEventSource` as custom event source is possible because it does implements `IServiceEventSource` interface provided by the **CoherentSolutions.Extensions.Hosting.ServiceFabric** infrastructure..

``` csharp
[EventSource(Name = "App-Service")]
internal sealed class ServiceEventSource : EventSource, IServiceEventSource, IApiServiceEventSource
```

**src/IApiServiceEventSource.cs**

The `IApiServiceEventSource` defines a specialized interface for event writing. This is done because `IApiServiceEventSource` implements `IServiceEventSourceInterface`. 

``` csharp
public interface IApiServiceEventSource : IServiceEventSourceInterface
{
    void GetValueMethodInvoked();
}
```

The **CoherentSolutions.Extensions.Hosting.ServiceFabric** infrastructure will automatically register `EventSource` implementation who implements interface derived from `IServiceEventSourceInterface` as instance of this interface. 

## How to use?

**Visual Studio**

1. Press `F5` and deploy the application.
2. Open `Diagnostics Events` window.
3. Invoke `GetValue` controller action by navigating to `ServiceEndpoint` and appending `/api/value` at the end.

The request output should be: `Value from ApiController`. 

In the diagnostics window you should see `GetValueMethodInvoked` events like this:

``` javascript
{
  "Timestamp": "2018-10-16T15:22:02.0626857+03:00",
  "ProviderName": "App-Service",
  "Id": 7,
  "Message": "GetValueMethodInvoked",
  "ProcessId": 883356,
  "Level": "Informational",
  "Keywords": "0x0000F00000000004",
  "EventName": "GetValueMethodInvoked",
  "ActivityID": null,
  "RelatedActivityID": null,
  "Payload": {}
}
```

These events are emitted using the focused interface. 

You also should notice a lot of `ServiceEvents` emitted with along side. 

``` javascript
{
  "Timestamp": "2018-10-16T15:29:51.9713913+03:00",
  "ProviderName": "App-Service",
  "Id": 2,
  "Message": "Request starting HTTP/1.1 GET http://localhost:33002/favicon.ico  ",
  "ProcessId": 999628,
  "Level": "Informational",
  "Keywords": "0x0000F00000000000",
  "EventName": "ServiceMessage",
  "ActivityID": null,
  "RelatedActivityID": null,
  "Payload": {
    "serviceName": "fabric:/App/Service",
    "serviceTypeName": "ServiceType",
    "replicaOrInstanceId": 131841665530617593,
    "partitionId": "\"008ef1cb-0d5b-4455-b481-6c1bc2ad73a9\"",
    "applicationName": "fabric:/App",
    "applicationTypeName": "AppType",
    "nodeName": "_Node_3",
    "message": "Request starting HTTP/1.1 GET http://localhost:33002/favicon.ico  "
  }
}
```

These events are the result of automatic event proxy done by the **CoherentSolutions.Extensions.Hosting.ServiceFabric** infrastructure (this behavior is controlled by `UseLoggerOptions` configuration):

``` csharp
new HostBuilder()
    .DefineStatefulService(
        serviceBuilder =>
        {
            serviceBuilder
                .DefineAspNetCoreListener(
                    listenerBuilder =>
                    {
                        listenerBuilder
                            .UseLoggerOptions(() => new ServiceHostLoggerOptions());
                    });
        })
    .Build()
    .Run();
```

The implementation of how these events are actually written to event log is implemented inside `ServiceEventSource` class as part of `IServiceEventSource` interface implementation:

``` csharp
[EventSource(Name = "App-Service")]
internal sealed class ServiceEventSource : EventSource, IServiceEventSource, IApiServiceEventSource
{
    private const int ServiceMessageEventId = 2;

    public void WriteEvent<T>(
        ref T eventData)
        where T : ServiceEventSourceData
    {
        this.ServiceMessage(
            eventData.ServiceName,
            eventData.ServiceTypeName,
            eventData.ReplicaOrInstanceId,
            eventData.PartitionId,
            eventData.ApplicationName,
            eventData.ApplicationTypeName,
            eventData.NodeName,
            eventData.EventMessage);
    }

    [Event(ServiceMessageEventId, Level = EventLevel.Informational, Message = "{7}")]
    private void ServiceMessage(
        string serviceName,
        string serviceTypeName,
        long replicaOrInstanceId,
        Guid partitionId,
        string applicationName,
        string applicationTypeName,
        string nodeName,
        string message)
    {
        this.WriteEvent(
            ServiceMessageEventId,
            serviceName,
            serviceTypeName,
            replicaOrInstanceId,
            partitionId,
            applicationName,
            applicationTypeName,
            nodeName,
            message);
    }
}
```

## Conclusion

For more information please check this [wiki article][1] and explore the source code! 

If you have a suggestion or found an issue please consider [reporting it][2].

[1]: https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/wiki/Understanding-Logging
[2]: https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/issues