---
title: My preferred .NET console stack
tags:
    - .NET
    - C#
    - OSS
    - Opinion
author: devlead
description: An opinionated view on the boilerplate starting point of any .NET console application
published: 2021-01-15
image: https://cdn.devlead.se/clipimg-vscode/2021/01/14/e02ce155-5ff2-8a7b-e306-8c486f23f864.png?sv=2019-12-12&st=2021-01-13T20%3A33%3A10Z&se=2031-01-14T20%3A33%3A10Z&sr=b&sp=r&sig=fdANWdytZGRdgDMxN%2B%2BTNSCAbz%2Bzg6R0iAvtvYWYoKw%3D
---

There's type of application that has followed me since I learned to code in the mid-'80s, and that's the console application. For years they looked the same a `Main(string[] args)` and some naive inconsistent command line parser. That gradually improved with the adoption of various OSS helper libraries. In this post, I'll walk through what today is my alternative starting point to `dotnet new console`, a way that greatly reduces the boilerplate code needed for logging, parsing, and validation of arguments, letting me focus on the problem to solve and not the plumbing.

## Templates

A convenient way to scaffold a new project is using the template function of .NET SDK CLI, it comes preloaded with several templates like `console`, `classlib`, etc., but beyond that, it's possible to create your own templates, which I've for my and your convenience created, so given [.NET 5 SDK](https://dotnet.microsoft.com/download/dotnet/5.0) installed, easily yourself can try and take a look at everything discussed in this post.

## Devlead Console Template

So let's get started with creating a new console application according to my opinionated recipe, .NET SDK Templates are distributed as NuGet packages and the canonical source for NuGet packages is [NuGet.org](https://www.nuget.org), where I've published my template as [Devlead.Console.Template](https://www.nuget.org/packages/Devlead.Console.Template/). Templates are installed using the `dotnet new` command with `--install packageId` parameter, in this case:

```bash
dotnet new --install Devlead.Console.Template
```

## dotnet new devleadconsole

With the template installed locally, we now have a new `devleadconsole` template at our disposal, to create our new console applications with according to me, essential dependencies and boilerplate code:

```bash
dotnet new devleadconsole -n MyConsoleApp
```

The above command will in the current directory result in the below folder structure

```text
MyConsoleApp
    │   MyConsoleApp.csproj
    │   Program.cs
    │
    └───Commands
        │   ConsoleCommand.cs
        │
        ├───Settings
        │       ConsoleSettings.cs
        │
        └───Validation
                ValidateStringAttribute.cs
```

## MyConsoleApp.csproj

The created project file will look something like below

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net5.0</TargetFramework>
    <Nullable>enable</Nullable>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Spectre.Console" Version="0.37.0" />
    <PackageReference Include="Spectre.Cli.Extensions.DependencyInjection" Version="0.3.0" />
    <PackageReference Include="Microsoft.Extensions.Logging" Version="5.0.0" />
    <PackageReference Include="Microsoft.Extensions.Logging.Console" Version="5.0.0" />
    <PackageReference Include="Microsoft.SourceLink.GitHub" Version="1.0.0" PrivateAssets="All"/>
  </ItemGroup>
</Project>
```

let's step for step break it down

### OutputType

`OutputType` with the value `exe`, indicates that this will be an executable.

### TargetFramework

`TargetFramework` with the value `net5.0`, means that this will be compiled for/targeting .NET 5.

### Nullable

`Nullable` with the value `enable`, enables the [nullable reference types](https://docs.microsoft.com/en-us/dotnet/csharp/nullable-references) feature that was introduced with C# 8, making reference types non-nullable by default, basically moving many errors from being caught late at runtime, to be caught early at compile time.

### TreatWarningsAsErrors

`TreatWarningsAsErrors` with the value `true` makes the compiler grumpier, it won't just break the build for compiler errors, but also for compiler warnings, combined with `Nullable` I personally believe code quality gets better from the start.

### PackageReference(s)

- **[Spectre.Console](https://www.nuget.org/packages/Spectre.Console)** is a lot of things, a true swiss army for anyone doing console applications, but in this template, it's foremost an extremely opinionated command-line parser.
- **[Spectre.Cli.Extensions.DependencyInjection](https://www.nuget.org/packages/Spectre.Cli.Extensions.DependencyInjection)** makes it easy for [Spectre.Console](https://www.nuget.org/packages/Spectre.Console) to interop with standard **[Microsoft DependencyInjection](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection/)**, same as used by default for dependency injection with i.e. ASP .NET and Azure Functions.
- **[Microsoft.Extensions.Logging](https://www.nuget.org/packages/Microsoft.Extensions.Logging)** simplified provides standard logging abstractions and **[Microsoft.Extensions.Logging.Console](https://www.nuget.org/packages/Microsoft.Extensions.Logging.Console)** provides an implementation for logging to the console.
- **[Microsoft.SourceLink.GitHub](https://www.nuget.org/packages/Microsoft.SourceLink.GitHub)** enables automatic tracking between artifact and source control, providing a better debugging and traceability experience.

## Program.cs

The generated `Program.cs` uses the new C#9 [Top-level statement](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-9#top-level-statements) pattern removing unnecessary ceremony code from the application, but what it does contain:

1. using statements
1. Creating dependency injection container
1. Console logger registration
1. Hooking up dependency injection container with Spectre.Console
1. Spectre.Console command declaration
1. Execute the application

> Note: [Top-level statement](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-9#top-level-statements)  means as `RunAsync` returns a `Task<int>`, .NET 5 will automatically generate "`Program`" class and `async Task<int> Main(string args)` for you, removing the need to write a lot of boilerplate code.

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Devlead.Console.Commands;
using Spectre.Console.Cli;
using Spectre.Cli.Extensions.DependencyInjection;

var serviceCollection = new ServiceCollection()
    .AddLogging(configure =>
            configure
                .AddSimpleConsole(opts => {
                    opts.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
                })
    );

using var registrar = new DependencyInjectionRegistrar(serviceCollection);
var app = new CommandApp(registrar);

app.Configure(
    config =>
    {
        config.ValidateExamples();

        config.AddCommand<ConsoleCommand>("console")
                .WithDescription("Example console command.")
                .WithExample(new[] { "console" });
    });

return await app.RunAsync(args);
```

## ConsoleCommand.cs

`ConsoleCommand.cs` contains "just" your business code, [Spectre.Console](https://spectresystems.github.io/spectre.console/) handles the heavy lifting of parsing and validating command-line arguments (*based on provided settings class, more on that later in the post.*), resolving constructor parameters using dependency injection, etc. Letting you focus on the domain and not the boilerplate code, resulting in a very similar experience to i.e. Azure Function or .NET Workers, enabling reuse of both patterns and code. [Spectre.Console](https://spectresystems.github.io/spectre.console/) has support for both async and sync commands.

```csharp
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyConsoleApp.Commands.Setting;
using Spectre.Console.Cli;

namespace MyConsoleApp.Commands
{
    public class ConsoleCommand : AsyncCommand<ConsoleSettings>
    {
        private ILogger Logger { get; }

        public override async Task<int> ExecuteAsync(CommandContext context, ConsoleSettings settings)
        {
            Logger.LogInformation("Mandatory: {Mandatory}", settings.Mandatory);
            Logger.LogInformation("Optional: {Optional}", settings.Optional);
            Logger.LogInformation("CommandOptionFlag: {CommandOptionFlag}", settings.CommandOptionFlag);
            Logger.LogInformation("CommandOptionValue: {CommandOptionValue}", settings.CommandOptionValue);
            return await Task.FromResult(0);
        }

        public ConsoleCommand(ILogger<ConsoleCommand> logger)
        {
            Logger = logger;
        }
    }
}
```

## ConsoleSettings.cs

`ConsoleSettings.cs` contains the definition of what parameters each command has, if they're are mandatory/optional, positional and how they validated. It also contains metadata used for automatically generating help and error messages.

```csharp
using System.ComponentModel;
using Devlead.Console.Commands.Validate;
using Spectre.Console.Cli;

namespace Devlead.Console.Commands.Setting
{
    public class ConsoleSettings : CommandSettings
    {
        [CommandArgument(0, "<mandatory>")]
        [Description("Mandatory argument")]
        public string Mandatory { get; set; } = string.Empty;

        [CommandArgument(1, "[optional]")]
        [Description("Optional argument")]
        public string? Optional { get; set; }

        [CommandOption("--command-option-flag")]
        [Description("Command option flag.")]
        public bool CommandOptionFlag { get; set; }

        [CommandOption("--command-option-value <value>")]
        [Description("Command option value.")]
        [ValidateString]
        public string? CommandOptionValue { get; set; }
    }
}
```

## ValidateStringAttribute.cs

[Spectre.Console](https://spectresystems.github.io/spectre.console/) can validate either by custom attributes on properties (*see `ConsoleSettings.CommandOptionValue` for an example of that*) or globally by overriding `Validate()` method on `CommandSettings`. The template ships with a sample `ValidateStringAttribute` that just validates the length of a string, but you can make it as advanced as you want.

```csharp
using Spectre.Console;
using Spectre.Console.Cli;

namespace MyConsoleApp.Commands.Validation
{
    public class ValidateStringAttribute : ParameterValidationAttribute
    {
        public const int MinimumLength = 3;

        public ValidateStringAttribute() : base(errorMessage: null)
        {
        }

        public override ValidationResult Validate(ICommandParameterInfo parameterInfo, object? value)
            => (value as string) switch {
                { Length: >= MinimumLength }
                    => ValidationResult.Success(),

                { Length: < MinimumLength }
                    => ValidationResult.Error($"{parameterInfo?.PropertyName} ({value}) needs to be at least {MinimumLength} characters long."),

                _ => ValidationResult.Error($"Invalid {parameterInfo?.PropertyName} ({value}) specified.")
            };
    }
}
```

## Result

![GIF animation of Console experience](https://cdn.devlead.se/clipimg-vscode/2021/01/15/devleadmyconsoleapp.gif?sp=rl&st=2021-01-15T09:33:29Z&se=2031-01-16T09:33:00Z&sv=2019-12-12&sr=b&sig=fp8lXgfDwOgGkdK3cYm0fFojddT8ZEx7SJuiIMkIOW8%3D)

## Conclusion

This is my opinionated happy path for doing .NET Console applications, feel free to let me know if you've got your own recipe for success, but must say I'm really happy how this combination lets me write console applications in the same way as I do my .NET workers, Azure Functions, ASP .NET Core, etc. ensuring consistency, less duplication and good reuse of both patterns and code. There's a LOT more to [Spectre.Console](https://spectresystems.github.io/spectre.console/) than command-line parsing, to I hight recommend you check out all the other features it has to offer.

## References

- [Devlead.Console.Template](https://github.com/devlead/Devlead.Console.Template) ([NuGet](https://www.nuget.org/packages/Devlead.Console.Template))
- [Spectre.Console](https://spectresystems.github.io/spectre.console/) ([NuGet](https://www.nuget.org/packages/Spectre.Console))
- [Spectre.Cli.Extensions.DependencyInjection](https://github.com/agc93/spectre.cli.extensions.dependencyinjection#spectrecliextensionsdependencyinjection) ([NuGet](https://www.nuget.org/packages/Spectre.Cli.Extensions.DependencyInjection))
- [Dependency injection in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-5.0) ([NuGet](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection/))
- [Source Link](https://github.com/dotnet/sourcelink#source-link) ([NuGet](https://www.nuget.org/packages/Microsoft.SourceLink.GitHub))
- [Logging in .NET Core and ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-5.0) ([NuGet](https://www.nuget.org/packages/Microsoft.Extensions.Logging.Console))
