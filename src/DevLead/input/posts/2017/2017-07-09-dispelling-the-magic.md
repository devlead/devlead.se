---
title: Dispelling the magic!
tags:
    - .NET
    - C#
    - Cake
    - DevOps
    - F#
    - Visual Basic
author: devlead
published: 2017-07-09
canonical: https://medium.com/hackernoon/dispelling-the-magic-6dc0fdfe476c
description: The layers and pieces of Cake
---
![build.cake -> magic - Build success/fail](https://cdn.devlead.se/clipimg-vscode/2021/01/11/9a556e23-9df5-b80b-edce-8103ba73bcea.png?sv=2019-12-12&st=2021-01-10T13%3A15%3A19Z&se=2031-01-11T13%3A15%3A19Z&sr=b&sp=r&sig=bY%2FrcHwH0ULHLyo0Gh00lXcpdE3XXE1S5ZF84AQ7B08%3D)

When you don’t know the details behind a technology, it’s easy to dismiss it as magic, but if you dare to look behind the curtain — it might not be that complicated or foreign to what you already know.

![Cake logo](https://cdn.devlead.se/clipimg-vscode/2021/01/11/53899274-3404-8df9-4c59-85cb29b5a673.png?sv=2019-12-12&st=2021-01-10T13%3A16%3A42Z&se=2031-01-11T13%3A16%3A42Z&sr=b&sp=r&sig=1vvUx7y%2FZV2df%2FQdO04t6h6SE3eM9J%2B4OB2gSqQCyvY%3D)

In this blog post, I’ll go through some of the pieces that make up the open source build orchestration tool [Cake](http://cakebuild.net/).

I’ll show you how you can reuse pieces of Cake with different environments and even languages, I’ll go into detail on some parts and glance over others.

If there’s any part you would want me to go into more detail with please let me now! I will provide links to the examples and resources at the end of this blog post.

## “It’s just C# and .Net”

This is a statement I have often made as Cake scripts provide a superset of C# — which means anything you can do in C# you can do Cake. The Cake DSL via its aliases merely provide a shortcut to make APIs and tools easier to consume, but you can always opt to like “regular” code reference assemblies, write methods and classes.

An example of this could be working with JSON, in your console or MVC app it’s likely that you would use JSON.Net to serialize/deserialize your JSON from/to .NET objects and doing the same with a Cake script doesn’t differ much from plain vanilla C#

```csharp
#addin "Newtonsoft.Json"
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class MyDto
{
    public string Name { get; set; }
}

var dto = new MyDto {
    Name = "John Doe"
};

var json = JsonConvert.SerializeObject(dto, Formatting.Indented);

var dto2 = JsonConvert.DeserializeObject<MyDto>(json);
```

Basically, only difference is the `#addin` preprocessor, which fetches and references an assembly from NuGet and the lack of need to wrap in the code in a namespace/class/method.

Being “just” C# and .NET truly means that you’ll always have the safety net of .NET, C# and the vast number of resources available for those pieces of technology.

## The pieces of Cake

Cake is distributed in two flavors, one for the full classic .NET which also works with Mono and a version for .NET Core — the new cross platform framework/runtime from Microsoft.

The most common way to obtain the Cake tool is via the `Cake` and `Cake.CoreCLR`NuGet packages (Cake is also available via Chocolatey, Homebrew, GitHub and official docker containers are on their way).

![.NET Full/Desktop vs .NET Core](https://cdn.devlead.se/clipimg-vscode/2021/01/11/46ed3f51-6cba-3f11-8cb7-04f729fb5877.png?sv=2019-12-12&st=2021-01-10T13%3A25%3A17Z&se=2031-01-11T13%3A25%3A17Z&sr=b&sp=r&sig=QIFKVh4SEpjHISyoO%2F9eMwFiUARNHuHCXB0lfjk9x5w%3D)

The NuGet packages contain all dependencies needed to execute under respective runtime, but the “Magic” is in three assemblies and a console application.

## Cake.exe / Cake.dll — “The Tool”

To name a few things it handles

* Compilation of scripts
* Argument parsing
* Console logging
* Composition of modules/assemblies
* Basically, it’s what glues everything together and provides the “Cake experience”

## Cake.Core — “The heart”

Provides things like

* Core abstractions/interfaces/attributes used by Cake Tool/Addins/Modules (*this is the only assembly needed to reference when creating a Cake addin/module*)
* DSL Parsing and transpiling/codegen to C# that Roslyn understands and can compile
* Base tool implementation (*tool resolution, execution, settings etc.*)
* Cake Task runtime (*task runner/dependency graph/setup & teardown etc.*)

## Cake.Common — “The doer”

This assembly provides almost 400 convenient aliases/extension methods for things like

* Tools (MSBuild, NuGet, .NET Core CLI, code signing, etc.)
* APIs (compression, project file parsing, text templating, HTTP, etc.)
* Build system support (AppVeyor, VSTS, Jenkins, TeamCity, MyGet, etc.)

## Cake.NuGet — “The Fetcher”

Cake module that enables fetching dependencies from NuGet for i.e. preprocessor directives like `#addin`, `#tool` and `#load`.

## Cake composition — “It’s just C# and .Net part deux”

The assemblies `Cake.Core`, `Cake.Common` and `Cake.NuGet` are all available up on NuGet.org targeting both .NET Full/Desktop and .NET Core.

This means you can reference and leverage parts/most of the work and effort that’s been put into Cake with almost any .NET application, environment or platform — it’s just standard C# code and assemblies!

That said Cake relies heavily on dependency injection and has over time been refactored into custom modules — so how it all fits together can be a bit daunting for someone that’s new to the project. A rough basic example of doing this using an Inversion of Control container like AutoFac could look something like this:

```csharp
var builder = new ContainerBuilder();
// Core services.
builder.RegisterType<CakeEngine>().As<ICakeEngine>().SingleInstance();
builder.RegisterType<FileSystem>().As<IFileSystem>().SingleInstance();
builder.RegisterType<CakeEnvironment>().As<ICakeEnvironment>().SingleInstance();
builder.RegisterType<Globber>().As<IGlobber>().SingleInstance();
builder.RegisterType<ProcessRunner>().As<IProcessRunner>().SingleInstance();
builder.RegisterType<WindowsRegistry>().As<IRegistry>().SingleInstance();
builder.RegisterType<ToolLocator>().As<IToolLocator>().SingleInstance();
builder.RegisterType<ToolResolutionStrategy>().As<IToolResolutionStrategy>().SingleInstance();
builder.RegisterType<ToolRepository>().As<IToolRepository>().SingleInstance();
builder.RegisterType<DefaultExecutionStrategy>().As<IExecutionStrategy>().SingleInstance();
builder.RegisterType<CakePlatform>().As<ICakePlatform>().SingleInstance();
builder.RegisterType<CakeRuntime>().As<ICakeRuntime>().SingleInstance();
builder.RegisterType<CakeContext>().As<ICakeContext>().SingleInstance();

// Script Host Services - these are currently part of Cake.exe/dll and
// not publicly available and need to be provided by any custom host
builder.RegisterType<CustomArguments>().As<ICakeArguments>();
builder.RegisterType<CustomConfiguration>().As<ICakeConfiguration>();
builder.RegisterType<CustomLog>().As<ICakeLog>().SingleInstance();
builder.RegisterType<CustomReportPrinter>().As<ICakeReportPrinter>().SingleInstance();
builder.RegisterType<CustomScriptHost>().As<IScriptHost>().SingleInstance();

// Script Host
IScriptHost ScriptHost = builder.Build().Resolve<IScriptHost>();
```

As the observant might see from the comment there’s a few cases where the implementation currently resides in `Cake.exe/Cake.dll` (*this might be something we’ll look at refactoring in the future*), the interfaces they implement exist in Core so you can implement and provide your own implementation or depending on what parts of Cake you reuse you might not need them (*for unit testing we provide Cake.Testing which provides fake context and file system, environment abstractions for tool testing, etc. Unit testing Cake addins / modules might be a good topic for another blog post — please let me know if you reckon that’s the case*).

## Proof of concept custom script host

So, to do your own custom host for your own “build script” implementation, you currently need to implement a few interfaces: `ICakeArguments`, `ICakeConfiguration`, `ICakeLog`, `ICakeReportPrinter` and `IScriptHost`, because these implementations as mentioned earlier currently resides in `Cake.exe/Cake`.dll, but all other are available in `Cake.Core` ready for reuse in any .NET project.

To demonstrate this, I’ve created the *“Proof of concept, in no way official, don’t use in production, just to see how the sausage is made, etc.”* assembly called `Cake.Bridge`, compiled for both .NET and .NET Core, which means it could be used most places .NET is available today (*binary is up on NuGet and source on GitHub, I’ll provide links to all resources at end of this post*).

`Cake.Bridge` provides a static `CakeBridge`class which provides easy access to working with the Cake task runner and the `ICakeContext` (*which is what all Cake aliases/methods extend*), created with the goal to demonstrate an easy way to reuse Cake assemblies from any .NET language and not only with C# which is what Cake supports out of the box.

What you won’t get is anything the Cake DSL provides and no addin nor module support.

To illustrate this I’ve authored a few code snippets on how using Cake from a few different .NET languages using the `Cake.Bridge` assembly, ***disclaimer*** the code snippets are quick and rough proof of concepts to mostly prove it can be done and obviously more refinement needs to be done for them to be more idiomatically correct.

There often exist more native, prominent and widely used build systems already.

## PowerShell

Even if perhaps mainly the sysadmin language of choice — PowerShell still is a .NET based language and its ability to utilize .NET makes it a very powerful scripting language as anything not provided native by the language often can be solved by reaching out to the framework it’s running on. It also means that you can use Cake from it:

```powershell
[string] $cakeBootstrapper      = './v0.0.5-alpha-cake.ps1'
[string] $cakeBootstrapperUrl   = 'https://raw.githubusercontent.com/devlead/Cake.Bridge/v0.0.5-alpha/src/cake.ps1'
if (!(Test-Path $cakeBootstrapper))
{
    Invoke-RestMethod $cakeBootstrapperUrl -OutFile $cakeBootstrapper
}
. $cakeBootstrapper

######################################################################
## GLOBALS
######################################################################
[FilePath]      $solution      = [Enumerable]::FirstOrDefault([GlobbingAliases]::GetFiles($context, "./src/*.sln"))
[string]        $configuration = "Release"
[DirectoryPath] $nugetRoot     = [DirectoryAliases]::MakeAbsolute($context, "./nuget");

######################################################################
## SETUP / TEARDOWN
######################################################################
Setup([Action[ICakeContext]]{
    param([ICakeContext] $ctx)
})

Teardown([Action[ITeardownContext]]{
    param([ITeardownContext] $ctx)
})

######################################################################
## TASKS
######################################################################
$cleanTask      = "Clean" |`
                    Task |`
                    Does -Action ({
                        [DirectoryAliases]::CleanDirectories($context, "./src/**/bin/$configuration")
                        [DirectoryAliases]::CleanDirectories($context, "./src/**/obj/$configuration")
                        [DirectoryAliases]::CleanDirectory($context, $nugetRoot)
                    })

$restoreTask    = "Restore" |`
                    Task |`
                    IsDependentOn -Dependency $cleanTask |`
                    Does -Action ({
                        [DotNetCoreAliases]::DotNetCoreRestore($context, $solution.FullPath)
                    })

$buildTask      = "Build" |`
                    Task |`
                    IsDependentOn -Dependency $restoreTask |`
                    Does -Action ({
                        [DotNetCoreAliases]::DotNetCoreBuild($context, $solution.FullPath)
                    })

$packTask       = "Pack" |`
                    Task |`
                    IsDependentOn -Dependency $buildTask |`
                    Does -Action ({
                        [DotNetCorePackSettings]   $packSettings = [DotNetCorePackSettings]::new()
                        $packSettings.OutputDirectory = $nugetRoot

                        [DotNetCoreAliases]::DotNetCorePack(
                            $context,
                            $solution.FullPath,
                            $packSettings
                        )
                    })

######################################################################
## EXECUTION
######################################################################
$packTask | RunTarget
```

## Visual Basic

You can’t talk about .NET languages without mentioning Visual Basic and now when it’s also joining the .NET Core party too it’s getting up to date with the times. And obviously you can **Bake** with **Cake** using some Basic (*there’s no VB.NET interactive console that I know of so I created a simple VB.NET Core console app*).

```vb
Imports System
Imports System.Linq
Imports CakeBridge
Imports Cake.Core
Imports Cake.Core.Diagnostics
Imports Cake.Core.IO
Imports Cake.Common
Imports Cake.Common.IO
Imports Cake.Common.Diagnostics
Imports Cake.Common.Tools.DotNetCore
Imports Cake.Common.Tools.DotNetCore.Build
Imports Cake.Common.Tools.DotNetCore.Pack
Imports Cake.Common.Tools.DotNetCore.Restore
Imports Cake.Common.Tools.DotNetCore.Test

Module Program
    Sub Main()
        '//////////////////////////////////////////////////////////////////////
        '// ARGUMENTS
        '//////////////////////////////////////////////////////////////////////
        Dim target          = Context.Argument("target", "Default"),
            configuration   = Context.Argument("configuration", "Release")

        '//////////////////////////////////////////////////////////////////////
        '// GLOBALS
        '//////////////////////////////////////////////////////////////////////
        Dim nugetRoot       As DirectoryPath    = Nothing,
            solution        As FilePath         = Nothing,
            solutionDir     As DirectoryPath    = Nothing,
            semVersion      As String           = Nothing,
            assemblyVersion As String           = Nothing,
            fileVersion     As String           = Nothing

        '//////////////////////////////////////////////////////////////////////
        '// SETUP / TEARDOWN
        '//////////////////////////////////////////////////////////////////////
        Setup(
            Sub(ctx As ICakeContext)
                ctx.Information("Setting up...")

                solution = ctx.GetFiles("./src/*.sln").Select(Function(file as FilePath) ctx.MakeAbsolute(file)).FirstOrDefault()

                If solution Is Nothing Then
                    Throw New Exception("Failed to find solution")
                End If

                solutionDir = solution.GetDirectory()
                nugetRoot = ctx.MakeAbsolute(ctx.Directory("./nuget"))

                Dim releaseNotes    = ctx.ParseReleaseNotes("./ReleaseNotes.md")
                assemblyVersion     = releaseNotes.Version.ToString()
                fileVersion         = assemblyVersion
                semVersion          = $"{assemblyVersion}-alpha"

                ctx.Information("Executing build {0}...", semVersion)
            End Sub
        )

        Teardown(
            Sub(ctx As ITeardownContext) ctx.Information("Tearing down...")
        )

        '//////////////////////////////////////////////////////////////////////
        '// TASKS
        '//////////////////////////////////////////////////////////////////////
        Dim cleanTask = Task("Clean").Does(
            Sub()
                Context.CleanDirectories($"{solutionDir.FullPath}/**/bin/{configuration}")
                Context.CleanDirectories($"{solutionDir.FullPath}/**/obj/{configuration}")
                Context.CleanDirectory(nugetRoot)
            End Sub
            )

        Dim restoreTask = Task("Restore").Does(
            Sub() Context.DotNetCoreRestore(solution.FullPath,
                                  New DotNetCoreRestoreSettings With {
                                  .Sources = {"https://api.nuget.org/v3/index.json"}
                                  })
            ).IsDependentOn(cleanTask)

        Dim buildTask = Task("Build").Does(
            Sub() Context.DotNetCoreBuild(solution.FullPath,
                                  New DotNetCoreBuildSettings With {
                                  .Configuration = configuration,
                                  .ArgumentCustomization = Function(args) args.Append(
                                                                                "/p:Version={0}", semVersion
                                                                            ).Append(
                                                                                "/p:AssemblyVersion={0}", assemblyVersion
                                                                            ).Append(
                                                                                "/p:FileVersion={0}", fileVersion
                                                                            )
                                  })
            ).IsDependentOn(restoreTask)

        Dim testTask = Task("Test").Does(
            Sub()
                For Each project In Context.GetFiles("./src/**/*.Tests.vbproj")
                    Context.DotNetCoreTest(project.FullPath,
                                    New DotNetCoreTestSettings With {
                                        .Configuration = configuration,
                                        .NoBuild = True
                                        })
                Next
            End Sub
            ).IsDependentOn(buildTask)

        Dim packTask = Task("Pack").Does(
            Sub()
                For Each project In (Context.GetFiles("./src/**/*.vbproj") - Context.GetFiles("./src/**/*.Tests.vbproj"))
                    Context.DotNetCorePack(project.FullPath,
                                    New DotNetCorePackSettings With {
                                        .Configuration = configuration,
                                        .OutputDirectory = nugetRoot,
                                        .NoBuild = True,
                                        .ArgumentCustomization = Function(args) args.Append(
                                                                                        "/p:Version={0}", semVersion
                                                                                    ).Append(
                                                                                        "/p:AssemblyVersion={0}", assemblyVersion
                                                                                    ).Append(
                                                                                        "/p:FileVersion={0}", fileVersion
                                                                                    )
                                        })
                Next
            End Sub
            ).IsDependentOn(testTask)

        Task("Default").IsDependentOn(packTask)

        '//////////////////////////////////////////////////////////////////////
        '// EXECUTION
        '//////////////////////////////////////////////////////////////////////
        RunTarget(target)
    End Sub
End Module¨
```

## F#

F# is a .NET language which makes it fully possible to use Cake with F#.
Full disclaimer though, as I’m not very proficient with the F# language and wanted something that somewhat looked like F# I called on the help and assistance of my friend [Mårten Rånge](https://github.com/mrange) to do a quick port from C# to F# and this was the result:

```fsharp
#load "tools/Cake.Bridge.0.0.4-alpha/content/cake.fsx"

//////////////////////////////////////////////////////////////////////
// NAMESPACE IMPORTS
//////////////////////////////////////////////////////////////////////
open Cake.Common
open Cake.Common.Diagnostics
open Cake.Common.IO
open Cake.Common.Tools.DotNetCore
open Cake.Common.Tools.DotNetCore.Build
open Cake.Common.Tools.DotNetCore.Pack
open Cake.Core
open Cake.Core.IO
open System

open CakeAdapter.CakeModule

// Execute script with: fsi build.fsx

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////
let target        = context.Argument("target", "Default")
let configuration = context.Argument("configuration", "Release")

//////////////////////////////////////////////////////////////////////
// GLOBALS
//////////////////////////////////////////////////////////////////////
let directoryPath           = !> (context.Directory "./nuget")
let nugetRoot               = context.MakeAbsolute directoryPath

type ProjectInfo =
  {
    AssemblyVersion : string
    FileVersion     : string
    SemVersion      : string
    Solution        : FilePath
  }

let mutable projectInfo     = None

let argumentCustomizer      = Func<ProcessArgumentBuilder,ProcessArgumentBuilder> (fun args ->
                                let p = projectInfo.Value
                                args
                                  .Append("/p:Version={0}"        , p.SemVersion      )
                                  .Append("/p:AssemblyVersion={0}", p.AssemblyVersion )
                                  .Append("/p:FileVersion={0}"    , p.FileVersion     ))

//////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
//////////////////////////////////////////////////////////////////////
setup (fun context ->
    context.Information "Setting up..."

    let solution        =
      match context.GetFiles "./src/*.sln" |> Seq.tryHead with
      | Some solution -> solution
      | None          -> failwith "Failed to find solution"

    let releaseNotes    = context.ParseReleaseNotes(!> "./ReleaseNotes.md")
    let assemblyVersion = string releaseNotes.Version
    let fileVersion     = assemblyVersion
    let semVersion      = assemblyVersion + "-alpha"

    projectInfo <- Some {
        AssemblyVersion = assemblyVersion
        FileVersion     = fileVersion
        SemVersion      = semVersion
        Solution        = solution
      }

    context.Information("Executing build {0}...", semVersion)
  )

tearDown (fun context ->
    context.Information "Tearing down..."
  )
//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////
let clean =
  task "Clean"
  |> does (fun () ->
    context.CleanDirectories("./src/**/bin/" + configuration)
    context.CleanDirectories("./src/**/obj/" + configuration)
    context.CleanDirectory nugetRoot
  )

let restore =
  task "Restore"
  |> isDependentOn clean
  |> does (fun () ->
    context.DotNetCoreRestore projectInfo.Value.Solution.FullPath
  )

let build =
  task "Build"
  |> isDependentOn restore
  |> does (fun () ->
    context.DotNetCoreBuild(
      projectInfo.Value.Solution.FullPath         ,
      DotNetCoreBuildSettings(
        Configuration         = configuration     ,
        ArgumentCustomization = argumentCustomizer))
  )

let pack =
  task "Pack"
  |> isDependentOn build
  |> does (fun () ->
    if context.DirectoryExists nugetRoot |> not then context.CreateDirectory nugetRoot

    let projectFiles =
      context.GetFiles "./src/**/*.fsproj"
      |> Seq.filter (fun file -> file.FullPath.EndsWith "Tests" |> not)
      |> Seq.toArray

    for project in projectFiles do
        context.DotNetCorePack(
          project.FullPath                            ,
          DotNetCorePackSettings(
            Configuration         = configuration     ,
            OutputDirectory       = nugetRoot         ,
            NoBuild               = true              ,
            ArgumentCustomization = argumentCustomizer))
  )

task "Default"
  |> isDependentOn pack

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////
runTarget target
```

## Cake / C#

For reference below is the script that was used for inspiration when porting to other languages/runtimes, there’s a little less bootstrapping involved but besides that they’re very similar:

```csharp
//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////
var target = Argument<string>("target", "Default");
var configuration = Argument<string>("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// GLOBALS
//////////////////////////////////////////////////////////////////////
DirectoryPath nugetRoot = MakeAbsolute(Directory("./nuget"));
FilePath solution = null;
string  semVersion,
        assemblyVersion,
        fileVersion;

//////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
//////////////////////////////////////////////////////////////////////

Setup(context =>
{
    Information("Setting up...");
    solution = GetFiles("./src/*.sln")
                .FirstOrDefault();
    if (solution == null)
        throw new Exception("Failed to find solution");


    var releaseNotes = ParseReleaseNotes("./ReleaseNotes.md");
    assemblyVersion =releaseNotes.Version.ToString();
    fileVersion = assemblyVersion;
    semVersion = assemblyVersion + "-alpha";

    Information("Executing build {0}...", semVersion);
});

Teardown(context =>
{
    Information("Tearing down...");
});

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

var clean = Task("Clean")
    .Does(() =>
    {
        CleanDirectories("./src/**/bin/" + configuration);
        CleanDirectories("./src/**/obj/" + configuration);
        CleanDirectory(nugetRoot);
    });

var restore = Task("Restore")
    .IsDependentOn(clean)
    .Does(() =>
    {
        DotNetCoreRestore(solution.FullPath);
    });

var build = Task("Build")
    .IsDependentOn(restore)
    .Does(() =>
    {
        DotNetCoreBuild(solution.FullPath, new DotNetCoreBuildSettings {
            Configuration = configuration,
            ArgumentCustomization = args => args
                .Append("/p:Version={0}", semVersion)
                .Append("/p:AssemblyVersion={0}", assemblyVersion)
                .Append("/p:FileVersion={0}", fileVersion)
        });
    });

var pack = Task("Pack")
    .IsDependentOn(build)
    .Does(() =>
    {
        if (!DirectoryExists(nugetRoot))
        {
            CreateDirectory(nugetRoot);
        }

        foreach(var project in GetFiles("./src/**/*.csproj")
                                .Where(file=>!file.FullPath.EndsWith("Tests")))
        {
            DotNetCorePack(project.FullPath, new DotNetCorePackSettings {
                Configuration = configuration,
                OutputDirectory = nugetRoot,
                NoBuild = true,
                ArgumentCustomization = args => args
                    .Append("/p:Version={0}", semVersion)
                    .Append("/p:AssemblyVersion={0}", assemblyVersion)
                    .Append("/p:FileVersion={0}", fileVersion)
            });
        }
    });

Task("Default")
    .IsDependentOn(pack);

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////
RunTarget(target);
```

## Conclusion

Even though there might seem to be some Magic around Cake and it’s DSL it’s mostly just .NET and C#, having skills in those areas — are skills you can use with Cake scripts or when using Cake assemblies in some other way. If you have a different need from what the official supported way offers, you could still have a piece of the Cake.

## Resources

### Cake.Bridge

* GitHub: [github.com/devlead/Cake.Bridge](https://github.com/devlead/Cake.Bridge)
* NuGet: [www.nuget.org/packages/Cake.Bridge](https://www.nuget.org/packages/Cake.Bridge)

### PowerShell proof of concept

* [github.com/devlead/PsPoC](https://github.com/devlead/PsPoC)

### Visual Basic proof of concept

* [github.com/devlead/VBPoC](https://github.com/devlead/VBPoC)

### F# interactive script proof of concept

* [github.com/devlead/FsxPoC](https://github.com/devlead/FsxPoC)

### C# interactive proof of concept

* [github.com/devlead/CsxPoC](https://github.com/devlead/CsxPoC)

### Cake

* Website: [cakebuild.net](https://cakebuild.net)
* GitHub: [github.com/cake-build/cake](https://github.com/cake-build/cake)
* NuGet: [www.nuget.org/profiles/cake-build](https://www.nuget.org/profiles/cake-build)