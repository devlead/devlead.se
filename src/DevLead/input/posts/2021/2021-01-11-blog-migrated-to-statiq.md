---
title: Blog migrated to Statiq
tags:
    - .NET
    - C#
    - Announcement
    - Statiq
author: devlead
published: 2021-01-11
image: https://cdn.devlead.se/clipimg-vscode/2021/01/11/a20c196d-ae40-ed83-1a49-ec09f0e55e2e.png?sv=2019-12-12&st=2021-01-10T20%3A53%3A57Z&se=2031-01-11T20%3A53%3A57Z&sr=b&sp=r&sig=ZzDlx22k%2BmCJJkPeHnEcxZFZ5xbBoG4PQluscRG7%2BeM%3D
description: It was long overdue
---

Since 2016 I've been using Medium as my platform of choice, this is not a rage quit from the platform, I'll keep posting on Medium, the difference is that the main source for my posts will be on my own canonical domain, where I've got full access and control over my words.

The decision is just as much that I've found a stack and toolchain I really like, feel comfortable, and hopefully productive with.

The stack I've settled on has a few key components

* [Statiq Web](https://statiq.dev/) - a brilliant .NET based static site generator by [Dave Glick](https://daveaglick.com/)
* [GitHub](https://github.com/) - where the code is stored.
* [GitHub Pages](https://pages.github.com/) - where the html is served from.
* [Azure CDN](https://azure.microsoft.com/en-us/services/cdn/) - where the blog images are served from.
* [ClipImg](https://marketplace.visualstudio.com/items?itemName=gep13.clipimg-vscode) - Visual Studio Code addin that takes images from clipboard, uploads to Azure blob storage and inserts the markdown needed to render image - with just one keyboard shortcut.
