---
title: 🍰Bringing Cake to Microsoft Teams📣
tags:
    - C#
    - .NET
    - Microsoft Teams
    - DevOps
author: devlead
published: 2017-02-22
canonical: https://medium.com/hackernoon/bringing-cake-to-microsoft-teams-b49848981d5d
description: Sending notifications to Microsoft Teams from your Cake build scripts
---
![Microsoft Teams message from Cake](https://cdn.devlead.se/clipimg-vscode/2021/01/11/951390f9-0c34-4f66-bd22-ddb9e8ad6810.png?sv=2019-12-12&st=2021-01-10T14%3A35%3A12Z&se=2031-01-11T14%3A35%3A12Z&sr=b&sp=r&sig=aOrx%2FnFtHtbsqHd3SVonTweauFt3w%2FqAoJwNRQVq0R4%3D)

Even though Teams at Microsoft would appreciate you bringing them that’s not what this post is about, but notifications to the Microsoft Teams collaboration / team chat product.

![Cake logo](https://cdn.devlead.se/clipimg-vscode/2021/01/11/53899274-3404-8df9-4c59-85cb29b5a673.png?sv=2019-12-12&st=2021-01-10T13%3A16%3A42Z&se=2031-01-11T13%3A16%3A42Z&sr=b&sp=r&sig=1vvUx7y%2FZV2df%2FQdO04t6h6SE3eM9J%2B4OB2gSqQCyvY%3D)

And Cake? [Cake](https://cakebuild.net/) is an open source build system, which lets you in an unintrusive way with a C# DSL both cross platform and environment orchestrate everything around your build process — all using existing skills if you’re a .NET developer.
( *read more at [cakebuild.net](https://cakebuild.net/)* )

So that settled, wouldn’t it be great if you could send notifications from your Cake scripts to a Microsoft Teams channel? Guess what — there is!

## The Addin

Cake can be extended through addins, Cake addins can be any .NET assembly available as a NuGet package on any nuget feed, where nuget.org is the most well known package source. However you can by using a couple of attributes (*i.e. [CakeMethodAliasAttribute](http://cakebuild.net/api/Cake.Core.Annotations/CakeAliasAttribute/) and [CakeNamespaceImportAttribute](http://cakebuild.net/api/Cake.Core.Annotations/CakeNamespaceImportAttribute/)*) become a more “native” Cake addin, in the sense that they lets you extend the DSL and import namespaces using an [#addin](http://cakebuild.net/docs/fundamentals/preprocessor-directives) preprocessor directive.

I’ve created such an addin which makes it easy for you to communicate with a Microsoft Teams channel

```csharp
#addin nuget:?package=Cake.MicrosoftTeams

MicrosoftTeamsPostMessage("Hello from Cake!",
    new MicrosoftTeamsSettings {
        IncomingWebhookUrl = EnvironmentVariable("MicrosoftTeamsWebHook")
    });
```

The `#addin`directive fetches the assembly from nuget, references it, finds any Cake extension methods and imports namespaces — making them conveniently globally available for you in your Cake scripts.

## Setup / Teardown

Reporting when script started and when it succeeded / failed can be achieved by registering actions on the [Setup](http://cakebuild.net/api/Cake.Core.Scripting/IScriptHost/9CF7C412) and [Teardown](http://cakebuild.net/api/Cake.Core.Scripting/IScriptHost/E94DA692) methods on the Cake script host, Setup is executed before Cake tasks are executed and Teardown is always executed post task execution.

```csharp
var teamsSettings = new MicrosoftTeamsSettings {
        IncomingWebhookUrl = EnvironmentVariable("MicrosoftTeamsWebHook")
    };

Setup(context => {
    context.MicrosoftTeamsPostMessage("Starting build...", teamsSettings);
});

Teardown(context => {
    context.MicrosoftTeamsPostMessage(context.Successful ? "Build completed successfully."
        : string.Format("Build failed.\r\n({0})", context.ThrownException),
        teamsSettings);
})
```

As you can see the teardown [context](http://cakebuild.net/api/Cake.Core/ITeardownContext/) has an [Successful](http://cakebuild.net/api/Cake.Core/ITeardownContext/64379305) property indicating build success/failure and [ThrownException](http://cakebuild.net/api/Cake.Core/ITeardownContext/246CAB42) property containing exception on failure. A successful build would look something like this:

![Cake Microsoft Teams Build started & succeeded messages](https://cdn.devlead.se/clipimg-vscode/2021/01/11/ca72898d-0a3d-c95f-1fb3-d1ba523c0ece.png?sv=2019-12-12&st=2021-01-10T14%3A40%3A41Z&se=2031-01-11T14%3A40%3A41Z&sr=b&sp=r&sig=a8tBO3bj%2FA0O2%2Bvq1kkxFtR7yvPC789GDOp5FakHc9c%3D)

And a failed build would contain the exception / stack trace like this:

![Cake Microsoft Teams Build started and failed messages](https://cdn.devlead.se/clipimg-vscode/2021/01/11/a8eaec61-11b3-5391-f3ee-e578542926e4.png?sv=2019-12-12&st=2021-01-10T14%3A41%3A09Z&se=2031-01-11T14%3A41%3A09Z&sr=b&sp=r&sig=SKH42BXm7trle1GaZpthvQt3oCa9avN4fU0HcG0L3aE%3D)

## Task Setup / Teardown

If you want to track the progress of individual tasks that’s possible using the TaskSetup and TaskTeardown, which lets you register actions executed before and after task is executed.

```csharp
TaskSetup(context => {
    context.MicrosoftTeamsPostMessage(string.Format("Starting task {0}...", context.Task.Name),
        teamsSettings);
});

TaskTeardown(context => {
    context.MicrosoftTeamsPostMessage(string.Format("Task {0} {1} ({2}).",
        context.Task.Name,
        context.Skipped ? "was skipped" : "processed",
        context.Duration),
        teamsSettings);
});
```

Both [setup](http://cakebuild.net/api/Cake.Core/ITaskSetupContext/) and [teardown](http://cakebuild.net/api/Cake.Core/ITaskTeardownContext/) context provides a [Task](http://cakebuild.net/api/Cake.Core/ITaskTeardownContext/EDEBEFE1) property which gives you meta data about the task, the [teardown](http://cakebuild.net/api/Cake.Core/ITaskTeardownContext/) context also provides [Duration](http://cakebuild.net/api/Cake.Core/ITaskTeardownContext/D4FAFF2B) and [Skipped](http://cakebuild.net/api/Cake.Core/ITaskTeardownContext/685F941C) properties indicating if task was executed and how long it took to execute the task.

## Advanced formatting

So besides add in method MicrosoftTeamsPostMessage that just takes a string as message, there’s also a MicrosoftTeamsPostMessage overload that takes an MicrosoftTeamsMessageCard, this gives you more control of the message layout.

Posting a message for each step of the build can become very chatty, tailoring the message to instead neatly summarize the build in one message like below is probably more sustainable in the long run.

![Advanced formatted Teams Message from Cake](https://cdn.devlead.se/clipimg-vscode/2021/01/11/ca7f994c-7db2-677c-3adb-0abd43d0bd95.png?sv=2019-12-12&st=2021-01-10T14%3A44%3A19Z&se=2031-01-11T14%3A44%3A19Z&sr=b&sp=r&sig=aVZAPsffW22Uorfvl9vztQWSFNyAJgFVEzfeBDA5F5Q%3D)

Which is very similar to what Cake outputs to the console

![Cake task summary](https://cdn.devlead.se/clipimg-vscode/2021/01/11/7b304622-3969-f8fa-5d2f-bbe412b1927b.png?sv=2019-12-12&st=2021-01-10T14%3A44%3A36Z&se=2031-01-11T14%3A44%3A36Z&sr=b&sp=r&sig=pNP2m2hDVt9w00Il4U75v8960QfflruxeldnCfSCbDA%3D)

On failure you’ll get the icon clearly indicating failure and steps executed just as on success.

![Failed task Microsoft Teams Message from Cake](https://cdn.devlead.se/clipimg-vscode/2021/01/11/badfd621-7028-7158-ef22-2a5883041f0a.png?sv=2019-12-12&st=2021-01-10T14%3A44%3A58Z&se=2031-01-11T14%3A44%3A58Z&sr=b&sp=r&sig=IvJfu5GwiKfkHglQYSM%2FLm5hmzyz1jt9RwVqsY7MtRE%3D)

But you also get the full stacktrace from any error when expanding the message

![Full failed task Microsoft Teams message from Cake](https://cdn.devlead.se/clipimg-vscode/2021/01/11/873ec4f5-8015-abe7-e318-3ef60f6b6df0.png?sv=2019-12-12&st=2021-01-10T14%3A45%3A29Z&se=2031-01-11T14%3A45%3A29Z&sr=b&sp=r&sig=KAwvBX9dEOges7jNB8Twl9HsNLGTXOx%2FoGZVkL%2FKnd8%3D)

So what could the code for this look like? A complete example you can test below

```csharp
#addin nuget:?package=Cake.MicrosoftTeams
string projectName = "Example";
string microsoftTeamsWebHook = EnvironmentVariable("MicrosoftTeamsWebHook");
DateTime startDate = DateTime.UtcNow;

if (string.IsNullOrEmpty(microsoftTeamsWebHook))
{
    throw new Exception("MicrosoftTeamsWebHook environment variable not specified.");
}

var teamsSettings = new MicrosoftTeamsSettings { IncomingWebhookUrl = microsoftTeamsWebHook };
var facts = new List<MicrosoftTeamsMessageFacts>();

Setup(context => {
    facts.Add(new MicrosoftTeamsMessageFacts {
        name = "Setup",
        value = startDate.ToString("yyyy-MM-dd HH:mm:ss")
    });
});

TaskTeardown(context => {
    facts.Add(new MicrosoftTeamsMessageFacts {
        name = string.Concat(context.Task.Name),
        value = context.Skipped ? "skipped" : context.Duration.ToString("c")
    });
});

Teardown(context => {
    var tearDownDate = DateTime.UtcNow;
    facts.Insert(0,
        new MicrosoftTeamsMessageFacts {
            name = "Success",
            value = context.Successful ? "Yes" : "No"
        });

    facts.Add(new MicrosoftTeamsMessageFacts {
        name = "Teardown",
        value = tearDownDate.ToString("yyyy-MM-dd HH:mm:ss")
    });

    facts.Add(new MicrosoftTeamsMessageFacts {
        name = "Total Duration",
        value = (tearDownDate - startDate).ToString()
    });

    if (context.ThrownException!=null)
    {
        facts.Add(new MicrosoftTeamsMessageFacts {
            name = "Exception",
            value = context.ThrownException.ToString()
        });
    }

    var activityImage = context.Successful  ? "https://cloud.githubusercontent.com/assets/1647294/21986014/cf83fdb0-dbfd-11e6-8d18-617b0fd17597.png"
                                            : "https://cloud.githubusercontent.com/assets/1647294/21986029/ec3256a0-dbfd-11e6-9300-f183681cee85.png";

    var messageCard = new MicrosoftTeamsMessageCard {
                          summary = string.Format("{0} build {1}.", projectName, context.Successful ? "succeeded" : "failed"),
                          title = string.Format("Build {0}", projectName),
                          sections = new []{
                              new MicrosoftTeamsMessageSection{
                                  activityTitle = string.Format("{0} build {1}.", projectName, context.Successful ? "succeeded" : "failed"),
                                  activitySubtitle = string.Format("using Cake version {0} on {1}",
                                    Context.Environment.Runtime.CakeVersion,
                                    Context.Environment.Runtime.TargetFramework),
                                  activityText = "Build details",
                                  activityImage = activityImage,
                                  facts = facts.ToArray()
                              }
                          }
                        };

    Information("{0}", context.MicrosoftTeamsPostMessage(messageCard, teamsSettings));
});


Task("Restore")
    //Imagine actual work being done
    .Does(()=>System.Threading.Thread.Sleep(8000));

Task("Build")
    .IsDependentOn("Restore")
    //Imagine actual work being done
    .Does(()=>System.Threading.Thread.Sleep(6000));

Task("Test")
    .IsDependentOn("Build")
    //Imagine actual work being done
    .Does(()=>System.Threading.Thread.Sleep(15000));

Task("Package")
    .IsDependentOn("Test")
    //Imagine actual work being done
    .Does(()=>System.Threading.Thread.Sleep(5000));

Task("Publish")
    .IsDependentOn("Package")
    //Imagine actual work being done
    .Does(()=>System.Threading.Thread.Sleep(11000));

RunTarget("Publish");
```

The above code basically hooks up actions to [Setup](http://cakebuild.net/api/Cake.Core.Scripting/IScriptHost/9CF7C412), [Teardown](http://cakebuild.net/api/Cake.Core.Scripting/IScriptHost/E94DA692) and [TaskTeardown](http://cakebuild.net/api/Cake.Core.Scripting/IScriptHost/3BE7A7EB). Only collecting data before [Teardown](http://cakebuild.net/api/Cake.Core.Scripting/IScriptHost/E94DA692) is finally called. It’s just C# so you can go as crazy as you want, but above is a good starting point.

## Conclusion

Hopefully this post has shown you how to send notifications from your cake scripts and what the extension points Cake provides for this. This could obviously be reused for other messaging platforms and if you check the [addins](http://cakebuild.net/addins/) section on Cake’s website you’ll find addins for Slack, HipChat, Gitter, Twitter, etc.