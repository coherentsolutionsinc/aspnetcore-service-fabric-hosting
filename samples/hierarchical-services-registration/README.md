# Hierarchical Services Registration

This sample demonstrates how to register services for multiple downstream `builders` without violating objects lifetime constraints.

## What is inside?

There are two projects inside:

* **App** - this is a application project
* **Service** - this is a service project

The application is configured to have stateful service without partitioning with one replica. 

The service is configured to have two **aspnetcore** listeners bound to two SF endpoints - `FirstEndpoint` and `SecondEndpoint` correspondingly. 

The usage of hierarchical services registration process is demonstrated by configuring two services (_singleton_ and _transient_) on `HostBuilder` level using `ConfigureServices(...)`.

The singleton registration is done in order to be shared between listeners and honor lifetime constraints (i.e. there would be only one instance in the process).

The transient registration is done in order to be shared between listeners on configuration level (i.e. avoiding unnecessary code duplication).

## How to use?

When application is deployed on the cluster navigate to `FirstEndpoint` and `SecondsEndpoint` appending `/api/value` at the end.

The result output should look like:

First Endpoint | Second Endpoint
--- | ---
Shared: Hash: **45937921**; Personal: Hash: 15599987 | Shared: Hash: **45937921**; Personal: Hash: 3649268

> **Developer's comment:**
>
> It is important that "Shared: Hash:" values should **be equal** but "Personal: Hash:" **shouldn't**.

## Conclusion

For more information please check this [wiki article][1] and explore source code! 

If you have a suggestion or found an issue please consider [reporting it][2].

[1]: https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/wiki/Hierarchical-Services-Registration
[2]: https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/issues