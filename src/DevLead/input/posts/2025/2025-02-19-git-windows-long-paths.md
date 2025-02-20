---
title: Long paths in Git on Windows
tags:
    - Git
    - Windows
author: devlead
description: How to deal with long paths in Git on Windows
published: 2025-02-19
image: https://cdn.devlead.se/clipimg-vscode/2025/02/19/d6040277-0b3f-4d2d-96be-356c4b5e8625.png?sv=2023-01-03&st=2025-02-19T19%3A10%3A42Z&se=2035-01-19T19%3A10%3A00Z&sr=b&sp=r&sig=f7JFWYHcFzUGKxOb4ihKJe9rt2bP%2BbIm%2Bk5LfJaTH6E%3D
---

On Windows, it's not unlikely that you'll encounter issues where you either have a repo that won't clone or files that won't commit. One common scenario that causes this is when doing snapshot testing, particularly with parameterized tests. These tests often generate snapshot files with names that include the test parameters, resulting in very long filenames. 
One workaround is to move folders into the root of drives or create shorter names, but ultimately, this will cause issues sooner or later.

Fortunately, Windows can handle long files, but it's opt-in for legacy and compatibility reasons.

## Enabling Long Paths in Git

To enable long paths in Git, you can set the following system configuration:

```bash
git config --system core.longpaths true
```

## Enabling Long Paths in Visual Studio

For Visual Studio, you need to enable long paths by setting a Windows Registry Key. You can do this using PowerShell with the following command:

```PowerShell
New-ItemProperty `
    -Path "HKLM:\SYSTEM\CurrentControlSet\Control\FileSystem" `
    -Name "LongPathsEnabled" `
    -Value 1 `
    -PropertyType DWORD `
    -Force
```

By following these steps, you can avoid the common pitfalls associated with long file and directory names in your Git repositories on Windows.

### Disclaimer

This isn't a silver bullet. While enabling long paths in Windows and Git helps in most scenarios, it's not a complete solution for all situations. Some workloads in Visual Studio can still encounter issues, particularly when paths are passed to other processes like legacy .NET Framework-based tools. If this is your process, you might want to consider using the [Pri.LongPath NuGet package](https://www.nuget.org/packages/Pri.LongPath), which provides drop-in replacements for System.IO APIs that handle long paths correctly.

