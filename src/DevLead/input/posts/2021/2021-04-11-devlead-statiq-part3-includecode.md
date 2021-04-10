---
title: Devlead.Statiq - Part 3 - IncludeCode 🤺
tags:
    - .NET
    - NuGet
    - C#
    - Statiq
    - Devlead.Statiq
author: devlead
description: An .NET assembly extending the static site generator Statiq with new core features
published: 2021-04-11
image: https://cdn.devlead.se/clipimg-vscode/2021/04/08/985368ff-3b5e-5079-00b1-2d01c433f6b8.png?sv=2019-12-12&st=2021-04-07T21%3A28%3A53Z&se=2031-04-08T21%3A28%3A53Z&sr=b&sp=r&sig=yJSMzQzEuAd%2B1IyVh9Hsn%2FBBJE4iLet9Ymg6nOqFmCA%3D
---

So I'm lazy... when doing a blog post, or documentation I don't want to repeat my self, and a prime example of that is when documenting Statiq features I found my self to want to both display both the "source" and the "result" without having to type the same thing twice nor update in multiple places and that's why I created the `IncludeCode` Shortcode and in this last of the three-part introductory blog post series about the NuGet package [Devlead.Statiq](https://www.nuget.org/packages/Devlead.Statiq) I'll tell you all about it.

## IncludeCode Shortcode

Code include shortcode - A Statiq [shortcode](https://statiq.dev/framework/content/shortcodes) enables you fetch an external file into a markdown code fence block.

Statiq comes with a built in `Include` shortcode, which will just merge in the source of another document into the document utilizing the shortcode. This is really handy as it'll for example lets you edit content in one place, but you can use it in multiple places keeping things dry.

The issue and feature with `Include` is that it'll be executed as part of your Statiq App pipeline, any code in it will be rendered. Which might not be what you want for a code sample. So I created the `IncludeCode` shortcode which will process and include the external document within a markdown code fence.

### Example usage

<?# IncludeCode "./../includes/posts/2021/devlead-statiq/includecode/includecode.md" /?>

### Result

<?# Include "./../includes/posts/2021/devlead-statiq/includecode/includecode.md" /?>

## Conclusion

`IncludeCode` shortcode is a small addition, but it keeps my content DRY which I've learned to really appreciate as it not only reduces typing, but also reduces risk of inconsistencies and errors.

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