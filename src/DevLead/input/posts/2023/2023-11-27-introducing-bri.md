---
title: Introducing BRI
tags:
    - .NET
    - Azure
    - DevOps
    - Bicep
    - IAAS
    - Tool
    - DevOpsDocs
author: devlead
description: A DevOps tool to document Bicep modules in a Azure container registry
published: 2023-11-27
image: https://cdn.devlead.se/clipimg-vscode/2022/11/27/9a2ae7d59ab04134b044699dfa437e61.jpg?sv=2021-10-04&st=2023-11-27T19%3A40%3A55Z&se=2031-11-28T19%3A40%3A00Z&sr=b&sp=r&sig=0KWUea7yaM5YorSAtmNaMUCHPjJRaMiqif95E1o%2BW%2Bo%3D
---

Are you looking for a way to document your Azure [Bicep](https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/overview?tabs=bicep) [modules](https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/modules) in a simple and elegant way? If so, you might want to check out BRI, a .NET tool that I created to help you with that.

## What is BRI?

BRI stands for Bicep registry inventory, and it does exactly what the name suggests: it scans a provided Azure Container Registry for any published Bicep modules and generates markdown files for each module version. These markdown files contain information about the module's input parameters, output values, and example usage. You can then use these files with any static site generator that supports markdown, such as [Statiq](https://statiq.dev/), to create a beautiful documentation site for your modules.

## How to use BRI?

BRI is very easy to use. You just need to install it from [NuGet.org](https://www.nuget.org/packages/bri) using the [.NET SDK](https://get.dot.net) [tool install command](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-tool-install). It supports both .NET 7 and .NET 8, and you can install it globally or locally. For example, to install it globally, you can run:

```bash
dotnet tool install --global BRI
```

Or, to install it locally (tool manifest), you can run:

```bash
dotnet tool install --local BRI
```

Once installed, you can run BRI by typing (`dotnet bri` if installed locally):

```bash
bri inventory <container registry name> <output path>
```

For example, to scan the container registry named myregistry.azurecr.io and generate markdown files in the ./output/path folder, you can run:

```bash
bri inventory myregistry.azurecr.io ./output/path
```

## Example

You can see an example of the output that BRI generates at [www.devlead.se/bri/](https://www.devlead.se/bri/britool.azurecr.io/bicep/modules/bri/4.0.0.0.html)
[![example](https://cdn.devlead.se/clipimg-vscode/2022/11/27/00ee924d-9de2-4b2b-90cc-1b5a8bcfb696_small.jpg?sv=2021-10-04&st=2023-11-27T19%3A39%3A50Z&se=2031-11-28T19%3A39%3A00Z&sr=b&sp=r&sig=oree1I2Ia%2BqTdwXxZ1teSRHTf9iPODDLjjp5HvUGVsA%3D)](https://www.devlead.se/bri/britool.azurecr.io/bicep/modules/bri/4.0.0.0.html).

## Authentication

By default, it'll try to authenticate using the [DefaultAzureCredential](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.defaultazurecredential?view=azure-dotnet) which tries to authorize in the following order based on your environment.

1. [EnvironmentCredential](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.environmentcredential?view=azure-dotnet)
1. [WorkloadIdentityCredential](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.workloadidentitycredential?view=azure-dotnet)
1. [ManagedIdentityCredential](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.managedidentitycredential?view=azure-dotnet)
1. [SharedTokenCacheCredential](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.sharedtokencachecredential?view=azure-dotnet)
1. [VisualStudioCredential](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.visualstudiocredential?view=azure-dotnet)
1. [VisualStudioCodeCredential](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.visualstudiocodecredential?view=azure-dotnet)
1. [AzureCliCredential](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.azureclicredential?view=azure-dotnet)
1. [AzurePowerShellCredential](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.azurepowershellcredential?view=azure-dotnet)
1. [AzureDeveloperCliCredential](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.azuredeveloperclicredential?view=azure-dotnet)
1. [InteractiveBrowserCredential](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.interactivebrowsercredential?view=azure-dotnet)

If running in a DevOps pipeline recommended way is to use a managed/workload identity, or create an Entra ID [service principal](https://learn.microsoft.com/en-us/entra/identity-platform/app-objects-and-service-principals?tabs=browser) and set the following environment variables fetched from pipeline secrets

1. `AZURE_TENANT_ID` to its tenant ID
1. `AZURE_CLIENT_ID` to its client ID
1. `AZURE_CLIENT_SECRET` to its secret.
1. `AZURE_AUTHORITY_HOST` to `https://login.microsoftonline.com/`

## Where to find BRI?

BRI is open source and licensed under the MIT license, so you can use it freely and modify it as you wish. You can find the source code on [GitHub](https://github.com/devlead/BRI) and the NuGet package at [NuGet.org](https://www.nuget.org/packages/bri).

## Feedback and suggestions

I hope you find BRI useful and that it helps you with your Bicep module development. If you have any feedback or suggestions, please feel free to open an issue or a pull request on GitHub. Happy coding!
