---
title: Deploying .NET Core to Azure using Bitrise and Cake
tags:
    - .NET
    - Azure
    - Bitrise
    - C#
    - Cake
    - DevOps
    - Docker
author: devlead
published: 2018-03-23
canonical: https://blog.bitrise.io/deploying-net-core-to-azure-using-bitrise-and-cake/
---

Originally published at [blog.bitrise.com](https://blog.bitrise.io/deploying-net-core-to-azure-using-bitrise-and-cake/).

Mattias Karlsson demonstrates how you can build and ship a .NET Core website using a custom docker build image and the open source build system Cake.

Guest blog post by Mattias Karlsson.

    Mattias Karlsson is a Microsoft Azure MVP and Open Source maintainer.

![Bitrise Cake](https://cdn.devlead.se/clipimg-vscode/2021/01/11/6ce723c3-edc9-8b41-6ed2-d474367040f5.png?sv=2019-12-12&st=2021-01-10T12%3A03%3A31Z&se=2031-01-11T12%3A03%3A31Z&sr=b&sp=r&sig=EMmpaNTM2%2Bhh3%2BVry4Tdumjs6aEMKLZwIKGQVE%2Bhwco%3D)

Bitrise is mostly known for providing hosted continuous integration and deployment targeting iOS and Android projects, but it’s much more capable than that and can build and ship basically anything you can build on MacOS or Linux.

## Adding a project to Bitrise

Adding a new project is straightforward, click the “Add new app” Button.

![Add New Bitrise app](https://cdn.devlead.se/clipimg-vscode/2021/01/11/6bffcbe9-e099-e882-c949-7578884ff356.png?sv=2019-12-12&st=2021-01-10T12%3A04%3A12Z&se=2031-01-11T12%3A04%3A12Z&sr=b&sp=r&sig=XsOZ8Dbh2%2F5ScmcaC5NIp2nFni6IJ5cl8%2B%2BTYfX8kKQ%3D)

Then pick your source code management provider of choice and pick your project.

![Bitrise new project UI](https://cdn.devlead.se/clipimg-vscode/2021/01/11/2d53378d-28c4-81c1-7455-65648e9e1156.png?sv=2019-12-12&st=2021-01-10T12%3A04%3A46Z&se=2031-01-11T12%3A04%3A46Z&sr=b&sp=r&sig=0BY%2BinxBgzqkiCooJgYL7ca%2FB7cZsqeRN7hsbKXx4YU%3D)

Let Bitrise setup SSH keys and validate the repository.

![Bitrise repo access configuration](https://cdn.devlead.se/clipimg-vscode/2021/01/11/77e1853c-dcd6-e0e7-7f9e-fc011611912e.png?sv=2019-12-12&st=2021-01-10T12%3A05%3A20Z&se=2031-01-11T12%3A05%3A20Z&sr=b&sp=r&sig=nX6475e%2BP%2FmYnKObAcQBmWs68HnZ2GkYRwsu5Anqv44%3D)

Once Bitrise is done validating the repository, pick manual and then `other / manual` and "Android & Docker on Ubuntu", this will let us pick our own custom docker image later.

![Bitrise Project build configuration](https://cdn.devlead.se/clipimg-vscode/2021/01/11/ef576575-dd01-9bd5-2323-1cbd02a295e2.png?sv=2019-12-12&st=2021-01-10T12%3A05%3A58Z&se=2031-01-11T12%3A05%3A58Z&sr=b&sp=r&sig=vLe20a998sciuAvXRTZOFtgVq6v4rWfjFqzY0S9hrA0%3D)

To get continuous builds let Bitrise setup a webhook for you:

![Bitrise webhook setup](https://cdn.devlead.se/clipimg-vscode/2021/01/11/69c69813-8114-8c85-91ed-00df387e6fd4.png?sv=2019-12-12&st=2021-01-10T12%3A06%3A19Z&se=2031-01-11T12%3A06%3A19Z&sr=b&sp=r&sig=WrGowtyfgngndjGfQDezBmt8nTNpuEPgNNnZirQ7yJI%3D)

Bitrise will then kick off a build, that will just clone the repository and let you know all is set up.

![Bitrise running build](https://cdn.devlead.se/clipimg-vscode/2021/01/11/f3d155a7-0577-67db-7e62-c96a47e3e68b.png?sv=2019-12-12&st=2021-01-10T12%3A06%3A42Z&se=2031-01-11T12%3A06%3A42Z&sr=b&sp=r&sig=y2MXfmSVQqX9JWqV53gqrGqkW5uINbTyKWA9oxMfO6k%3D)

## Setup a custom build docker image

You could download and bootstrap Cake as part of a build step, but Bitrise also lets you pick custom docker images from Docker Hub, which could be tailored with your tools pre-installed. I’ve created and published `cakebuild/cake:2.1-sdk-bitrise` on Docker Hub, which contains Bitrise CLI, .NET Core SDK 2.1 and Cake script runner.

To switch to custom Docker image, navigate to “Workflow” and click on “Stack”.

![Bitrise workflow UI](https://cdn.devlead.se/clipimg-vscode/2021/01/11/7abbd37e-e5c1-002a-2e74-e48957a39ed0.png?sv=2019-12-12&st=2021-01-10T12%3A07%3A30Z&se=2031-01-11T12%3A07%3A30Z&sr=b&sp=r&sig=n9pR68dRGlzeG25Gqiif%2FhxF8S%2Fdnl7pkR6v4PjdPjs%3D)

Then change Docker Image to use to your custom image (i.e. `cakebuild/cake:2.1-sdk-bitrise`, the only requirement is that Bitrise CLI and its dependencies need to be present).

## Configuring Cake

When picking the “manual” template you’ll get 4 build steps, which will handle authentication, fetching code, the “hello world” build script and also handle artifacts.

![Bitrise configuring Build work flow](https://cdn.devlead.se/clipimg-vscode/2021/01/11/53b6d29d-26d1-9b4b-bcf9-9df2579e0f69.png?sv=2019-12-12&st=2021-01-10T12%3A13%3A30Z&se=2031-01-11T12%3A13%3A30Z&sr=b&sp=r&sig=YtIPD%2Be5oIH6t7DC1o1G6U5WCleKFXeR3yfhYS4oaOc%3D)

As Cake is pre-loaded on the custom container image, all that’s needed to execute Cake build script is `Cake [path to cake script]`, if your build script is in the root and follows the `build.cake` convention, then all that's needed is `Cake`.

![Bitrise Cake build step](https://cdn.devlead.se/clipimg-vscode/2021/01/11/60b0c772-1718-2cfa-9d9e-6c7b1ae0124c.png?sv=2019-12-12&st=2021-01-10T12%3A15%3A08Z&se=2031-01-11T12%3A15%3A08Z&sr=b&sp=r&sig=N%2FlvKWLEw4KZeNrmwssdn5ZgRNYdzxJOhEn02mswtCA%3D)

## The Build script

The example [build.cake](https://github.com/azurevoodoo/DeployingToAzureUsingBitriseAndCake/blob/master/build.cake) used for this post has the following steps

1. Clean build output & Restore build dependencies
1. Build code
1. Run unit tests
1. Publish web
1. Deploy to Azure

Deploying to Azure is done using Azure App Service Kudu rest API using the [Cake.Kudu.Client](https://hackernoon.com/introducing-cake-kudu-client-abda40d15f38) Cake addin.

## Keeping secrets

To deploy to Azure with Cake.Kudu.Client, you’ll need the base URL for the site’s Kudu API, the username, and password. You can find this information by downloading your publishing profile from the Azure portal

![Azure Portal App Servuce overview blade](https://cdn.devlead.se/clipimg-vscode/2021/01/11/9246ea9b-4f1a-6d44-49bc-2259baf27317.png?sv=2019-12-12&st=2021-01-10T12%3A17%3A08Z&se=2031-01-11T12%3A17%3A08Z&sr=b&sp=r&sig=awELNBjLpVX389GaIU%2Flod3ecRMC6t4J06j%2B6US9IQ8%3D)

This is, of course, sensitive information that you don’t want to be in your repository or accessible from e.g. pull requests.

To cater for this need Bitrise provides “Secrets”, Secrets are exposed as environment variables, so they can easily be consumed from build steps / scripts. But to add an extra level of security they’re not part of your build definition and by default, they won’t be available from pull requests.

![Bitrise Secret Environment Variables](https://cdn.devlead.se/clipimg-vscode/2021/01/11/7c7e9f35-228b-5473-7d48-4d9ff08483da.png?sv=2019-12-12&st=2021-01-10T12%3A17%3A49Z&se=2031-01-11T12%3A17%3A49Z&sr=b&sp=r&sig=zt4xJyLiZCO5AC%2FwWXRxELQREb68NpgVQtSf7aajVxQ%3D)

## All set

Now hit Start a Build or push a commit and your website will continuously build and deploy.

![Bitrise Start/Schedule new build](https://cdn.devlead.se/clipimg-vscode/2021/01/11/1ce86013-621d-ba8f-3490-37902ab1b0dc.png?sv=2019-12-12&st=2021-01-10T12%3A20%3A20Z&se=2031-01-11T12%3A20%3A20Z&sr=b&sp=r&sig=aXM%2BQrIollFehwDDMS7dxEGY9e96s5x26HXe70jqMEs%3D)

![Cake build log](https://cdn.devlead.se/clipimg-vscode/2021/01/11/aeaed4b6-04dc-d99e-fe2a-bb8ffcc04057.png?sv=2019-12-12&st=2021-01-10T12%3A20%3A33Z&se=2031-01-11T12%3A20%3A33Z&sr=b&sp=r&sig=2JZDluWVvMzRVKOyjaX4IqQmWavSwfynDIzBBEARCUQ%3D)

## Resources

The example repository can be found on GitHub [here](https://github.com/azurevoodoo/DeployingToAzureUsingBitriseAndCake).

A GitHub Gist of the Bitrise build definition is available [here](https://gist.github.com/devlead/6086fe79b0a5e4ebbbf145d4244a9713).

If you haven’t but want to try Bitrise, then you can use [this referral link](https://www.bitrise.io/?referrer=8049ac73f66b5427) which benefits the [Cake](https://cakebuild.net/) project.
