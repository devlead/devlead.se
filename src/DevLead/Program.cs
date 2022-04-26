using System;
using Devlead.Statiq.Code;
using Devlead.Statiq.Tabs;
using Devlead.Statiq.Themes;
using Statiq.App;
using Statiq.Common;
using Statiq.Web;

await Bootstrapper
    .Factory
    .CreateDefault(args)
    .AddThemeFromUri(new Uri("https://github.com/devlead/CleanBlog/archive/5eb1381346e550db6e1fbd4e268889dbc1dfcee.zip"))
    .AddWeb()
    .AddTabGroupShortCode()
    .AddIncludeCodeShortCode()
    .ConfigureSettings(settings =>
    {
        settings["GitHubBranch"] = "main";
    })
    .DeployToGitHubPages(
        "devlead",
        "devlead.github.io",
        Config.FromSetting<string>("DEVLEADSE_GITHUB_TOKEN")
    )
    .RunAsync();
