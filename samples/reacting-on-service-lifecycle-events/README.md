# Services Lifecycle Events

This sample demonstrates how to configure actions to be triggered on service events for both `StatefulService` and `StatelessService`.

## What is inside?

**App.sfproj**

The application project is configured to have:
* One `StatefulService` with `Singletone` partitioning schema and `MinReplicaSetSize` and `TargetReplicaSetSize` set to `1`.
* One `StatelessService` with `Singletone` partitioning schema and `InstanceCount` set to `1`.

**Service.csproj**

The service project implements:
* One `StatefulService` that does nothing. 
* One `StatelessService` that does nothing. 

Files:
* `src/StatefulServiceEventSource.cs` - an `EventSource` implementation for writing StatefulService events.
* `src/StatelessServiceEventSource.cs` - an `EventSource` implementation for writing StatelessService events.
* `Program.cs` - an entry point and configuration of `StatefulService` and `StatelessService`.

## Key points

**Program.cs**

Both services register all events defined in `StatefulServiceLifecycleEvent` and `StatelessServiceLifecycleEvent` enumerations. 

``` csharp
new HostBuilder()
    .DefineStatefulService(
        serviceBuilder =>
        {
            serviceBuilder
                .DefineDelegate(
                    delegateBuilder =>
                    {
                        delegateBuilder
                            .UseEvent(StatefulServiceLifecycleEvent.OnStartup)
                            .UseDelegate(
                                (
                                    StatefulServiceContext svcCtx) =>
                                {
                                    StatefulServiceEventSource.Current.ServiceReplicaStartupEvent(svcCtx.ReplicaId);
                                });
                    })
        })
    .Build()
    .Run();
```

Event occurences are reported using two event sources: `StatefulServiceEventSource` and `StatelessServiceEventSource`.

## How to use?

**Visual Studio**

1. Press `F5` and deploy the application.
2. Open `Diagnostics Events` window.

In the diagnostics window you should see events from two providers: `App-StatefulService` and `App-StatelessService`.

Example of `ServiceReplicaStartupEvent`

``` javascript
{
  "Timestamp": "2018-10-16T17:40:45.7885068+03:00",
  "ProviderName": "App-StatefulService",
  "Id": 1,
  "Message": "The replica 131841744321216887 is starting up.",
  "ProcessId": 1534772,
  "Level": "Informational",
  "Keywords": "0x0000F00000000004",
  "EventName": "ServiceReplicaStartupEvent",
  "ActivityID": "00000019-0001-0000-346b-1700ffdcd7b5",
  "RelatedActivityID": null,
  "Payload": {
    "replicaId": 131841744321216887
  }
}
```

> **NOTE**
>
> You can [induce chaos to cluster](https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-controlled-chaos) to see more events coming from services while they expirience multiple state transitions.

## Conclusion

For more information please check this [wiki article][1] and explore the source code! 

If you have a suggestion or found an issue please consider [reporting it][2].

[1]: https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/wiki/Defining-Delegates
[2]: https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/issues