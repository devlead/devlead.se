---
title: Introducing Cake ClickOnce Recipe
tags:
    - .NET
    - Azure
    - C#
    - Cake
    - WPF
    - OSS
author: devlead
description: An opinionated recipe for building and publishing .NET 5 Windows apps
published: 2021-03-03
image: https://cdn.devlead.se/clipimg-vscode/2021/01/31/efaa9de4-9e5c-e66b-8b35-3f6b3475b8d3.png?sv=2019-12-12&st=2021-01-30T15%3A03%3A52Z&se=2031-01-31T15%3A03%3A52Z&sr=b&sp=r&sig=wSUYJRdZjnHt0AZ86nc06YeF5eKqwXWAH%2F5525wyiAk%3D
---

A little over a decade ago I did a lot of business applications with .NET Framework WPF and ClickOnce was in many cases used as a convenient way to deploy and update Windows applications. ClickOnce hasn't gotten much love over the years, and when .NET Core 3 introduced support for WPF applications ClickOnce support was nowhere to be found. But guess what, that's changed with .NET 5 and in this post, I'll go through my recipe for simplifying the building and publishing .NET 5 Windows application using GitHub Actions, Cake, and ClickOnce to Azure Blob Storage.

## Cake Recipe?

So what is a Cake recipe? Regardless of what it sounds like, it in this case has nothing to do with baking, but it's a set of [Cake](https://cakebuild.net/) scripts packaged as NuGet package and published on NuGet.org, providing a highly reusable way to with just a few lines of code get access to rich functionality.

For example this recipe will for a .NET 5 based Windows application, in a very optioned way

1. Version application
1. Build application
1. Create ClickOnce assets
1. Deploy to Azure Blob storage

## Sample application

For simplicity I'm here going to use the standard .NET 5 WPF template, using .NET CLI that would look something like below

```bash
dotnet new wpf -n MyApp
dotnet new sln -n MyApp
dotnet sln add MyApp/MyApp.csproj
```

Which essentially:

1. Creates project
1. Creates solution
1. Adds project to solution

