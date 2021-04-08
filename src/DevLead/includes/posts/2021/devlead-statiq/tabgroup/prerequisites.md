First of all in your Statiq App you need to add the [Devlead.Statiq](https://www.nuget.org/packages/Devlead.Statiq) NuGet package to your Statiq web application.

Enabling `TabGroup` shortcode is done by invoking the `AddTabGroupShortCode` extension on your Statiq app [Bootstrapper](https://statiq.dev/framework/configuration/bootstrapper/) which will enable the shortcode and add the needed CSS file to your generation output.

```csharp
using System;
using Devlead.Statiq.Tabs;
using Statiq.App;
using Statiq.Web;

await Bootstrapper
    .Factory
    .CreateDefault(args)
    .AddWeb()
    .AddTabGroupShortCode()
    .RunAsync();
```

The CSS file will end up in `output/scss/TabGroup.css` and you'll need to reference it where you link in your other CSS custom files (i.e. `_head.cshtml`), but for your convenience, I've created a `TabGroupCss` shortcode you can use i.e. with this site my [_head.cshtml](https://github.com/devlead/devlead.se/blob/main/src/DevLead/input/_head.cshtml) looks like this

<?# IncludeCode "./_head.cshtml" lang="html" /?>
