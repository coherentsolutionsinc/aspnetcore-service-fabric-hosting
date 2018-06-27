# Defining Multiple Services

This sample demonstrates how to configure a stateful and stateless services each with its own single **aspnetcore** listener by using `HostBuilder` and **CoherentSolutions.Extensions.Hosting.ServiceFabric**.

## What is inside?

There are two projects inside:

* **App** - this is a application project
* **Service** - this is a service project (contains implementation of both services)

The application is configured to have stateful service without partitioning with one replica and stateless service without partitioning with one instance. 

Each service is configured to have one **aspnetcore** listener bound to SF endpoint - `StatefulServiceEndpoint` and `StatelessServiceEndpoint` correspondingly. 

## How to use?

When application is deployed on the cluster navigate to `StatefulServiceEndpoint` and `StatelessServiceEndpoint` appending `/api/value` at the end.

The result output should be:

Stateful Service Endpoint | Stateless Service Endpoint
--- | ---
I am Stateful Service! | I am Stateless Service!

## Conclusion

For more information please check this [wiki article][1] and explore source code! 

If you have a suggestion or found an issue please consider [reporting it][2].

[1]: https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/wiki/Defining-Services#defining-multiple-services
[2]: https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/issues