---
title: Preparing for .NET 9
tags:
    - .NET
author: devlead
description: Initial reflections after running .NET 9 in production
published: 2024-09-22
image: https://cdn.devlead.se/clipimg-vscode/2024/09/22/df979602c75a4074bff02d5d02fdb88e.jpg?sv=2023-01-03&st=2024-09-22T12%3A57%3A23Z&se=2035-07-29T12%3A57%3A00Z&sr=b&sp=r&sig=V0HqKU5KQUe8JuTyKzmuxyNNyyX735PAm4M6Lx%2FaXoY%3D
---

.NET 9 is just around the corner with the General Availability (GA) release scheduled for November 2024. The .NET 9 RC 1 (released September 10, 2024) already comes with a Go-Live license, meaning it’s supported by Microsoft for use in production environments.

If you’re currently running .NET 6 or newer, I’ve found the migration process to .NET 9 to be fairly straightforward. Here are some key steps to guide you through the transition.

### 1. Update Dependencies

Update outdated dependencies. This is always a good practice before migrating, many incompatibility issues are often sorted this way. An excellent tool aiding with this is the [dotnet-outdated](https://github.com/dotnet-outdated/dotnet-outdated) tool, run `dotnet outdated <solution/project folder>` to check for any out of date packages and it can even update the packages for you.

### 2. Remove Obsolete Target Frameworks

If you are still targeting obsolete frameworks like `net7.0`, it's time to clean them up. Many newer .NET 9 assemblies only target .NET 8 and above. If you need to maintain compatibility with .NET 6, you can use conditionals on your package references as a workaround during the interim, example:

```xml
    <PackageReference Condition=" '$(TargetFramework)' == 'net6.0' " Include="Verify.Http" Version="5.0.1" />
    <PackageReference Condition=" '$(TargetFramework)' != 'net6.0' " Include="Verify.Http" Version="6.3.0" />
```

### 3. Update `global.json`

An easy way to update your `global.json` is to simply delete it and run `dotnet new globaljson`. This command will create a `global.json` matching the preview SDK, which will work smoothly in your CI/CD pipeline.

### 4. Add `net9.0` Target Framework Moniker (TFM)

Add the `net9.0` target framework to your project, using the `TargetFrameworks` MSBuild property (if you're using `TargetFramework` today and want to multitarget it will need to be relaced with `TargetFrameworks`), example:

```xml
<TargetFrameworks>net9.0;net8.0</TargetFrameworks>
```

After November 12, 2024, the supported frameworks will be:
- .NET 8 (Long Term Support until November 10, 2026)
- .NET 9 (Standard Term Support, 18 months from release)


### 5. Build and Address Issues

Once you’ve set everything up, build your solution and resolve any issues that arise:
1. **Source Issues**: The only code issue I encountered was an `Index` extension method for `IEnumerable<T>`, which is now built-in with .NET 9. In my case, the method was internal and semantically different, so I simply renamed it.
   
2. **Dependency Errors/Warnings**: The .NET 9 SDK is more vocal about packages with security issues or deprecated dependencies. Even transient dependencies (ones you don’t directly reference) might trigger warnings. `dotnet nuget why` is helpful here. You can either pin specific versions or migrate to replacement packages (especially common with Azure SDKs).

### My Experience Running .NET 9 in Production

I’ve had code running on .NET 9 in production since the day after RC1. It’s not about being an early adopter for the sake of it, but about preparing for the future. By picking small, isolated workloads, I’m learning what challenges might come up during a larger migration. There are many things we can prepare for now so that the full migration is as smooth as possible. 

In fact, you can start using the new .NET 9 SDK today while still targeting older frameworks, and gaining access to new build-time features while staying on your current runtime.

Overall, my experience running .NET 9 in production has been smooth sailing, with only minor issues. The one hiccup I encountered involved the .NET 9 base Docker images, which use a slightly newer Linux distribution, requiring some native binaries to be installed. But that’s exactly the kind of valuable lesson you want to learn early before migrating all your workloads. One common example is [System.Text.Json](https://www.nuget.org/packages/System.Text.Json) which many packages reference old and outdated versions of, by explicitly adding a package reference to it, you're pinning the version used by your project, example:
```xml
<PackageReference Include="System.Text.Json" Version="8.0.4" />
```

### Adding `net9.0` to My Open-Source Tools

I’ve already started adding the `net9.0` target framework to my OSS tools. Here are a few that have been shipped and published so far:

- [**ARI** - Azure Resource Inventory](https://www.nuget.org/packages/ARI) .NET Tool: Inventories and documents Azure Tenant resources.
- [**Blobify** - Blobify](https://www.nuget.org/packages/Blobify) .NET Tool: Archives local files to Azure Blob Storage.
- [**BRI** - Bicep Registry Inventory](https://github.com/yourlink) .NET Tool: Inventories and documents Bicep modules in an Azure container registry.
- [**DPI** - Dependency Inventory](https://www.nuget.org/packages/DPI) .NET Tool: Inventories dependencies to Azure Log Analytics.
- [**UnpackDacPac** - Unpack DAC Package](https://www.nuget.org/packages/UnpackDacPac) .NET Tool: Extracts a DACPAC and generates a deployment script to a target folder, without needing a live database.
