---
title: Polyfill with PowerShell
tags:
    - .NET
    - Clean Code
    - DevOps
    - Legacy
    - Powershell
author: devlead
published: 2017-08-20
canonical: https://medium.com/hackernoon/polyfill-with-powershell-ad70c6cd7703
description: Spackle missing pieces on legacy versions of PowerShell
---

When writing scripts, targeting multiple runtime versions can be really painful, scripts can be forked in different files or contain hairy conditional statements to handle differences/missing between versions of PowerShell runtime/modules, resulting in unreadable and unmaintainable spaghetti code.

What if you instead detect missing commands and supply an implementation for those when needed? This would allow you to have your scripts look the same and basically be agnostic to the which runtime it’s running on, making code more concise and easier to maintain.

The technique for this is called polyfilling and is common practise in web development where things can differ between browser versions and vendors.

It turns out it’s fairly simple to do in PowerShell, I’ll demonstrate this using the [Expand-Archive](https://docs.microsoft.com/en-us/powershell/module/microsoft.powershell.archive/expand-archive?view=powershell-5.0) command that was introduced in PowerShell 5, which extracts files from a specified archive

```powershell
Expand-Archive
      -DestinationPath] <String>
      -LiteralPath <String>
```

It’ll obviously fail miserably on previous versions of PowerShell

![Expand-Archive failes on previous versions of PowerShell](https://cdn.devlead.se/clipimg-vscode/2021/01/11/6b6ac33f-a389-ab22-e0e7-533d069770ce.png?sv=2019-12-12&st=2021-01-10T12%3A57%3A25Z&se=2031-01-11T12%3A57%3A25Z&sr=b&sp=r&sig=mbq3sabMzSdHFGGntTGEUJ3Ya%2BaalzKv3zX2U2svLXA%3D)

As PowerShell has the full power of .NET we can utilize the ZipFile class that has been provided since .NET 4.5 (*for even older versions of PowerShell we could also potentially just shell out to 7zip or similar command line utility or utilize 3rd party .NET assembly, as we can do anything in our polyfill that’s possible PowerShell*).
Such a polyfill could look something like below:

```powershell
if (-not (Get-Command Expand-Archive -ErrorAction SilentlyContinue))
{
  & {
    Add-Type -AssemblyName System.IO.Compression.FileSystem
    function global:Expand-Archive
    {
        param([string]$Path, [string]$DestinationPath)
        [System.IO.Compression.ZipFile]::ExtractToDirectory($Path, $DestinationPath)
    }
  }
}
```

The above script checks if command exists, if it doesn’t it’ll execute a code block which will create a globally available function, with the parameters and functionality we expect. To use it in our scripts we dot source it into our script or shell
`. .\Expand-Archive.Polyfill.ps1`

![Demonstraing Expand-Archive available](https://cdn.devlead.se/clipimg-vscode/2021/01/11/32e374ff-3b83-e9eb-a0a7-39cac1d37cca.png?sv=2019-12-12&st=2021-01-10T12%3A59%3A00Z&se=2031-01-11T12%3A59%3A00Z&sr=b&sp=r&sig=ZPFzNZQHdnUgbLCeJAf9pfwFGg1N7VdOWHzO9A%2F0Xqs%3D)

and it’ll be available for us to utilize just like we would have done in a newer version of PowerShell.

![Demonstrating usage of the polyfilled command](https://cdn.devlead.se/clipimg-vscode/2021/01/11/02fd4235-bed7-d9ea-299b-4cbb38ec58b8.png?sv=2019-12-12&st=2021-01-10T12%3A59%3A43Z&se=2031-01-11T12%3A59%3A43Z&sr=b&sp=r&sig=DMrIzL%2FATJBZdm7cb6kkCKU7DZrKbTGJzTAycjgKO0k%3D)

## Conclusion

Polyfilling is a very neat way having more homogenous and maintainable scripts, while also letting you adopt new commands and features, even if you have one or two old servers in the closet ;)
