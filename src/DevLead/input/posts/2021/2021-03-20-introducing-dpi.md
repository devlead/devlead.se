---
title: Introducing DPI
tags:
    - .NET
    - NuGet
    - Azure
    - Azure Log Analytics
    - DevOps
    - Tool
author: devlead
description: A DevOps tool to inspect dependencies and report to Azure Log Analytics
published: 2021-03-20
image: https://cdn.devlead.se/clipimg-vscode/2021/03/16/5e783ec1-b7cf-aaac-bf41-582e4110dfa9.png?sv=2019-12-12&st=2021-03-15T19%3A51%3A42Z&se=2031-03-16T19%3A51%3A42Z&sr=b&sp=r&sig=0jvdk30dmb7ba7dKnLlfNMvY32nKHI1kqCxXzz3VhXo%3D
---

When brought in as DevOps consultant or a new employee for that matter, one is often tasked with getting a clear picture of the current state of projects, how they work and fit together.<br /><br />
A big part of this initial and ongoing is to analyze and audit dependencies, which is why I've created `dpi` a tool that both locally and as part of your DevOps pipeline can analyze your repository projects dependencies and report to Azure Log Analytics, console, and files.

## The tool's initial scope

In this initial version, focus has been fairly complete support for C# .NET NuGet package dependencies, and the tool currently supports analyzing and reporting NuGet package dependencies for:

- [x] C# project files
- [x] MS Build Project assets
- [x] NuGet Package config files
- Cake script
  - [x] Addins
  - [x] Tools
  - [x] Modules
  - [x] Recipes

## Tool value proposition?

<img src="https://cdn.devlead.se/clipimg-vscode/2021/03/17/0f47c134-5029-94f4-7a4e-07d4cd5b5d77.png?sv=2019-12-12&st=2021-03-16T20%3A08%3A30Z&se=2031-03-17T20%3A08%3A30Z&sr=b&sp=r&sig=ZXq2a0u1x0ows1MPnkzgCWtLMz8fjvJLXvGtyVeu0dE%3D" alt="KQL Collage" align="right" width="600" height="426" />

