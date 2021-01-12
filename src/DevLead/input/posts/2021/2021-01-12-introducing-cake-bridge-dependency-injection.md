---
title: Introducing Cake Bridge Dependency Injection
tags:
    - .NET
    - C#
    - Cake
    - DevOps
    - Release Notes
author: devlead
description: Utilize Cake abstractions and addins using Microsoft dependency injection
published: 2021-01-12
image: https://cdn.devlead.se/clipimg-vscode/2021/01/12/c2a643e9-2c63-361b-b458-a647352ebe87.png?sv=2019-12-12&st=2021-01-11T10%3A13%3A57Z&se=2031-01-12T10%3A13%3A57Z&sr=b&sp=r&sig=qInj%2F3o1X2jmpZR1KQIWUgaYF5S8c9zJz0l7zZVYmWg%3D
---

A couple of years ago I blogged [Dispelling the magic!](/posts/2017/2017-07-09-dispelling-the-magic), a post explaining the internals of the [Cake](https://cakebuild.net) build orchestration tool, with that post as a proof of concept I created [Cake.Bridge](https://www.nuget.org/packages/Cake.Bridge) assembly which provided an easy way from any .NET language get access to Cake abstractions and addins from a single instance static class.

## The Need

Cake has some real nice testable abstractions around working with the filesystem, processes, tools, etc. and for a .NET project, we had just that need. But a static instance isn't very testable, and for most of our .NET projects (console apps, APIs, Azure Functions, WPF, etc.) we now use dependency injection using [Microsoft.Extensions.DependencyInjection](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection/).

So with that in mind, I created an extension method to [IServiceCollection](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection?view=dotnet-plat-ext-5.0) that easily provided me a way to get a [IFileSystem](https://cakebuild.net/api/Cake.Core.IO/IFileSystem/) injected and utilizing the [FakeFileSystem](https://cakebuild.net/api/Cake.Testing/FakeFileSystem/) provided by [Cake.Testing](https://www.nuget.org/packages/Cake.Testing) for testing.
That grew into supporting most core Cake types including the more complex [ICakeContext](https://cakebuild.net/api/Cake.Core/ICakeContext/) and [IScriptHost](https://cakebuild.net/api/Cake.Core.Scripting/IScriptHost/).

## Introducing Cake.Bridge.DependencyInjection

If we got a need, chances are someone else has it too, so I've open-sourced the code and made it available as a NuGet [package](https://www.nuget.org/packages/Cake.Bridge.DependencyInjection/). It's early bits tailored for a specific need, so you should expect some rough edges, but sharing is caring.

## Usage

### Obtain

The assembly is published at [NuGet.org/packages/Cake.Bridge.DependencyInjection](https://www.nuget.org/packages/Cake.Bridge.DependencyInjection).

#### .NET CLI

```bash
dotnet add package Cake.Bridge.DependencyInjection
```

#### PackageReference

```xml
<PackageReference Include="Cake.Bridge.DependencyInjection" Version="0.1.0" />
```

### Register

```csharp
using Cake.Bridge.DependencyInjection;
...
serviceCollection
    .AddCakeCore();
```

### Resolve

```csharp
using Cake.Core;
...
var cakeContext = serviceProvider.GetRequiredService<ICakeContext>();
```

### Use

Once registered you can now via dependency injection access the majority [Cake.Core](https://cakebuild.net/api/Cake.Core/#InterfaceTypes) interfaces with ease, i.e:

- [ICakeContext](https://cakebuild.net/api/Cake.Core/ICakeContext/) - Gives access to Cake built-in and addin aliases, and most Cake abstractions.
- [IScriptHost](https://cakebuild.net/api/Cake.Core.Scripting/IScriptHost/) - Gives access to script/task runner.
- [ICakeLog](https://cakebuild.net/api/Cake.Core.Diagnostics/ICakeLog/) - Cake logging implementation.
- [IFileSystem](https://cakebuild.net/api/Cake.Core.IO/IFileSystem/) - Cake file system abstraction.

### Example ICakeContext

```csharp
using Cake.Bridge.DependencyInjection;
using Cake.Core;
using Cake.Core.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

var serviceCollection = new ServiceCollection()
    .AddCakeCore();

var serviceProvider = serviceCollection.BuildServiceProvider();

var cakeContext = serviceProvider.GetRequiredService<ICakeContext>();

cakeContext.Log.Write(
    Verbosity.Normal,
    LogLevel.Information,
    "Hello World!");
```

will output

```powershell
Hello World!
```

### Example IScriptHost and Cake.Common

Cake.Common contains Cake aliases normally ships together with the Cake script runner, when using Cake Bridge you'll need to add it to you project (same for any Cake addin).

#### .NET CLI

```bash
dotnet add package Cake.Common --version 1.0.0-rc0002
```

#### PackageReference

```xml
<PackageReference Include="Cake.Common" Version="1.0.0-rc0002" />
```

then add the appropriate using statements and you can achieve something very similar to a Cake "script"

```csharp
using Cake.Bridge.DependencyInjection;
using Cake.Core;
using Cake.Common.Diagnostics;
using Cake.Core.Scripting;
using Microsoft.Extensions.DependencyInjection;

var serviceCollection = new ServiceCollection()
    .AddCakeCore();

var serviceProvider = serviceCollection.BuildServiceProvider();

var scriptHost = serviceProvider.GetRequiredService<IScriptHost>();

scriptHost.Task("Hello")
    .Does(ctx => ctx.Information("Hello"));

scriptHost.Task("World")
    .IsDependentOn("Hello")
    .Does(ctx => ctx.Information("World"));

await scriptHost.RunTargetAsync("World");
```

will output

```powershell
========================================
Hello
========================================
Hello

========================================
World
========================================
World

Task                          Duration
--------------------------------------------------
Hello                         00:00:00.0226275
World                         00:00:00.0002682
--------------------------------------------------
Total:                        00:00:00.0228957
```

### Complete example project

A full example console application using [Spectre.Console](https://www.nuget.org/packages/Spectre.Console) demonstrating usage of both [ICakeContext](https://cakebuild.net/api/Cake.Core/ICakeContext/) and [IScriptHost](https://cakebuild.net/api/Cake.Core.Scripting/IScriptHost/) can be found in project's GitHub [repository](https://github.com/devlead/Cake.Bridge.DependencyInjection) at [src/Cake.Bridge.DependencyInjection.Example](https://github.com/devlead/Cake.Bridge.DependencyInjection/tree/develop/src/Cake.Bridge.DependencyInjection.Example).

## Resources

* GitHub - [github.com/devlead/Cake.Bridge.DependencyInjection](https://github.com/devlead/Cake.Bridge.DependencyInjection)
* NuGet -  [nuget.org/packages/Cake.Bridge.DependencyInjection](https://www.nuget.org/packages/Cake.Bridge.DependencyInjection/)
