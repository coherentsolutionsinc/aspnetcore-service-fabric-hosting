# IMPLEMENTATION DETAILS

This section provides some additional information about internal workings of the **CoherentSolutions.AspNetCore.ServiceFabric.Hosting**.

Please make sure you've read [BASIC_SCENARIOS.md][1] first.

## General Information

The **CoherentSolutions.AspNetCore.ServiceFabric.Hosting** heavily uses [builder design pattern](http://www.oodesign.com/builder-pattern.html) to simplify configuration flow.

All `builders` follow these rules:

1. `builder's methods` **do** change its internal state but arguments, parameters etc. are used **only on build**
2. `build` is **stateless** i.e. multiple instances can be built from the same `builder`

## Convention

All `builders` use the following naming convention / pattern when exposing their functionality.

### Use*(...) Methods

`Use()` methods allow developers to set custom values to be used by a `builder`.

#### Rules

1. Multiple invocations don't stack - only the last invocation counts.
   ``` csharp
   new HybridHostBuilder()
     .UseWebHostBuilder(() => CreateBuilderA())
     .UseWebHostBuilder(() => CreateBuilderB()) // This is the final value
   ```
2. Values set by `Use()` methods are inherited and reused by downstream `builders`.
   ``` csharp
   new HybridHostBuilder()
     .UseWebHostBuilder(() => CreateBuilderA()) // Can be called on build
     .ConfigureStatelessServiceHost(
       serviceHostBuilder =>
       {
         /*
           Service builder supports .UseWebHostBuilder().
         */
         serviceHostBuilder
         //.UseWebHostBuilder(() => CreateBuilderA()) - Can be called on build
           .DefineAspNetCoreListener(
             listenerBuilder =>
             {
               /*
                 Listener builder supports .UseWebHostBuilder().
               */
               listenerBuilder
               //.UseWebHostBuilder(() => CreateBuilderA()) - Can be called on build
                 .ConfigureDefaultWebHost();
             });
       })
   ```

> **Developer's comment:**
> 
> Design of `Use()` methods was dictated by the intention to simplify configuration and support high level of code reuse at the same time.

### Configure*(...) Methods

`Configure()` methods allow developers to enqueue a configuration delegate into builder's pipeline.

#### Rules

1. Multiple invocations do stack - and maintain order.
   ``` csharp
   new HybridHostBuilder()
     .ConfigureWebHost(builder => /* code1 */)
     .ConfigureWebHost(builder => /* code2 */)
   
   /*
     During the build:
       Invoke 'code1'
       Invoke 'code2' 
   */
   ```
2. All `Configure()` methods on the same level receive the same instance of `builder`.

### Define*(...) Methods

`Define()` methods allow developers to define a new instance of component by providing appropriate builder configuration.

#### Rules

1. Multiple invocations count. Each `Define()` method instructs builder to create a separate builder for configurable component.
   ``` csharp
   /* Plenty of code is omitted for brevity */
   new HybridHostBuilder()
     .ConfigureStatelessServiceHost(
       serviceHostBuilder =>
       {
         serviceHostBuilder
           .DefineAspNetCoreListener( // Create new listener for 'endpoint1'
             listenerBuilder =>
             {
               listenerBuilder.UseEndpointName("endpoint1");
             })
           .DefineAspNetCoreListener( // Create new listener for 'endpoint2'
             listenerBuilder =>
             {
               listenerBuilder.UseEndpointName("endpoint2");
             });
       })
   ```

## Dependency Injection

During the `build` system automatically registers the following services in `IWebHost` dependency injection provider.

For each listener defined using `DefineAspNetCoreListener()` there are following dependencies:
* `ServiceContext` type to `context` singleton object mapping
* `IServicePartition` type to `partition` singleton object mapping
* `IServiceHostAspNetCoreListenerInformation` type to `listenerInformation` singleton object mapping

The `IServiceHostAspNetCoreListenerInformation` exposes information about listener instance created by `builder`:
``` csharp
public interface IServiceHostAspNetCoreListenerInformation
{
  string EndpointName { get; }
  
  string UrlSuffix { get; }
}
```

For each listener defined using `DefineAspNetCoreListener()` inside `ConfigureStatelessServiceHost()` there are following dependencies:
* `StatelessServiceContext` type to `context` singleton object mapping (same as `ServiceContext`)
* `IStatelessServicePartition` type to `partition` singleton object mapping (same as `IServicePartition`)

For each listener defined using `DefineAspNetCoreListener()` inside `ConfigureStatefulServiceHost()` there are following dependencies:
* `IReliableStateManager` type to `reliableManager` singleton object mapping
* `StatefulServiceContext` type to `context` singleton object mapping (same as `ServiceContext`)
* `IStatefulServicePartition` type to `partition` singleton object mapping (same as `IServicePartition`)

> **Developer's comment:**
> 
> Base `ServiceContext` and `IServicePartition` registration is useful for resolving dependencies that can be used in both Stateful and Stateless services while concrete implementations are for strongly typed components.

[1]: BASIC_SCENARIOS.md "Basic scenarios"
