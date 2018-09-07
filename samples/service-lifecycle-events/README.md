# Services Lifecycle Events

This sample demonstrates how to configure **delegates** to react on service events for both **stateful service** and **stateless service**.

## What is inside?

There are two projects inside:

* **App** - this is a application project
* **Service** - this is a service project

The application is configured to have one **stateful service** and one **stateless service** both configured to use singleton partitioning and have one replica / instance. 

Both services have **delegates** configured on each service event. Each **delegate** writes an event to ETW pipeline when service event occurs.

## How to use?

Open **Diagnostics Windows** in Visual Studio or use other ETW friendly tool and register two additional ETW providers: `AppType.StatelessServiceType` and `AppType.StatefulServiceType`.

When application is deployed to cluster you should start seeing services events.

## Conclusion

For more information please check this [wiki article][1] and explore source code! 

If you have a suggestion or found an issue please consider [reporting it][2].

[1]: https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/wiki/Defining-Delegates
[2]: https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/issues