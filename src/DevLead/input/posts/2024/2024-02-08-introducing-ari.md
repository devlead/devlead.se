---
title: Introducing ARI
tags:
    - .NET
    - Azure
    - DevOps
    - ARM
    - IAAS
    - Tool
author: devlead
description: A DevOps tool to document a Azure tenant resources
published: 2024-02-08
image: https://cdn.devlead.se/clipimg-vscode/2024/02/08/23be558ef4784e74a88b6bab00141708.jpg?sv=2021-10-04&st=2024-02-08T19%3A03%3A22Z&se=2050-12-16T19%3A05%3A00Z&sr=b&sp=r&sig=MYOLIoM7zuZkWN7OQmLDVMTP796IKEhotMKRCtPYIsY%3D
---

If you are working with Azure, you might have encountered the challenge of keeping track of all the subscriptions, resource groups, and resources that you have in your tenant. You might also want to document them in a clear and consistent way, for example, for compliance, auditing, or reporting purposes.

That's why I created ARI, a .NET tool that inventories and documents your Azure tenant's subscriptions, resource groups, and resources. ARI stands for Azure Resource Inventory, and it is a free and open source tool that you can install and use with the .NET 7 or 8 SDK.

## How to install ARI

ARI is available as a NuGet package at [nuget.org/packages/ARI](https://www.nuget.org/packages/ARI). You can install it either globally on your machine or locally in a specific folder or repository.

To install it globally, run the following command:

```bash
dotnet tool install --global ARI
```

To install it locally, you need to create a tool manifest file first. You can do this by running the following command in the folder where you want to use ARI:

```bash
dotnet new tool-manifest
```

Then, you can install ARI in that folder by running:

```bash
dotnet tool install --local ARI
```

## How to use ARI

Once you have installed ARI, you can use it by typing `ari` if you installed it globally, or `dotnet ari` if you installed it locally.

You can use the `-h` or `--help` parameters to get the current list of available commands and options. For example:

```bash
ari --help
```

or

```bash
ari <command> --help
```

The main current available command of ARI is `inventory`, which takes a tenant ID and an output path as required parameters. It also has some optional parameters that you can use to customize the inventory process. For example:

```bash
ari inventory <tenantId> <outputpath> [options]
```

The `inventory` command will scan your Azure tenant and generate a set of GitHub flavoured markdown files that document your subscriptions, resource groups, and resources. It will also create indexes by tag name and value, or by resources missing tags.

## How to contribute to ARI

ARI is licensed under the MIT license, which means you can use it for any purpose, modify it, and distribute it freely. The source code is hosted on GitHub at [github.com/devlead/ari](https://github.com/devlead/ari).

I welcome any feedback, suggestions, bug reports, or pull requests from the community. If you want to contribute to ARI, please check out the issues on GitHub.

## How to use the output of ARI

The output of ARI is a set of GitHub flavored markdown files that you can use for various purposes. For example, you can use them with static site generators, wikis, or other tools that support markdown.

One example of a static site generator that works well with ARI is Statiq ( [statiq.dev](https://statiq.dev) ), which is also a .NET framework for generating static websites and markdown files is one of its supported input formats. You can see an example of a static website generated using Statiq and ARI at [devlead.se/ARI](https://www.devlead.se/ARI/).

I hope you find ARI useful and helpful for your Azure projects. Please let me know what you think and how I can improve it.

Thank you for reading!
