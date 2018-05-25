# ROADMAP

Our roadmap is flexible. Bug-fixing and immediate feature requests can have significant influence on it.

Here is a list of items we already done:

- [x] Publish pre-release version of **CoherentSolutions.AspNetCore.ServiceFabric.Hosting**

Here is a list of items we will implement:

* Support for Application Insights 

> **Developer's comment:**
>
> The goal is to have a flexible way to configure Application Insights on a per web host basis, including support for custom events.

* Support for Background Workers
  
> **Developer's comment:**
>
> The goal is to have a flexible way to register background workers that would be executed in both self-hosted scenarios, per web host scenarios, and per service scenarios  support for stateful and stateless service behavior.

* Support for Event Tracing, Health and Monitoring
  
> **Developer's comment:**
>
> The goal is to integrate ETW, H&M into default implementations and provide a flexible way to log and report information on per web host basis, and service basis.

* Native support for dependency injection for `ConfigureOnRun` and other extensions

> **Developer's comment:**
>
> Currently `ConfigureOnRun()` action accepts IServiceProvider and implementation is forced to retrieve dependencies manually. The goal is to allow passing `Action<T, T1, ...>` as a parameter to `ConfigureOnRun()` method and perform automated dependency injection.