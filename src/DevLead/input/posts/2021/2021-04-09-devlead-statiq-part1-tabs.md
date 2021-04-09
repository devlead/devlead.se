---
title: Devlead.Statiq - Part 1 - Tabs
tags:
    - .NET
    - NuGet
    - C#
    - Statiq
    - Devlead.Statiq
author: devlead
description: An .NET assembly extending the static site generator Statiq with new core features
published: 2021-04-09
image: https://cdn.devlead.se/clipimg-vscode/2021/04/05/390eeb4b-ad66-7406-860f-44ed05918873.png?sv=2019-12-12&st=2021-04-04T18%3A20%3A27Z&se=2031-04-05T18%3A20%3A27Z&sr=b&sp=r&sig=yLJRew2YKzfdLNObjcb3I6kkwfWiegYX%2FP68MwmoX3k%3D
---

Earlier this year I blogged about that my ["Blog migrated to Statiq"](/posts/2021/2021-01-11-blog-migrated-to-statiq), one advantage with [Statiq](https://statiq.dev/) is that it's through .NET code really customizable and lets you adapt it fully to your needs. Code that can be packaged and distributed as a NuGet package, making it straightforward to share and reuse functionality between sites.<br/> <br/> In a three-part blog post series, I'll start going through the features of the NuGet package [Devlead.Statiq](https://www.nuget.org/packages/Devlead.Statiq) created for my own Statiq based sites - but probably useful for others too, and this first part will be about the TabGroup Shortcode.

## TabGroup Shortcode

<?# TabGroup ?>
<?*
tabs:
  - name: Introduction
    content: |
      Statiq [shortcodes](https://statiq.dev/framework/content/shortcodes) are small but powerful macros that can generate content or add metadata to your documents.

      The `TabGroup` shortcode, is a CSS-only solution to simplify adding tabs in your Statiq input files.

      Why add tabs? Well with some content, a good example of that is code samples, tabs make it easier to group content together, keep things more focused and reduce user vertical scrolling.

      With the `TabGroup` shortcode tab content can be defined as either

      - Content - markdown defined directly in the shortcode content
      - Include - markdown fetched and processed from a external file
      - Code - fetch external file into markdown code fence

      the shortcode content is defined as `YAML`, you can within a single tab combine all variants (`content`, `include`, and `code`), and it'll render in the following order

      1. `content`
      1. `include`
      1. `code`

  - include: ./../includes/posts/2021/devlead-statiq/tabgroup/prerequisites.md

  - include: ./../includes/posts/2021/devlead-statiq/tabgroup/content.md

  - include: ./../includes/posts/2021/devlead-statiq/tabgroup/include.md

  - include: ./../includes/posts/2021/devlead-statiq/tabgroup/code.md

?>
<?#/ TabGroup ?>

## Conclusion

I'm really happy how flexible and versatile the `TabGroup` shortcode ended up being, while still keeping my markdown files nice and tidy, abstracting away the complexity.

## Resources

- Statiq - [GitHub](https://github.com/statiqdev) / [Documentation](https://statiq.dev/)
- Devlead.Statiq - [GitHub](https://github.com/devlead/Devlead.Statiq) / [NuGet](https://www.nuget.org/packages/Devlead.Statiq/) / [Part 2 - Theme from external web resource ](/posts/2021/2021-04-10-devlead-statiq-part2-theme-from-uri)
