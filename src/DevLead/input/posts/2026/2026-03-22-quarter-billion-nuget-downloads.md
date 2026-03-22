---
title: A quarter of a billion NuGet downloads
tags:
    - .NET
    - NuGet
    - OSS
    - Cake
author: devlead
description: A personal milestone and a short reflection on NuGet's history and what it has enabled for open source and the community
published: 2026-03-22
image: https://cdn.devlead.se/clipimg-vscode/2026/03/22/019d14ed-ff79-7c53-b9ed-40e27779409a.png?sv=2025-07-05&spr=https&st=2026-03-22T05%3A31%3A00Z&se=2036-03-23T09%3A31%3A00Z&sr=b&sp=r&sig=cL%2FseHDMzerFGyf5a0w0H2JNWWfTytZ5tOqcVCBrueY%3D
---

The total downloads of my packages on [NuGet.org](https://www.nuget.org/profiles/devlead) have crossed a quarter of a billion. It is a vanity number, but it got me thinking less about the count and more about the platform that made it possible. So here is a short look back at NuGet, the positive bits, and what it has enabled for open source and the community.

## NuGet in context

An essential tool for any modern development platform is a way for developers to create, share, and consume useful libraries. For .NET, that mechanism is [NuGet](https://www.nuget.org/policies/About). It started out as NuPack, was [renamed to NuGet](https://haacked.com/archive/2010/10/29/nupack-is-now-nuget.aspx/) in 2010, and became one of Microsoft's early forays into open source package management under the OuterCurve Foundation. Later, and still today, NuGet is part of the [.NET Foundation](https://dotnetfoundation.org/). The [NuGet Gallery](https://blog.davidebbo.com/2011/01/introducing-nuget-gallery.html) followed in 2011, and the rest is history.

In May 2016, [NuGet.org reached its first billion downloads](https://devblogs.microsoft.com/dotnet/the-1st-billion-1/). Today the gallery has passed 895 billion package downloads, over 11 million package versions, and over half a million unique packages. The ecosystem has grown enormously around that: .NET Framework, .NET Core and the unified .NET platform, better tooling, and a gallery that keeps improving. What has stayed constant is that the [NuGet Gallery is open source](https://www.nuget.org/policies/About), licensed under the Apache 2 License on GitHub. The NuGet team and a long list of community contributors have built and maintained the site that powers NuGet.org. That openness is something worth celebrating. 

## What NuGet has enabled

For open source, NuGet has become the place where .NET libraries and tools get discovered and reused. Maintainers can publish once and reach millions of developers. Projects like [Cake](https://cakebuild.net/) and countless libraries, tools, and templates are easily available because there is a single, trusted place to host and consume packages. That has made it far easier for teams and individuals to adopt .NET open source.

For the community, NuGet has evolved beyond the basics. Package signing, two-factor authentication, [NuGetAudit](https://devblogs.microsoft.com/dotnet/nugetaudit-2-0-elevating-security-and-trust-in-package-management/), and [Trusted Publishing](https://devblogs.microsoft.com/dotnet/enhanced-security-is-here-with-the-new-trust-publishing-on-nuget-org/) have raised security and trust. [Sponsorship on NuGet.org](https://devblogs.microsoft.com/dotnet/announcing-sponsorship-on-nugetdotorg-for-maintainer-appreciation/) gives users a way to support maintainers. The gallery itself has improved with better search, [Central Package Management](https://devblogs.microsoft.com/dotnet/introducing-central-package-management/) support, dark mode, and clearer package details. None of that happens without a platform that the team and community keep investing in. Development does not always move as fast as users would like, but that is the trade-off when you are also operating and maintaining a service that so many developers depend on every day.

For me, NuGet has been the channel for publishing and maintaining Cake-related packages and .NET tools such as [Cake.Tool](https://www.nuget.org/packages/Cake.Tool), [Cake.Git](https://www.nuget.org/packages/Cake.Git) and latest being [Cake.Sdk](https://www.nuget.org/packages/Cake.Sdk), along with several other tools and libraries like [LitJSON](https://www.nuget.org/packages/LitJson), [DPI](https://www.nuget.org/packages/DPI), [ARI](https://www.nuget.org/packages/ARI), [BRI](https://www.nuget.org/packages/BRI), [Blobify](https://www.nuget.org/packages/Blobify), and [UnpackDacPac](https://www.nuget.org/packages/UnpackDacPac), etc. It has also given me the chance to give back. The gallery is open for contributions, and I was able to add [Central Package Management and multiple package manager commands to the NuGet.org command palette](https://github.com/NuGet/NuGetGallery/pull/10277). That kind of improvement is possible because the gallery is open source and the team welcomes community pull requests.

## Thanks

Having reached a quarter of a billion downloads is not something one does alone. Thank you to everyone who uses these packages, to my projects co-maintainers, and to the NuGet team and everyone who contributes to the gallery and the ecosystem. NuGet has come a long way since 2010, and I am looking forward to seeing how it continues to improve and support maintainers and the .NET community.
