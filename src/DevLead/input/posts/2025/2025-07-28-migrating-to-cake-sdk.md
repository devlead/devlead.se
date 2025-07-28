---
title: Migrating to Cake.Sdk
tags:
    - .NET
    - CSharp
    - DevOps
author: devlead
description: Taking your build.cake to cake.cs
published: 2025-07-28
image: https://cdn.devlead.se/clipimg-vscode/2025/07/28/aba66765-b6fb-4c2a-1553-71ec0c766405.png?sv=2025-01-05&st=2025-07-27T20%3A09%3A59Z&se=2035-07-28T20%3A09%3A59Z&sr=b&sp=r&sig=FsI7ji92JZB3VCtCDhCl5ujmhxlCLoi0xsw1ukh0nFw%3D
---

The Cake team recently announced [Cake.Sdk](https://cakebuild.net/blog/2025/07/dotnet-cake-cs), a new way to get the Cake tool scripting experience in regular .NET console applications. This brings the stellar experience of the new "dotnet run app.cs" feature (requires .NET 10), while also working seamlessly with .NET 8 and 9 for regular csproj projects.

In this post, I'll walk you through migrating from a traditional Cake .NET Tool build script to the new Cake.Sdk single file approach.

## What's Changing

The migration involves converting from a `build.cake` file with `#addin` and `#tool` directives to a `cake.cs` file with `#:sdk` and `#:package` directives. The `#tool` directive is replaced with the `InstallTool()` method call. The new approach leverages .NET 10's file-based execution while maintaining all the familiar Cake functionality.

## Migration Steps

### 1. Update global.json

First, add the Cake.Sdk to your `global.json` to pin the version:

```json
{
  "sdk": {
    "version": "10.0.100-preview.6.25358.103",
    "allowPrerelease": false,
    "rollForward": "latestMajor"
  },
  "msbuild-sdks": {
    "Cake.Sdk": "5.0.25198.49-beta"
  }
}
```

### 2. Rename and Update Build Script

Rename your `build.cake` to `cake.cs` and update the directives:

**Example before (build.cake):**
```csharp
#addin nuget:?package=Cake.FileHelpers&version=7.0.0
#addin nuget:?package=Newtonsoft.Json&version=13.0.3
#tool dotnet:?package=GitVersion.Tool&version=5.12.0

// ... existing code ...
```

**Example after (cake.cs):**
```csharp
#:sdk Cake.Sdk
#:package Cake.FileHelpers
#:package Newtonsoft.Json
InstallTool("dotnet:https://api.nuget.org/v3/index.json?package=GitVersion.Tool&version=5.12.0");

// ... existing code ...
```

### 3. Update Package Management

For Central Package Management (CPM), add the packages to your `Directory.Packages.props`:

```xml
<ItemGroup>
  <PackageVersion Include="Cake.FileHelpers" Version="7.0.0" />
  <PackageVersion Include="Newtonsoft.Json" Version="13.0.3" />
  <!-- ... existing packages ... -->
</ItemGroup>
```

Alternatively, you can specify versions directly in the package directives:

```csharp
#:package Cake.FileHelpers@7.0.0
#:package Newtonsoft.Json@13.0.3
```

### 4. Update Build Command

The build command changes from:

```bash
dotnet cake build.cake
```

To:

```bash
dotnet cake.cs
```

## Key Differences

### Package References

**Old way:**
```csharp
#addin nuget:?package=Cake.FileHelpers&version=7.0.0
#addin nuget:?package=Newtonsoft.Json&version=13.0.3
```

**New way:**
```csharp
#:package Cake.FileHelpers
#:package Newtonsoft.Json
```

### Tool Installation

**Old way:**
```csharp
#tool dotnet:?package=GitVersion.Tool&version=5.12.0
```

**New way:**
```csharp
InstallTool("dotnet:https://api.nuget.org/v3/index.json?package=GitVersion.Tool&version=5.12.0");
```

## Requirements

- **File-based approach**: .NET 10 Preview 6 or later
- **Project-based approach**: .NET 8.0 or later

## Benefits

1. **Simplified Setup**: No need for wrapper scripts or tool installation
2. **Better IDE Support**: Full IntelliSense and debugging capabilities
3. **Centralized Package Management**: Works seamlessly with CPM
4. **Standard NuGet Auth Support**: Uses your existing NuGet configuration and credentials
5. **.NET SDK Tooling**: Leverages standard .NET tooling and build processes
6. **Directory.Build.props/targets Support**: Integrates with MSBuild's directory-level customization for build settings

## Converting to Project-Based

If you prefer a traditional project-based approach, you can convert your `cake.cs` file to a full .NET project using:

```bash
dotnet project convert cake.cs
```

This command creates a new directory named for your file, scaffolds a `.csproj` file, moves your code into the new directory as `cake.cs`, and translates any `#:` directives into MSBuild properties and references.

**Before (cake.cs):**
```csharp
#:sdk Cake.Sdk
#:package Cake.FileHelpers
#:package Newtonsoft.Json
#:property ProjectType=Test

// ... existing code ...
```

**After (cake/cake.csproj):**
```xml
<Project Sdk="Cake.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <PublishAot>true</PublishAot>
  </PropertyGroup>
  <PropertyGroup>
    <ProjectType>Test</ProjectType>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Cake.FileHelpers" />
    <PackageReference Include="Newtonsoft.Json" />
  </ItemGroup>
</Project>
```

The project-based approach works with .NET 8, 9, and 10, while the file-based approach requires .NET 10.

## Real-World Example

The [Polly](https://github.com/App-vNext/Polly) project recently migrated from Cake .NET Tool to Cake.Sdk in their [dotnet-vnext branch](https://github.com/App-vNext/Polly/pull/2676). The migration involved:

- Renaming `build.cake` to `cake.cs` and adding SDK directives
- Updating `build.ps1` to use `dotnet cake.cs` instead of `dotnet cake`
- Adding `Cake.Sdk` to `global.json` `msbuild-sdks` section
- Moving `Cake.FileHelpers` and `Newtonsoft.Json` from `#addin` to `#package` directives
- Adding `ProjectType=Test` property for analyzer support

This real-world example demonstrates how straightforward the migration process is, even for large, complex projects like Polly.

## Conclusion

The migration to Cake.Sdk is straightforward and brings significant improvements to the development experience. The new approach maintains all existing functionality while providing better tooling support, simplified project structure, and enhanced IDE integration.

For more information, check out the [official announcement](https://cakebuild.net/blog/2025/07/dotnet-cake-cs) and the [example repository](https://github.com/cake-build/cakesdk-example).
