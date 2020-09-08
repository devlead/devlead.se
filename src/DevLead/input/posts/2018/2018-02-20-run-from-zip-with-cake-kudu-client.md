---
title: “Run-From-Zip” with Cake Kudu Client
tags:
    - .NET
    - Azure
    - C#
    - Cake
    - DevOps
author: devlead
published: 2018-02-20
canonical: https://medium.com/hackernoon/run-from-zip-with-cake-kudu-client-5c063cd72b37
description: A new way to deploy your Azure Web Apps and Function Apps
---
![Ship at sea](https://cdn.devlead.se/clipimg-vscode/2021/01/11/29bf78b1-94f2-6301-ec83-fd4541c438b7.png?sv=2019-12-12&st=2021-01-10T12%3A37%3A03Z&se=2031-01-11T12%3A37%3A03Z&sr=b&sp=r&sig=eyImi7f1xhlW0EtOsWQOS5NhsBgUv6fWlKtF03AqOXc%3D)

    Update! Since this post was written, there’s been some breaking changes to Azure App Services Run-From-Zip feature, this was fixed in Cake.Kudu.Client version 0.6.0 you can read more about that at the post below

[![Cake.Kudu Client version 0.6.0 released](https://cdn.devlead.se/clipimg-vscode/2021/01/11/3d72e5ff-aca6-965a-1989-d837ba7cce69.png?sv=2019-12-12&st=2021-01-10T12%3A35%3A54Z&se=2031-01-11T12%3A35%3A54Z&sr=b&sp=r&sig=bbuUUuci2RePI%2BEBfDKhyAvBNOV98aYSZ%2FBEqXNQ%2F1M%3D)](https://medium.com/@devlead/cake-kudu-client-version-0-6-0-released-cb6435629684)

A couple of days ago Azure announced that they in preview added a new way to do app services deployments called [Run-From-Zip](https://github.com/Azure/app-service-announcements/issues/84), which lets you deploy using a zip file.

Deploying using a zip file as been possible before, the difference with this new method is that the file isn’t extracted into the “wwwroot” directory, but instead the zip file *mounted* read only as “wwwroot”.

The zip file can either be hosted externally from the site or in a special folder on the app service itself, and the latter is now what the “Kudu Client” Cake addin now supports — enabling you to use this new method of deployment in your Cake build scripts.

## Prerequisites

To enable Run-From-Zip deployments you’ll first need to set an application setting called `WEBSITE_USE_ZIP`, you either set this to an url when deploying from an external source, or in this case just set it to `1`.

![App Service app settings](https://cdn.devlead.se/clipimg-vscode/2021/01/11/6ca61d0c-daf2-3ada-e681-a129eee866fe.png?sv=2019-12-12&st=2021-01-10T12%3A38%3A49Z&se=2031-01-11T12%3A38%3A49Z&sr=b&sp=r&sig=RHcKAy41C%2FrjCXhM5jfrZsIqlSi%2BEamUIZoLSp%2Bqjao%3D)

## ZipRunFromDirectory

Naming things is hard, but the addin now has a method called [ZipRunFromDirectory](https://cakebuild.net/api/Cake.Kudu.Client.Extensions/KuduClientZipExtensions/AA111BEB), which will do all the “heavy lifting” and deploy a local directory.

## Example usage

o deploying a site using this new method just becomes a couple of lines of code

```csharp
#addin nuget:?package=Cake.Kudu.Client&version=0.3.0

string  baseUri     = EnvironmentVariable("KUDU_CLIENT_BASEURI"),
        userName    = EnvironmentVariable("KUDU_CLIENT_USERNAME"),
        password    = EnvironmentVariable("KUDU_CLIENT_PASSWORD");

IKuduClient kuduClient = KuduClient(
    baseUri,
    userName,
    password);

DirectoryPath sourceDirectoryPath = "./Documentation/";

FilePath deployFilePath = kuduClient.ZipRunFromDirectory(sourceDirectoryPath);

Information("Deployed to {0}", deployFilePath);
```

The file path returned, is the zip file deployed to the app service.

## Behind the curtain

So what does actually happen here?
In a nutshell the method will:

1. In memory zip source directory
1. Push that zip to d:\home\data\SitePackages to a unique date stamped file name
1. Push the filename of the zip to d:\home\data\SitePackages\siteversion.txt
1. Via Kudu API call the site to ensure it’s up and right version deployed (a file called KuduClientZipRunFromDirectoryVersion.txt is included in deployed zip for this purpose)
1. Return the remote path of the deployed zip

![Kudu deploy zip files](https://cdn.devlead.se/clipimg-vscode/2021/01/11/996eea2c-912e-28db-5430-4a962037b645.png?sv=2019-12-12&st=2021-01-10T12%3A41%3A15Z&se=2031-01-11T12%3A41%3A15Z&sr=b&sp=r&sig=jQ0EAY8Joiny7%2BSJEP%2BwyjbR3kZRDBAsWBt%2FSVr5pzU%3D)

## Closing thoughts

I’ve tried this for a couple of days now, and have found deployments to be very stable and quick. But really need some more testing to see what the implications of running this in production, how existing apps behave in a read only mode, etc.

This is a shiny new feature still in preview, general guidance and tooling support isn’t quite there yet, but as you seen with this post the primitives are in place to build upon and refine the experience in the future!

[![Introducing Cake Kudu Client](https://cdn.devlead.se/clipimg-vscode/2021/01/11/d815ce8f-f979-6770-5abb-02d65e195cf3.png?sv=2019-12-12&st=2021-01-10T12%3A42%3A16Z&se=2031-01-11T12%3A42%3A16Z&sr=b&sp=r&sig=r4hBCgyhHLD%2FsHuT3RN49DXX4AeZczADKWtDnKCU5Vc%3D)](https://hackernoon.com/introducing-cake-kudu-client-abda40d15f38)
