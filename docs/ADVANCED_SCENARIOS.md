# ADVANCED SCENARIOS

## Table of Contents
* [Executing an action before IWebHost.Run()](#executing-an-action-before-iwebhostrun)

## Executing an action before IWebHost.Run()

Consider the following code in *Program.cs*.

``` csharp
public class Program
{
  public static void Main(
    string[] args)
  {
    var host = BuildWebHost(args);
    var serviceProvider = host.ServiceProvider;

    using (var scope = serviceProvider.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
          DatabaseSeed(services);
        }
        catch (Exception ex)
        {
          var logger = services.GetRequiredService<ILogger<Program>>();
          logger.LogError(ex, "An error occurred seeding the DB.");
        }
    }

    host.Run();
  }

  private static IWebHost BuildWebHost(
    string[] args)
  {
      return WebHost.CreateDefaultBuilder(args).UseStartup<Startup>().Build();
  }

  private static void DatabaseSeed(
    IServiceProvider services)
  {
    using (var context = services.GetService<QuizzesApplicationContext>())
    {
      /*
        Seeding code
      */
      context.SaveChanges();
    }
  }
}
```

> **Developer's comment:**
> 
> This scenario doesn't include the configuration for Stateful or Stateless service because it is related to `IWebHostBuilder` rather than any of these `builders`. 

Transforming this code to use `HostBuilder` has one problem illustrated below:

``` csharp
public class Program
{
  public static void Main(
    string[] args)
  {
    var host = new HostBuilder()
       .UseWebHostBuilder(() => BuildWebHostBuilder(args))
       .ConfigureDefaultWebHost()
       .Build();

    /*
      ERROR! HostBuilder doesn't have ServiceProvider property.
    */
    var serviceProvider = host.ServiceProvider; 

    using (var scope = serviceProvider.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
          DatabaseSeed(services);
        }
        catch (Exception ex)
        {
          var logger = services.GetRequiredService<ILogger<Program>>();
          logger.LogError(ex, "An error occurred seeding the DB.");
        }
    }

    host.Run();
  }

  private static IWebHost BuildWebHost(
    string[] args)
  {
    return BuildWebHostBuilder(args).Build();
  }

  private static IWebHostBuilder BuildWebHostBuilder(
    string[] args)
  {
    return WebHost.CreateDefaultBuilder(args).UseStartup<Startup>();
  }
}
```

The solution is to use a special extension method `ConfigureOnRun` inside `ConfigureWebHost` method and put seeding code there.

``` csharp
public class Program
{
  public static void Main(
    string[] args)
  {
      var host = new HostBuilder()
         .UseWebHostBuilder(() => BuildWebHostBuilder(args))
         .ConfigureWebHost(
              builder =>
              {
                  builder.ConfigureOnRun(
                      serviceProvider =>
                      {
                          using (var scope = serviceProvider.CreateScope())
                          {
                              var services = scope.ServiceProvider;
                              try
                              {
                                  DatabaseSeed(services);
                              }
                              catch (Exception ex)
                              {
                                  var logger = services.GetRequiredService<ILogger<Program>>();
                                  logger.LogError(ex, "An error occurred seeding the DB.");
                              }
                          }
                      });
              })
         .Build();

      host.Run();
  }

  private static IWebHost BuildWebHost(
    string[] args)
  {
    return BuildWebHostBuilder(args).Build();
  }

  private static IWebHostBuilder BuildWebHostBuilder(
    string[] args)
  {
    return WebHost.CreateDefaultBuilder(args).UseStartup<Startup>();
  }
}
```