Which is similar to *New Project -> WPF Application (C# / Windows / Desktop .NET Core)*

![Create new project in Visual Studio 2019](https://cdn.devlead.se/clipimg-vscode/2021/01/31/9348f210-84a6-74b2-5f9f-6f31e20b1877.png?sv=2019-12-12&st=2021-01-30T17%3A12%3A36Z&se=2031-01-31T17%3A12%3A36Z&sr=b&sp=r&sig=YnOmvtwboOr30u0v1EnSsdpBpJnHl%2FcwEIKr9p%2FAQ1Y%3D)

![.NET Core WPF Application Template](https://cdn.devlead.se/clipimg-vscode/2021/01/31/6dadc800-fcc0-31ab-f4f3-9446b5f5c411.png?sv=2019-12-12&st=2021-01-30T17%3A13%3A34Z&se=2031-01-31T17%3A13%3A34Z&sr=b&sp=r&sig=jdfRM7ANfxT8XyzhzfXKZfv2U8JV%2B%2Bl%2BREX4ALUpdiM%3D)

Resulting in a bare minimum folder/file structure like below

```text
src
 │    MyApp.sln
 │
 └─── MyApp
        App.xaml
        App.xaml.cs
        AssemblyInfo.cs
        MainWindow.xaml
        MainWindow.xaml.cs
        MyApp.csproj
```

## Adding Cake.ClickOnce.Recipe

Now that we have our sample application, let's rub some DevOps on it using Cake.ClickOnce.Recipe.

### Prerequisites

#### Tools

To run our recipe we need one .NET tool installed

1. Cake.Tool 1.0.0 or newer

My preferred way is to install using a .NET tool manifest in repo root, so the tools are versioned and restored within the repo, a manifest is easiest created using the .NET CLI template `tool-manifest`:

```bash
dotnet new tool-manifest
```

and then install the tool

```bash
dotnet tool install Cake.Tool
```

#### Azure

The recipe uses Azure Blob Storage to distribute the application and ClickOnce manifest, so you'll need to create:

1. Azure Storage Account
1. Container with anonymous read access for blobs only

![New Storage Collage](https://cdn.devlead.se/clipimg-vscode/2021/01/31/2afa2bbc-0d9a-7898-d056-d9a221d8dc1d.png?sv=2019-12-12&st=2021-01-30T20%3A42%3A16Z&se=2031-01-31T20%3A42%3A16Z&sr=b&sp=r&sig=Kx6eIPJh9ZHdOvWEeP6GJl%2BChVgyrR21%2FO7qlEJ7us4%3D)

### Adding recipe

In repo root create a `build.cake` file containing

```csharp
#load "nuget:?package=Cake.ClickOnce.Recipe&version=0.2.0"

ClickOnce.ApplicationName = "MyApp";
ClickOnce.Publisher = "devlead";
ClickOnce.PublishUrl = "https://cakeclickonceexample.blob.core.windows.net/publish";
ClickOnce.RunBuild();
```

The Recipe has three required settings

1. `ApplicationName` - in general same as project name
1. `Publisher` - name of publisher
1. `PublishUrl` - base url for where application is installed from, in this case Azure Blob Storage container url

## Local build

We're now all set to kick of a local build using Cake

```bash
dotnet cake
```

and if all goes well you should see something like below

```text
Task                           Duration
---------------------------------------------------
Setup                          00:00:00.0070864
Clean                          00:00:00.0114975
Restore                        00:00:01.0136714
Build                          00:00:02.6537503
Publish                        00:00:00.9728737
ClickOnce-Launcher             00:00:00.3629962
ClickOnce-Application-Manifest 00:00:00.5536790
ClickOnce-Deployment-Manifest  00:00:00.5924181
---------------------------------------------------
Total:                         00:00:06.1730701
```

and a new folder in repo root called artifacts containing application ClickOnce manifest and a date versioned folder (you can override the generated version using `ClickOnce.Version` property).

```text
artifacts
 │   MyApp.application
 │
 └───MyApp.2021.01.31.25014
        Launcher.exe
        MyApp.deps.json
        MyApp.dll
        MyApp.exe
        MyApp.manifest
        MyApp.pdb
        MyApp.runtimeconfig.json
```

## GitHub Actions

A fairly minimal GitHub Actions could essentially be

1. Checkout source
1. Install .NET SDK
1. Install & Execute Cake

### Build definition

The yaml build definition could look something like below

```yaml
name: Build
on:
  pull_request:
  push:
    branches:
      - main
      - develop

jobs:
  build:
    name: Build
    runs-on: windows-latest
    steps:
      - name: Get the sources
        uses: actions/checkout@v2

      - name: Install .NET Core SDK
        uses: actions/setup-dotnet@v1

      - name: Run Cake script
        env:
          PUBLISH_STORAGE_ACCOUNT: ${{ secrets.PUBLISH_STORAGE_ACCOUNT }}
          PUBLISH_STORAGE_CONTAINER: ${{ secrets.PUBLISH_STORAGE_CONTAINER }}
          PUBLISH_STORAGE_KEY: ${{ secrets.PUBLISH_STORAGE_KEY }}
        run: |
          dotnet tool restore
          dotnet cake
```

The `Run Cake script` step has three environment variables defined

- `PUBLISH_STORAGE_ACCOUNT` - Azure storage account name i.e. `cakeclickonceexample`
- `PUBLISH_STORAGE_CONTAINER` - Azure storage container name i.e. `publish`
- `PUBLISH_STORAGE_KEY` - Azure storage account secondary or primary key.

These are fetched from your GitHub repo settings Secrets section, which means they're not publicly stored anywhere in the repo and also not available from pull requests.

![GitHub Repo Secrets settings](https://cdn.devlead.se/clipimg-vscode/2021/03/02/8ebf213e-1f0d-8d68-d12d-58ceafe6d728.png?sv=2019-12-12&st=2021-03-01T21%3A51%3A01Z&se=2031-03-02T21%3A51%3A01Z&sr=b&sp=r&sig=U%2BCyMGwq5x66LcKonxmi%2FjD52btWXph5Ku2P33T8rGE%3D)

### Output Example on GitHub Actions

The recipe will automatically identify that it's executing on GitHub Actions, and execute a few extra steps to upload and point the ClickOnce manifest to the new version.

If all goes well GitHub Actions output would look something like below

```bash

----------------------------------------
Setup
----------------------------------------
Setting up version 2021.01.27.18139
▶ "Clean"
▶ "Restore"
▶ "Build"
▶ "Publish"
▶ "ClickOnce-Launcher"
▶ "ClickOnce-Application-Manifest"
▶ "ClickOnce-Deployment-Manifest"
▶ "ClickOnce-Deployment-UpdateManifest"
▶ "ClickOnce-Deployment-CreateAppRef"
▶ "ClickOnce-Upload-Version"
▶ "ClickOnce-Upload-Application"
▶ "Publish-ClickOnce"

Task                                Duration
--------------------------------------------------------
Setup                               00:00:00.0160939
Clean                               00:00:00.0084806
Restore                             00:00:02.1274733
Build                               00:00:03.3076849
Publish                             00:00:01.2192429
ClickOnce-Launcher                  00:00:00.4506914
ClickOnce-Application-Manifest      00:00:00.6510728
ClickOnce-Deployment-Manifest       00:00:00.9086913
ClickOnce-Deployment-UpdateManifest 00:00:00.6800874
ClickOnce-Deployment-CreateAppRef   00:00:00.0112772
ClickOnce-Upload-Version            00:00:02.1736495
ClickOnce-Upload-Application        00:00:00.6269294
--------------------------------------------------------
Total:                              00:00:12.1814083
```

and you now have a fully automated build and release of your ClickOnce application.

## Installing application

The recipe will automatically generate an `appref-ms` file, downloading and opening this file from blob storage is enough to trigger the ClickOnce runtime and install the application.

Example: [cakeclickonceexample.blob.core.windows.net/publish/MyApp.appref-ms](https://cakeclickonceexample.blob.core.windows.net/publish/MyApp.appref-ms)

![ClickOnce installation example](https://cdn.devlead.se/clipimg-vscode/2021/03/02/installclickonce.gif?sp=r&st=2021-03-01T23:03:03Z&se=2031-03-03T07:03:03Z&spr=https&sv=2020-02-10&sr=b&sig=hSBI7hfdtkWR0SqdX5yiHDeE9E%2FwZzWCEHTZqp518Ls%3D)

## Conclusion

Long story short, the recipe greatly simplifies the automation of building a WPF application, getting it published and deployed using ClickOnce technologies - achieving this only configuring the bare minimum what's needed to do so.

There's still a lot of work to do with the recipe especially around signing, icons, etc.

That said it's fully functional and a good starting point.
The recipe is open source, so I'll happily take feedback and contributions to improve it.

## Resources

- Recipe source on GitHub - [github.com/devlead/Cake.ClickOnce.Recipe](https://github.com/devlead/Cake.ClickOnce.Recipe)
- Recipe on NuGet.org - [nuget.org/packages/Cake.ClickOnce.Recipe](https://www.nuget.org/packages/Cake.ClickOnce.Recipe/)
- Example repository on GitHub - [github.com/devlead/Cake.ClickOnce.Recipe.Example](https://github.com/devlead/Cake.ClickOnce.Recipe.Example)
- Cake - [cakebuild.net](https://cakebuild.net/)
