---
title: Mend Renovate Now Supports C# File-Based Apps and Cake.Sdk
tags:
    - .NET
    - CSharp
    - DevOps
    - Renovate
    - Cake
author: devlead
description: "Renovate can now update dependencies in C# single-file scripts and Cake.Sdk build scripts with #:sdk, #:package, and InstallTool."
published: 2026-02-26
image: https://cdn.devlead.se/clipimg-vscode/2026/02/26/a39f98b5-b854-48a8-a44b-2affb1b8d043.png?sp=r&st=2026-02-26T22:10:14Z&se=2036-02-27T06:25:14Z&spr=https&sv=2024-11-04&sr=b&sig=FIy9HlptIfMEwPF9kW3iJoNqz6iN%2BMMqX%2BezOGZ9DNM%3D
---

[Mend Renovate](https://github.com/renovatebot/renovate) automates dependency updates by opening pull requests when newer versions of your dependencies are available. Until recently, if you used .NET single-file scripts or [Cake.Sdk](https://cakebuild.net/docs/running-builds/runners/cake-sdk) build scripts written in C# (e.g. `cake.cs` or `build.cs`), Renovate did not look inside those files. Two merged changes fix that: the NuGet manager now understands `#:sdk` and `#:package` directives in C# files, and the Cake manager can extract and update packages from [`InstallTool`](https://cakebuild.net/docs/writing-builds/sdk/tools) and `InstallTools` calls. In this post I'll summarize what was added and how to enable it in your repo.

## What Changed

.NET 10 introduced [file-based execution](https://devblogs.microsoft.com/dotnet/announcing-dotnet-run-app/) for single-file C# apps. In that model, dependencies are declared with `#:sdk` and `#:package` at the top of the file instead of in a `.csproj`. Cake.Sdk uses the same mechanism for build scripts, and tools are installed via `InstallTool()` or `InstallTools()` rather than the old `#tool` directive. Renovate's default file patterns only include project and config files (e.g. `.csproj`, `global.json`, `dotnet-tools.json`), so it did not previously scan plain `.cs` files. That meant dependencies in `cake.cs` or other single-file C# scripts were never considered for updates.

Two pull requests extend Renovate to support these formats:

1. **[feat(manager/nuget): Support single file package directives](https://github.com/renovatebot/renovate/pull/40040)** (merged in [v43.26.0](https://github.com/renovatebot/renovate/releases/tag/43.26.0))  
   The NuGet manager can now extract and update dependencies from C# files that use `#:sdk` and `#:package`. Because the target files are regular source files, they are not part of the default `managerFilePatterns`. You must add the patterns you want (e.g. `/\\.cs$/` or something narrower) in your Renovate config.

2. **[feat(manager/cake): Support extracting nuget packages from InstallTools helper methods](https://github.com/renovatebot/renovate/pull/40070)** (merged in [v43.41.0](https://github.com/renovatebot/renovate/releases/tag/43.41.0))  
   The Cake manager gains support for single-file Cake.Sdk build scripts. It can extract NuGet package references from `InstallTool()` and `InstallTools()` method parameters (e.g. `InstallTool("dotnet:https://api.nuget.org/v3/index.json?package=DPI&version=2025.12.17.349");`). By default the Cake manager still only matches `*.cake` files, so to have Renovate process C# Cake scripts you need to extend the Cake manager's file patterns in config.

So in practice: use the **NuGet** manager for `#:sdk` and `#:package` in any C# file, and use the **Cake** manager for Cake-specific constructs (including `InstallTool(s)`) in C# files. For a typical `cake.cs` you will often enable both and let each manager handle the directives it understands.

## What Renovate Can Update

In a C# single-file script or Cake.Sdk build file, Renovate can now update:

- **`#:sdk`** – e.g. `#:sdk Cake.Sdk@6.0.0`
- **`#:package`** – e.g. `#:package Cake.Core@6.0.0`
- **[InstallTool() / InstallTools()](https://cakebuild.net/docs/writing-builds/sdk/tools)** – e.g. `InstallTool("dotnet:https://api.nuget.org/v3/index.json?package=DPI&version=2025.12.17.349");`

The `#:sdk` and `#:package` directives are handled by the [NuGet manager](https://docs.renovatebot.com/modules/manager/nuget/). The [Cake manager](https://docs.renovatebot.com/modules/manager/cake/) handles the Cake-specific tool installation methods. Both managers need to be told to look at `.cs` files (or the specific paths you use) via configuration.

## Enabling It in Your Repository

Renovate reads config from your repo (e.g. `renovate.json` or `renovate.json5`) and merges it with [default and inherited config](https://docs.renovatebot.com/config-overview/). To have Renovate consider your C# single-file scripts and Cake.Sdk build files, add the corresponding `managerFilePatterns` (or the `fileMatch`-style option your preset uses) so that the NuGet and Cake managers include those files.

Example addition to your Renovate config (e.g. in `renovate.json`):

```json
{
  "$schema": "https://docs.renovatebot.com/renovate-schema.json",
  "nuget": {
    "managerFilePatterns": ["/\\.cs$/"]
  },
  "cake": {
    "managerFilePatterns": ["/\\.cs$/"]
  }
}
```

If you only want to include specific script names (e.g. only `cake.cs` and `build.cs`), you can narrow the pattern:

```json
{
  "nuget": {
    "managerFilePatterns": ["/(^|/)(cake|build)\\.cs$/"]
  },
  "cake": {
    "managerFilePatterns": ["/(^|/)(cake|build)\\.cs$/"]
  }
}
```

After this, Renovate will detect dependencies in those C# files and open PRs when updates are available. You can combine this with your existing presets (e.g. `config:recommended`) and other options like `packageRules` or scheduling as needed.

## Example Repositories and PRs

Real-world examples of Renovate running against Cake.Sdk-style repos:

- **[devlead/Devlead.Console](https://github.com/devlead/Devlead.Console)** – [PR #239](https://github.com/devlead/Devlead.Console/pull/239) shows Renovate updating dependencies in a C# build script.
- **[azurevoodoo/renovate-cake-sdk-test](https://github.com/azurevoodoo/renovate-cake-sdk-test)** – A small test repo used to validate the feature; e.g. [PR #4](https://github.com/azurevoodoo/renovate-cake-sdk-test/pull/4), [PR #8](https://github.com/azurevoodoo/renovate-cake-sdk-test/pull/8), [PR #9](https://github.com/azurevoodoo/renovate-cake-sdk-test/pull/9).

Running Renovate locally against that test repo with a config that includes `(^|/)(cake|build)\\.cs$` for the Cake manager produces PRs for outdated packages in those files without issues.

## Summary

If you use C# single-file apps or Cake.Sdk build scripts with `#:sdk`, `#:package`, and `InstallTool(s)`, you can now keep those dependencies up to date with Mend Renovate. Add the appropriate `managerFilePatterns` for the NuGet and Cake managers so they scan your `.cs` files (or the specific paths you use), and Renovate will open pull requests for available updates. For more on the managers and config, see the [NuGet manager docs](https://docs.renovatebot.com/modules/manager/nuget/), the [Cake manager docs](https://docs.renovatebot.com/modules/manager/cake/), and the [Renovate configuration overview](https://docs.renovatebot.com/config-overview/).
