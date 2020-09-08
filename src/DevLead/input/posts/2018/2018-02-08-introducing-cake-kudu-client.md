---
title: Introducing Cake Kudu Client
tags:
    - .NET
    - Azure
    - C#
    - Cake
    - DevOps
author: devlead
published: 2018-02-08
canonical: https://medium.com/hackernoon/introducing-cake-kudu-client-abda40d15f38
description: Ship remotely to Azure App Services using Cake
---
![Long ship at sea](https://cdn.devlead.se/clipimg-vscode/2021/01/11/4e020ade-f4b0-0bc4-55d8-fccd0fc0e416.png?sv=2019-12-12&st=2021-01-10T12%3A45%3A04Z&se=2031-01-11T12%3A45%3A04Z&sr=b&sp=r&sig=uakWSiJ6qtkF8nHV89itEeTnzN48MTOAzVLHZmfE77g%3D)

I’ve previously written how you on Azure App Services itself can build and deploy your web applications and functions utilizing [Cake](https://cakebuild.net/) build scripts and the [Cake.Kudu](https://www.nuget.org/packages/Cake.Kudu) addin.

While it’s fairly easy to get going, just add a build script and connect it to your source code repository of choice, for some scenarios it makes more sense to ship your application prebuilt.

A few of those scenarios can be

* **Static web sites**, where it requires more resources build, than to serve.
* **Multi region apps**, where you deploy same bits to multiple sites and makes sense to just build it once.
* **Build requirements**, by building on App services you’re limited to the tooling it has pre-installed or it’s environment supports.
* **Private resources**, using private nugget feeds or other compile time sensitive information can be cumbersome, and might not be what you want to have lying around on your web site.

## Cake Kudu Client to the rescue

Kudu the “engine” behind Azure web and function apps provides and http-based API, which deploy is one of the features it offers. And it’s this API the “Cake Kudu Client” provides a typed C# API which makes it close to a one-liner to deploy from a directory or zip file.

```csharp
 #addin nuget:?package=Cake.Kudu.Client

 string  baseUri     = EnvironmentVariable("KUDU_CLIENT_BASEURI"),
         userName    = EnvironmentVariable("KUDU_CLIENT_USERNAME"),
         password    = EnvironmentVariable("KUDU_CLIENT_PASSWORD");

 IKuduClient kuduClient = KuduClient(
     baseUri,
     userName,
     password);

 DirectoryPath sourceDirectoryPath = "./Documentation/";

 kuduClient.ZipDeployDirectory(
     sourceDirectoryPath);
```

What the above does is essentially

1. From environment variables gets Kudu endpoint for your App Service (i.e. `https://{yoursite}.scm.azurewebsites.net`), user name and password.
1. Instantiate a new client using the `KuduClient` alias.
1. Use the `ZipDeployDirectory` method which with zip that folder in memory and deploy it.

A real world example using this with the static site generator WYAM could look something like this:

```csharp
#tool "nuget:https://api.nuget.org/v3/index.json?package=Wyam&version=1.2.0"
#addin "nuget:https://api.nuget.org/v3/index.json?package=Cake.Wyam&version=1.2.0"
#addin "nuget:https://api.nuget.org/v3/index.json?package=Cake.Kudu.Client&version=0.1.0"

DirectoryPath   outputPath = MakeAbsolute(Directory("./output"));
string          target     = Argument("target", "Kudu-Publish-Documentation"),
                baseUri    = EnvironmentVariable("KUDU_CLIENT_BASEURI"),
                userName   = EnvironmentVariable("KUDU_CLIENT_USERNAME"),
                password   = EnvironmentVariable("KUDU_CLIENT_PASSWORD");

Task("Clean-Documentation")
    .Does(() =>
{
    CleanDirectory(outputPath);
});

Task("Generate-Documentaton")
    .IsDependentOn("Clean-Documentation")
    .Does(() =>
{
    Wyam(new WyamSettings
    {
        Recipe = "Docs",
        Theme = "Samson",
        OutputPath = outputPath,
        Settings = new Dictionary<string, object>
        {
            { "BaseEditUrl", "https://github.com/cake-contrib/Cake.Kudu.Client" },
            { "SourceFiles", "./src" },
            { "Title", "Cake Kudu Client" }
    }});
});

Task("Kudu-Publish-Documentation")
    .IsDependentOn("Generate-Documentaton")
    .WithCriteria(!string.IsNullOrEmpty(baseUri)
        && !string.IsNullOrEmpty(userName)
        && !string.IsNullOrEmpty(password)
    )
    .Does(()=>
{
    IKuduClient kuduClient = KuduClient(
        baseUri,
        userName,
        password);

    kuduClient.ZipDeployDirectory(
        outputPath);
});

RunTarget(target);
```

## Not just deployments

Deployments is only one of the features the Cake Kudu Client addin handles

* Execute remote shell commands
* Enumerate remote files and directories on
* Upload files and directories to AppService
* Download files and directories from AppService
* Deploy to AppService from local folder or zip file

and more features are planned.

You can find the complete list of available methods with examples are available on the Cake web site at:
[cakebuild.net/dsl/kudu/](https://cakebuild.net/dsl/kudu/)
