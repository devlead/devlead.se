---
title: Devlead.Statiq - Part 2 - Theme from external web resource
tags:
    - .NET
    - NuGet
    - C#
    - Statiq
    - Devlead.Statiq
author: devlead
description: An .NET assembly extending the static site generator Statiq with new core features
published: 2021-04-10
image: https://cdn.devlead.se/clipimg-vscode/2021/04/08/04e3408e-5295-3d37-0421-11c3ec60a691.png?sv=2019-12-12&st=2021-04-07T20%3A30%3A55Z&se=2031-04-08T20%3A30%3A55Z&sr=b&sp=r&sig=VI8xA0tYLfkNIsS%2FQx%2B9HDbDPU3n1TckpRJFflxg4sg%3D
---

Sometimes you just want things your way, in this second part of the three-part introductory blog post series about the NuGet package [Devlead.Statiq](https://www.nuget.org/packages/Devlead.Statiq), I'll go through how it can enable having your Statiq web theme reside as a common external web resource.

## The problem

Themes in Statiq are essentially a folder alongside your `input` folder. An example of such a theme is [statiqdev/CleanBlog](https://github.com/statiqdev/CleanBlog), the official recommended way to use it as a GIT sub-module like below

```bash
git submodule add https://github.com/statiqdev/CleanBlog.git theme
```

But I wanted a way to just clone and build without the hassle of sub-modules, but I also didn't want to just "copy-and-paste" the theme. I still wanted to with ease take the benefits of any updates to the upstream repository.

## The solution

My solution ended up to extend Statiq so I could just configure the theme to a specific external uri, pointing to a web resource containing a zip archive of that theme, and Statiq would then transparently download, extract and cache that theme as the site was generated. Changing the uri would invalidate the cache and fetch the theme again.

Fortunately, GitHub provides the functionality to serve any commit/tag/release as a zip archive using the `https://github.com/{owner}/{repo}/archive/{reference}` syntax, i.e. `https://github.com/statiqdev/CleanBlog/archive/ceb5055f3d0f7a330708494ed21eb469cde62ce2.zip`, making it really easy to obtain a specific immutable version of an upstream theme.

I then "gitignored" my `theme` folder - so it would never be added to the site's repository. I made sure any site-specific modifications only existed within my Statiq content `input` folder - ensuring new versions of the theme could be fetched without losing my tweaks to the theme.

## Example usage

First of all in your Statiq App you need to add the [Devlead.Statiq](https://www.nuget.org/packages/Devlead.Statiq) NuGet package to your Statiq web application.

Then theme from uri support in Statiq is enabled like below by invoking the `AddThemeFromUri` extension on the Statiq app [Bootstrapper](https://statiq.dev/framework/configuration/bootstrapper/), and as a parameter to it specifying the uri where the theme zip archive is located

```csharp
using System;
using Devlead.Statiq.Themes;
using Statiq.App;
using Statiq.Common;
using Statiq.Web;

await Bootstrapper
    .Factory
    .CreateDefault(args)
    .AddThemeFromUri(new Uri("https://github.com/statiqdev/CleanBlog/archive/ceb5055f3d0f7a330708494ed21eb469cde62ce2.zip"))
    .AddWeb()
    .RunAsync();

```

## Conclusion

IMHO this results in a fairly clean way to reuse themes between sites and makes it easy to contribute to a site using this method - all the setup a contributor needs to be able to test/preview the site clone the repository and run the project.

## Resources

- Statiq
  - [GitHub](https://github.com/statiqdev)
  - [Documentation](https://statiq.dev/)
- Devlead.Statiq
  - [GitHub](https://github.com/devlead/Devlead.Statiq)
  - [NuGet](https://www.nuget.org/packages/Devlead.Statiq/)
  - [Part 1 - Tabs](/posts/2021/2021-04-09-devlead-statiq-part1-tabs)
  - [Part 2 - Theme from external web resource](/posts/2021/2021-04-10-devlead-statiq-part2-theme-from-uri)
  - [Part 3 - IncludeCode 🤺](/posts/2021/2021-04-11-devlead-statiq-part3-includecode)
