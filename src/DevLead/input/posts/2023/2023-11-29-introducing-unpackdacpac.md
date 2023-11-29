---
title: Introducing UnpackDacPac
tags:
    - .NET
    - SQL
    - Dacpac
    - Dacfx
    - Tool
author: devlead
description: A .NET Tool for Extracting DAC Packages
published: 2023-11-29
image: https://cdn.devlead.se/clipimg-vscode/2023/11/29/d29f363677f845559d19401cd5a47318.jpg?sv=2021-10-04&st=2023-11-29T17%3A14%3A21Z&se=2035-11-30T17%3A14%3A00Z&sr=b&sp=r&sig=ga7enWBL804T0haYj29QJE7hvaV14REXobavSOZ4wwU%3D
---

If you work with SQL Server databases, you may have encountered DAC packages, or dacpacs, which are a way of packaging a database's schema and seed data for deployment or migration. Dacpacs are useful for deploying databases to different environments, such as development, testing, or production, but it's tooling comes with some limitations. For example, you cannot easily inspect the contents of a dacpac file without having a running instance of SQL Server.

That's why I created UnpackDacPac, a .NET tool that enables you to extract a dacpac file and generate a deployment script to a target folder without the need to deploy it to an actual database. A really useful for example when you need to debug a failed deployment or retrieve an old version of a SQL object from just a DevOps build artifact.
UnpackDacPac is a cross-platform tool that runs on .NET SDK 6, 7, and 8 (*If you don't have the .NET SDK installed, you can get it from [here](https://get.dot.net)*).

## Installation

You can install UnpackDacPac as a global tool from the command line by typing:

```bash
dotnet tool install --global UnpackDacPac
```

## Usage

The usage of UnpackDacPac is simple:

```bash
unpackdacpac unpack <dacPacPath> <outputPath> [OPTIONS]
```

For example, if you have a dacpac file named Source.dacpac and you want to extract it to a folder named TargetPath, you can run:

```bash
unpackdacpac unpack Source.dacpac TargetPath
```

This will create the following files in the TargetPath folder:

- `DacMetadata.xml`: This file contains the metadata of the dacpac, such as the name, version, description, and dependencies of the database.
- `Deploy.sql`: This file contains the generated deployment script that can be used to create or update a database from the dacpac.
- `model.sql`: This file contains the formatted SQL code that defines the schema and data of the database.
- `model.xml`: This file contains the XML representation of the database model.
- `Origin.xml`: This file contains the origin information of the dacpac, such as the source server and database name.
- `postdeploy.sql`: This file contains any post-deployment scripts that are included in the dacpac.

One of the features of UnpackDacPac is that you can exclude certain types of objects from the generated deployment script by using the `--deploy-script-exclude-object-type` parameter. For example, if you want to exclude users, logins, and role memberships from the deployment script, you can run:

```bash
unpackdacpac unpack Source.dacpac TargetPath --deploy-script-exclude-object-type Users --deploy-script-exclude-object-type Logins --deploy-script-exclude-object-type RoleMembership
```

This will generate a deployment script that does not contain statements related to users, logins, or role memberships.

## Where to find UnpackDacPac?

UnpackDacPac is an open-source project under a permissive MIT license, the source can be found on [GitHub](https://github.com/devlead/UnpackDacPac) and the NuGet package at [NuGet.org](https://www.nuget.org/packages/UnpackDacPac).

## Feedback and suggestions

I hope you find UnpackDacPac useful and feel free to provide any feedback, suggestions, or pull requests.

*Happy unpacking!*