Analyzing and reporting dependencies to a central place enables you to discover, correlate, and aggregate dependencies across multiple locations. `dpi` reports its findings to an Azure Log Analytics workspace, which provides an efficient way to store and query large amounts of tabular data using [KQL - Kusto Query Language](https://docs.microsoft.com/en-us/azure/data-explorer/kusto/query/?WT.mc_id=AZ-MVP-5002677).

This enables you to quickly tackle several scenarios i.e

- Find which repositories have certain dependencies
- Identify which versions of dependencies used
- Most / least used dependencies
- If a dependency is used anywhere
- Follow dependency usage over time
- Only limited outbound/one-way http traffic with analytics data to one common endpoint

<br />

## Acquiring the tool

### Requirements

<img src="https://cdn.devlead.se/clipimg-vscode/2021/03/18/64b64f7a-9eaa-3753-71e5-781a58a6d188.png?sv=2019-12-12&st=2021-03-17T17%3A08%3A25Z&se=2031-03-18T17%3A08%3A25Z&sr=b&sp=r&sig=1iNd2EAsow19wsG8M2Av69Y52BjqUpzKl13bXiQxZ%2FY%3D" alt="Azure Log Analytics Workspace Agents management" align="right" />

`dpi` is a .NET 5 Tool, so .NET 5 SDK is required to be installed - you can find the download and install the latest version of the .NET SDK from [get.dot.net](https://get.dot.net/).

The tool is published to and distributed via [NuGet.org](https://www.nuget.org/packages/DPI/), but you could also upload it to your internal NuGet feed and have your users and pipelines obtain it from there.

To report you also need an Azure Log Analytics workspace, and the tool uses Workspace Id and Key (either primary or secondary, both work), you'll find those under "Agents management"

<br />

### Installing globally

.NET Tools can be installed globally for the current user using the .NET SDK CLI

```bash
dotnet tool install --global DPI
```

Once installed the tool will be available anywhere for the current user as `dotnet tool run dpi`, `dotnet dpi`, or just `dpi`.

### Installing via Tool manifest

My preferred way to install and maintain a .NET tool is using a .NET local tool manifest file added to your repository, this means which tools are used and their specific versions is maintained within the repository.

If you don't have a manifest in your repository already, the easiest way to create a new manifest is by using the .NET SDK CLI in your repo root.

```bash
dotnet new tool-manifest
```

then or if you already have a manifest you add the tool

```bash
dotnet tool install DPI
```

And the tool is now within the repository available as `dotnet tool run dpi` or `dotnet dpi`, then your pipelines and anyone cloning your repository can obtain all your specified tools with the same versions as you're using by executing

```bash
dotnet tool restore
```

<br />

## Usage

### NuGet Analyze command

`dpi nuget [SourcePath] [NUGET OPTIONS] analyze [ANALYZE OPTIONS]`

The NuGet analyze command will inventory the specified path recursively for known files containing references to NuGet packages and output its findings to the console, the current directory will be used if no path specified.

#### Example analyzing current directory

```bash
dotnet dpi nuget analyze
```

#### Example analyzing specific directory

```bash
dotnet dpi nuget ./path/to/folder analyze
```

### NuGet Report command

`dpi nuget [SourcePath] [NUGET OPTIONS] report [REPORT OPTIONS]`

The NuGet report command will beyond analyze also report tools findings as a custom log to Azure Log Analytics.

#### Example analyzing and reporting current directory

```bash
dotnet dpi nuget report --workspace <WORKSPACEID> --sharedkey <SHAREDKEY>
```

#### Example analyzing and reporting specific directory

```bash
dotnet dpi nuget ./path/to/folder report --workspace <WORKSPACEID> --sharedkey <SHAREDKEY>
```

Azure Log Analytics Workspace ID and Key can also be passed as environment variables `NuGetReportSettings_WorkspaceId` and `NuGetReportSettings_SharedKey`, which is the preferred way when passing secrets in a DevOps pipeline, the result of nuget analyze is reported to custom log `NuGetReport_CL` in the specified workspace.

### Console output formats

By using the `--output <FORMAT>` option you can change the console output of the tools analyze result, currently, the tool supports the following formats

- JSON - output in a machine consumable JSON format
- TABLE - outputs a nicely formatted table format

(You can use the `--file <FILEPATH>` option to output to file instead of console)

#### Example analyzing and outputting as json

```bash
dotnet dpi nuget --output json analyze
```

```json
[
  {
    "sessionId": "b770baf0-8eb7-496f-aa19-770f36838689",
    "buildProvider": "Local",
    "platformFamily": "Windows",
    "buildNo": "202103181806",
    "buildSCM": "dpi",
    "buildVersion": null,
    "sourceType": "CSProj",
    "source": "TestProj/TestProj.csproj",
    "targetFramework": "net5.0",
    "packageId": "Microsoft.Extensions.DependencyInjection",
    "version": "5.0.1",
    "timestamp": "2021-03-18T18:06:30.0178489+00:00",
    "Computer": "HAL"
  },
  {
    "sessionId": "b770baf0-8eb7-496f-aa19-770f36838689",
    "buildProvider": "Local",
    "platformFamily": "Windows",
    "buildNo": "202103181806",
    "buildSCM": "dpi",
    "buildVersion": null,
    "sourceType": "CSProj",
    "source": "TestProj/TestProj.csproj",
    "targetFramework": "net5.0",
    "packageId": "Cake.Core",
    "version": "1.1.0",
    "timestamp": "2021-03-18T18:06:30.0178489+00:00",
    "Computer": "HAL"
  }
]
```

This means that you as an example in PowerShell could do something like the below script to get all versions of `YamlDotNet` used.

```powershell
dotnet dpi nuget --output json analyze `
  | ConvertFrom-Json `
  | ForEach-Object { $_ } `
  | Where-Object { $_.packageId -eq 'YamlDotNet' } `
  | ForEach-Object version
```

#### Example analyzing and outputting as table

```bash
dotnet dpi nuget --output table analyze
```

![Console example dpi table format](https://cdn.devlead.se/clipimg-vscode/2021/03/18/2465161d-a1c7-5401-b67e-8d84cf66911a.png?sv=2019-12-12&st=2021-03-17T18%3A21%3A33Z&se=2031-03-18T18%3A21%3A33Z&sr=b&sp=r&sig=hsF2ReevVLx3mEaRmTumGikNQGuAJNGVD3FDB7zEOWM%3D)

<br />

## Pipeline examples

As `dpi` is just a command-line tool, you can just use the same commands in your build pipeline as you use in your shell of choice locally.

1. `dotnet tool restore` - Restore .NET Tools (i.e. download and install `dpi`)
1. `dotnet dpi <COMMAND> [COMMAND OPTIONS] <SUBCOMMAND> [SUB COMMAND OPTIONS]` - Execute `dpi` i.e. `nuget` `analyze` / `report`

The below pipeline examples you'll see two options not previously mentioned

- `--silent` - Silent removes all console log output except the `table` (`json` will bet "silent" by default so output can be parsed by another tool.)
- `--buildversion` - Build version is an option to pass metadata unique to the workflow, in this case, the unique run number from GitHub Action workflow, but could i.e. be asserted version from a tool like GitVersion.

### GitHub Action workflow example

Below YAML example will

1. Checks out code from the repository
1. Installs .NET SDK based on [global.json](https://docs.microsoft.com/en-us/dotnet/core/tools/global-json?WT.mc_id=DT-MVP-5002677) in repository
1. Restore .NET Tools (i.e. `dpi`)
1. Build project
1. `dpi` Analyze if secrets **aren't** available (i.e. a fork pull request)
1. `dpi` Report if secrets **are** available (i.e. merged into `main`)

```yaml
name: Build
on:
  pull_request:
  push:
    branches:
      - main

env:
  NuGetReportSettings_SharedKey: ${{ secrets.NUGETREPORTSETTINGS_SHAREDKEY }}
  NuGetReportSettings_WorkspaceId: ${{ secrets.NUGETREPORTSETTINGS_WORKSPACEID }}

jobs:
  build:
    runs-on: windows-latest
    steps:
      - name: Get the sources
        uses: actions/checkout@v2

      - name: Install .NET Core SDK
        uses: actions/setup-dotnet@v1

      - name: Restore .NET Tools
        run: dotnet tool restore

      - name: Build Project
        run: dotnet build

      - name: Run DPI Analyze
        if: ${{ env.NuGetReportSettings_SharedKey == null || env.NuGetReportSettings_WorkspaceId == null }}
        shell: bash
        run: dotnet dpi nuget --silent --output table analyze --buildversion $GITHUB_RUN_NUMBER

      - name: Run DPI Report
        if: ${{ env.NuGetReportSettings_SharedKey != null && env.NuGetReportSettings_WorkspaceId != null }}
        shell: bash
        run: dotnet dpi nuget --silent --output table report --buildversion $GITHUB_RUN_NUMBER
```

<br />

### Azure DevOps Pipelines example

Below YAML example will

1. Checks out code from the repository
1. Installs .NET SDK based on [global.json](https://docs.microsoft.com/en-us/dotnet/core/tools/global-json?WT.mc_id=DT-MVP-5002677) in the repository
1. Restore .NET Tools (i.e. `dpi`)
1. Build project
1. `dpi` Analyze if it's a pull request
1. `dpi` Report if executing in the `main` branch

```yaml
name: $(Year:yyyy).$(Month).$(DayOfMonth)$(Rev:.r)
trigger:
- main

pool:
  vmImage: 'windows-latest'

steps:
- task: UseDotNet@2
  displayName: 'Install .NET Core SDK'
  inputs:
    packageType: sdk
    useGlobalJson: true

- script: dotnet tool restore
  displayName: 'Restore .NET Tools'

- script: dotnet build src
  displayName: 'Build Project'

- script: dotnet dpi nuget --silent --output table analyze --buildversion "$(Build.BuildNumber)"
  displayName: Run DPI Analyze
  condition: eq(variables['Build.Reason'], 'PullRequest')

- script: dotnet dpi nuget --silent --output table report --buildversion "$(Build.BuildNumber)"
  displayName: Run DPI Report
  env:
    NuGetReportSettings_SharedKey: $(NUGETREPORTSETTINGS_SHAREDKEY)
    NuGetReportSettings_WorkspaceId: $(NUGETREPORTSETTINGS_WORKSPACEID)
  condition: eq(variables['Build.SourceBranch'], 'refs/heads/main')
```

<img src="https://cdn.devlead.se/clipimg-vscode/2021/03/19/659e0331-59b9-a7b6-1a72-81c04208beaa.png?sv=2019-12-12&st=2021-03-18T12%3A51%3A01Z&se=2031-03-19T12%3A51%3A01Z&sr=b&sp=r&sig=zEkSEyMfWQ5u4lJLxrPQHgooMxD10VmDNijiwnGkax0%3D" alt="Azure Pipelines Secrets" align="right" />

<br />

In the above example, the Azure Log Analytics Workspace Key and Id are passed as environment variables

- `NuGetReportSettings_SharedKey`
- `NuGetReportSettings_WorkspaceId`

which are populated from pipeline secret variables

- `NUGETREPORTSETTINGS_SHAREDKEY`
- `NUGETREPORTSETTINGS_WORKSPACEID`

Pipeline variables are administrated under your Pipeline details -> Edit -> Variables, they're made secrets by checking the "Keep this value secret" checkbox.

Secrets need to be explicitly defined in YAML to be accessible from tasks.

<br />

### Cake build examples

There's not yet a Cake addin or built-in support for `dpi`, but still, it's fairly straightforward to use `dpi` from a Cake script.

#### Example Cake script DPI installed globally / via .NET tool manifest

The Cake script below will

1. Setup context
    - `Version` - Date based here but normally fetched from build provider or asserted from a tool (*i.e. GitVersion*)
    - `Analyze` - flag for if `analyze` or `report` based on presence of environment variables
1. Build project
1. Analyze or Report based on context `Analyze` flag

```csharp
public record BuildData(string Version, bool Analyze);

Setup(
  static context => new BuildData(
    Version: FormattableString.Invariant(
                $"{DateTime.UtcNow:yyyy.M.d}"
              ),
    Analyze: new[] {
              "NuGetReportSettings_SharedKey",
              "NuGetReportSettings_WorkspaceId"
            }
            .Select(key => context.EnvironmentVariable(key))
            .Where(string.IsNullOrWhiteSpace)
            .Any()
  )
);

Task("Build")
    .Does<BuildData>(
        static (context, data) => context.DotNetCoreBuild(
          "src",
          new DotNetCoreBuildSettings {
            MSBuildSettings = new DotNetCoreMSBuildSettings()
              .WithProperty("Version", data.Version)
          }
    )
  );

Task("DPI")
  .IsDependentOn("Build")
  .Does<BuildData>(
      static (context, data) => context.DotNetCoreTool(
        "dpi",
        new DotNetCoreToolSettings {
            ArgumentCustomization = args => args
                                              .Append("nuget")
                                              .Append("--silent")
                                              .AppendSwitchQuoted("--output", "table")
                                              .Append(data.Analyze ? "analyze" : "report")
                                              .AppendSwitchQuoted("--buildversion", data.Version)
        }
      )
  );

Task("Default")
  .IsDependentOn("DPI");

RunTarget(Argument("target", "Default"));
```

#### Example Self-contained Cake script

The Cake script below will

1. Install the specified version of `dpi` into Cake tool directory from NuGet
1. Setup context
    - `Version` - Date based here but normally fetched from build provider or asserted from a tool (*i.e. GitVersion*)
    - `Analyze` - flag for if `analyze` or `report` based on presence of environment variables
1. Build project
1. Analyze or Report based on context `Analyze` flag

```csharp
#tool dotnet:?package=DPI&version=2021.3.16.28

public record BuildData(string Version, bool Analyze);

Setup(
  static context => new BuildData(
    Version: FormattableString.Invariant(
                $"{DateTime.UtcNow:yyyy.M.d}"
              ),
    Analyze: new[] {
              "NuGetReportSettings_SharedKey",
              "NuGetReportSettings_WorkspaceId"
            }
            .Select(key => context.EnvironmentVariable(key))
            .Where(string.IsNullOrWhiteSpace)
            .Any()
  )
);

Task("Build")
    .Does<BuildData>(
        static (context, data) => context.DotNetCoreBuild(
          "src",
          new DotNetCoreBuildSettings {
            MSBuildSettings = new DotNetCoreMSBuildSettings()
              .WithProperty("Version", data.Version)
          }
    )
  );

Task("DPI")
  .IsDependentOn("Build")
  .Does<BuildData>(
      static (context, data) => context.StartProcess(
        context.Tools.Resolve("dpi") ?? context.Tools.Resolve("dpi.exe"),
        new ProcessSettings {
            Arguments = new ProcessArgumentBuilder()
                                                .Append("nuget")
                                                .Append("--silent")
                                                .AppendSwitchQuoted("--output", "table")
                                                .Append(data.Analyze ? "analyze" : "report")
                                                .AppendSwitchQuoted("--buildversion", data.Version)
        }
    )
  );

Task("Default")
  .IsDependentOn("DPI");

RunTarget(Argument("target", "Default"));
```

<br />

## Analyzing the data

NuGet package references end up as a custom log named `NuGetReport_CL` in your Azure Log Analytics Workspace, it will automatically as data is ingested infer types (`dates`,`strings`, `guids`, etc.) and using [KQL (Kusto Query Language)](https://docs.microsoft.com/en-us/azure/data-explorer/kusto/query/?WT.mc_id=AZ-MVP-5002677) you can now start to explore discover, correlate and aggregate.

![Azure Log Analytics Workspace General Logs NuGetReport_CL](https://cdn.devlead.se/clipimg-vscode/2021/03/19/927adaa4-2902-74f1-e587-44377b7131e2.png?sv=2019-12-12&st=2021-03-18T14%3A49%3A47Z&se=2031-03-19T14%3A49%3A47Z&sr=b&sp=r&sig=4eUp0%2F4y2jRxH1rCXJbz25%2BteDLSKSuiNv0V9oWti20%3D)

*(Tip there's a [Log Analytics tutorial](https://docs.microsoft.com/en-us/azure/azure-monitor/logs/log-analytics-tutorial?WT.mc_id=AZ-MVP-5002677) on [Microsoft Docs](https://docs.microsoft.com/en-us/azure/azure-monitor/logs/log-analytics-tutorial?WT.mc_id=AZ-MVP-5002677) if you're new to Azure Log Analytics)*

### Top 5 used packages

The below query counts unique repositories group by package id and returns the five with the highest package count.

```fsharp
NuGetReport_CL
| where TimeGenerated >= ago(2d)
| summarize Count = dcount(buildSCM_s) by packageId_s
| order by Count desc, packageId_s asc
| take 5
```

<br />

### Top 5 used packages by last report per repository

When querying you in general just want the latest set of data per repository, to solve this you can use the fact that each report execution has a unique session-id, and each log has a date/time when ingested.

By fetching the last session-id we can get a more correct picture of package usage, as a package could have been removed/updated between reports. There are several ways to solve this, in the example below, we use the [arg_max](https://docs.microsoft.com/en-us/azure/data-explorer/kusto/query/arg-max-aggfunction?WT.mc_id=AZ-MVP-5002677) aggregation function to get the session id based on the highest time generated grouped by repository name using the [summarize](https://docs.microsoft.com/en-us/azure/data-explorer/kusto/query/summarizeoperator?WT.mc_id=AZ-MVP-5002677) operator.

```fsharp
NuGetReport_CL
| where TimeGenerated >= ago(2d)
| summarize arg_max(TimeGenerated, sessionId_g) by buildSCM_s
| join NuGetReport_CL on $left.sessionId_g == $right.sessionId_g
| summarize Count = dcount(buildSCM_s) by packageId_s
| order by Count desc, packageId_s asc
| take 5
```

<br />

![Query Top 5 used packages by last report per repository](https://cdn.devlead.se/clipimg-vscode/2021/03/20/top5packagesquery.gif?sp=r&st=2021-03-19T11:32:00Z&se=2031-03-21T11:32:00Z&sv=2020-02-10&sr=b&sig=hMWNA6rshw2MfQ8WmufiaLcJ%2FaektrDjnhsu6qotrSo%3D)

<br />

### Custom log NuGetReport_CL columns

Above were just a couple of examples of NuGet package references queries, but there are almost endless possibilities, and the columns to your disposable are

| Name                | Description                               | Example value                                                               |
|---------------------|-------------------------------------------|-----------------------------------------------------------------------------|
| `TimeGenerated`     | Date/time **UTC** data ingested           | `2021-03-18T15:07:06.46Z`                                                   |
| `Computer`          | Machine name log generated on             | `fv-az68-89`                                                                |
| `buildVersion_s`    | Value passed using `--buildversion`       | `2021.03.18.18138`                                                          |
| `buildNo_s`         | Build number from build provider          | `664953123-3544`                                                            |
| `timestamp_t`       | Date/time **UTC** data reported           | `2021-03-18T15:07:04.798Z`                                                  |
| `platformFamily_s`  | Operating system reported from            | `Linux`, `OSX`, `Window`                                                    |
| `buildSCM_s`        | Source control reference                  | `devlead/DPI`                                                               |
| `buildProvider_s`   | Build provider                            | `GitHubActions`, `AppVeyor`, `AzurePipelinesHosted`                         |
| `source_s`          | Relative file path where references found | `src/DPI/DPI.csproj`                                                        |
| `sourceType_s`      | Type of source file                       | `CSProj`, `Cake`, `DotNetToolsManifest`, `PackagesConfig`, `ProjectAssets`  |
| `packageId_s`       | NuGet package id                          | `Cake.Bridge.DependencyInjection`                                           |
| `version_s`         | NuGet package version                     | `0.5.0`                                                                     |
| `sessionId_g`       | Unique correlation id for each report     | `b85f687f-9e34-43c6-8a45-f8b803b373ce`                                      |
| `targetFramework_s` | Package .NET target framework             | `net5.0`                                                                    |

<br />

## Future

This project was initially done for my own fun and profit, and during that process realized that this can be really useful.

There's currently no set roadmap, but there are a few things on my radar that I would like to add support for:

- Docker images
- NPM packages
- More .NET project types
- SDK versions
- Reporting to more services and formats

And that I would happily take contributions in form of PRs and issues, the code is open-source and available on GitHub at [github.com/devlead/DPI](https://github.com/devlead/DPI).

## Conclusion

`dpi` is starting to become a really versatile tool, it's a bit abstract to explain, and I've struggled with that a bit with this blog post, but hopefully, I've at least somewhat painted a picture of what's possible, and what problems it can help to solve. I've personally found the process of ingesting "raw" data and then querying both powerful and inspirational, some answers have led to going down rabbit holes of answering new questions.

Please take it for a spin and if you have any questions, suggestions, or even opinions please feel free to reach out.

## Thanks

This tool was made possible thanks to a couple of excellent open source projects, special thanks go out to:

- [Spectre.Console](https://github.com/spectresystems/spectre.console)
- [Spectre.Cli.Extensions.DependencyInjection](https://github.com/agc93/spectre.cli.extensions.dependencyinjection)
- [Reactive Extensions](https://github.com/dotnet/reactive)
- [Cake](https://github.com/cake-build/cake/)
- [Cake.Bridge.DependencyInjection](https://github.com/devlead/Cake.Bridge.DependencyInjection)
