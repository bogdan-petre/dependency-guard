[![Build Status](https://dev.azure.com/FabricaDeDezvoltare/DependencyGuard/_apis/build/status/devfact.dependency-guard?branchName=master)](https://dev.azure.com/FabricaDeDezvoltare/Failure%20Tracker/_build/latest?definitionId=1&branchName=master)

# What is it?
Dependency Guard is a library to help you validate your ASP .NET Core 2.* DI container at startup.
Before ASP.NET Core 3.* there is no build validation for ServiceCollection in Startup.cs, so I wrote one for .NET Core 2.*

# How it works?
It's similar to ServiceProvider's [ValidateOnBuild](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.serviceprovideroptions.validateonbuild?view=dotnet-plat-ext-5.0).
It's an extension method for IServiceCollection that you have to call in Startup.cs's ConfigureServices method:
``` C#
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
    services.AddScoped<MyDependency, MyDependency>();

    // will validate your ServiceCollection
    services.ValidateAtStartup();
}
```

If any of you registered services has a dependency that hasn't been registered, the validation method will throw an [ServiceNotRegisteredException](https://github.com/devfact/dependency-guard/blob/master/DependencyGuard/Exceptions/ServiceNotRegisteredException.cs) at application startup.

<b><i>Note: DependencyGuard will only analize dependencies injected by constructors, not properties or any other methods.</i></b>

If for some reason you want DependencyGuard to ignore some services, mark the class with [IgnoreGuard](https://github.com/devfact/dependency-guard/blob/master/DependencyGuard/IgnoreGuardAttribute.cs) attribute:

```C#
[IgnoreGuard]
public class MyDependency
{
    private Test test;

    public MyDependency(Test test)
    {
        this.test = test;
    }
}
```
