# Defining Services

This sample demonstrates how to configure a simple stateful service with one **aspnetcore** listener by using `HostBuilder` and **CoherentSolutions.Extensions.Hosting.ServiceFabric**.

## What is inside?

There are two projects inside:

* **App** - this is a application project
* **Service** - this is a service project

The application is configured to have stateful service without partitioning with one replica. 

The service is configured to have one **aspnetcore** listener bound to SF endpoint - `ServiceEndpoint`. 

## How to use?

1. Deploy the application
2. Navigate to SFE (by default is: http://localhost:19080)
3. Navigate down to primary replica: `Cluster -> Applications -> AppType -> fabric:/App -> fabric:/App/Service -> (GUID) -> (INTEGER)`
4. Navigate to URL's of `ServiceEndpoint` appending `/api/value` at the end

The result output should be - **Value from ApiController**

## Conclusion

For more information please check this [wiki article][1] and explore source code! 

If you have a suggestion or found an issue please consider [reporting it][2].

[1]: https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/wiki/Defining-Services#defining-single-service
[2]: https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/issues