---
title: SLNX Finally here📄
tags:
    - .NET
    - Visual Studio    
author: devlead
description: The new .NET solution format has evolved from being messy and bloated to being focused and clean
published: 2025-02-24
image: https://cdn.devlead.se/clipimg-vscode/2025/02/24/b83c06ea-69c7-be32-31cd-da3c5e6a5173.png?sv=2025-01-05&st=2025-02-23T06%3A49%3A41Z&se=2035-02-24T06%3A49%3A41Z&sr=b&sp=r&sig=B0uwPpXawEe%2BfbdTgsiNHrULc3f6zv%2BUgHR%2Fdk3tlQA%3D
---

The Visual Studio solution files have long been an explicit and messy format, with lots of configuration that could be inferred from conventions. However, with the release of the latest .NET 9 SDK (9.0.200) earlier this month, things have changed. The new XML-based solution format, SLNX, is now out of preview, bringing clean, convention-based defaults while still allowing for explicit configuration when needed.

## What has changed?

Let's look at a simple "Hello World" example to illustrate the difference between the old and new formats.

### Traditional .sln file - HelloWorld.sln

```ini
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version 17
VisualStudioVersion = 17.0.31903.59
MinimumVisualStudioVersion = 10.0.40219.1
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "HelloWorld", "HelloWorld\HelloWorld.csproj", "{979C8E48-A2EA-4647-A3A1-8647AB5F20C6}"
EndProject
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "HelloWorld.Tests", "HelloWorld.Tests\HelloWorld.Tests.csproj", "{3B7AB8F1-88C0-4303-856B-A1E1EDE0A736}"
EndProject
Global
        GlobalSection(SolutionConfigurationPlatforms) = preSolution
                Debug|Any CPU = Debug|Any CPU
                Debug|x64 = Debug|x64
                Debug|x86 = Debug|x86
                Release|Any CPU = Release|Any CPU
                Release|x64 = Release|x64
                Release|x86 = Release|x86
        EndGlobalSection
        GlobalSection(ProjectConfigurationPlatforms) = postSolution
                {979C8E48-A2EA-4647-A3A1-8647AB5F20C6}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
                {979C8E48-A2EA-4647-A3A1-8647AB5F20C6}.Debug|Any CPU.Build.0 = Debug|Any CPU
                {979C8E48-A2EA-4647-A3A1-8647AB5F20C6}.Debug|x64.ActiveCfg = Debug|Any CPU
                {979C8E48-A2EA-4647-A3A1-8647AB5F20C6}.Debug|x64.Build.0 = Debug|Any CPU
                {979C8E48-A2EA-4647-A3A1-8647AB5F20C6}.Debug|x86.ActiveCfg = Debug|Any CPU
                {979C8E48-A2EA-4647-A3A1-8647AB5F20C6}.Debug|x86.Build.0 = Debug|Any CPU
                {979C8E48-A2EA-4647-A3A1-8647AB5F20C6}.Release|Any CPU.ActiveCfg = Release|Any CPU
                {979C8E48-A2EA-4647-A3A1-8647AB5F20C6}.Release|Any CPU.Build.0 = Release|Any CPU
                {979C8E48-A2EA-4647-A3A1-8647AB5F20C6}.Release|x64.ActiveCfg = Release|Any CPU
                {979C8E48-A2EA-4647-A3A1-8647AB5F20C6}.Release|x64.Build.0 = Release|Any CPU
                {979C8E48-A2EA-4647-A3A1-8647AB5F20C6}.Release|x86.ActiveCfg = Release|Any CPU
                {979C8E48-A2EA-4647-A3A1-8647AB5F20C6}.Release|x86.Build.0 = Release|Any CPU
                {3B7AB8F1-88C0-4303-856B-A1E1EDE0A736}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
                {3B7AB8F1-88C0-4303-856B-A1E1EDE0A736}.Debug|Any CPU.Build.0 = Debug|Any CPU
                {3B7AB8F1-88C0-4303-856B-A1E1EDE0A736}.Debug|x64.ActiveCfg = Debug|Any CPU
                {3B7AB8F1-88C0-4303-856B-A1E1EDE0A736}.Debug|x64.Build.0 = Debug|Any CPU
                {3B7AB8F1-88C0-4303-856B-A1E1EDE0A736}.Debug|x86.ActiveCfg = Debug|Any CPU
                {3B7AB8F1-88C0-4303-856B-A1E1EDE0A736}.Debug|x86.Build.0 = Debug|Any CPU
                {3B7AB8F1-88C0-4303-856B-A1E1EDE0A736}.Release|Any CPU.ActiveCfg = Release|Any CPU
                {3B7AB8F1-88C0-4303-856B-A1E1EDE0A736}.Release|Any CPU.Build.0 = Release|Any CPU
                {3B7AB8F1-88C0-4303-856B-A1E1EDE0A736}.Release|x64.ActiveCfg = Release|Any CPU
                {3B7AB8F1-88C0-4303-856B-A1E1EDE0A736}.Release|x64.Build.0 = Release|Any CPU
                {3B7AB8F1-88C0-4303-856B-A1E1EDE0A736}.Release|x86.ActiveCfg = Release|Any CPU
                {3B7AB8F1-88C0-4303-856B-A1E1EDE0A736}.Release|x86.Build.0 = Release|Any CPU
        EndGlobalSection
        GlobalSection(SolutionProperties) = preSolution
                HideSolutionNode = FALSE
        EndGlobalSection
EndGlobal
```


### New SLNX file - HelloWorld.slnx

```xml
<Solution>
    <Project Path="HelloWorld/HelloWorld.csproj" />
    <Project Path="HelloWorld.Tests/HelloWorld.Tests.csproj" />
</Solution>
```

The code speaks for itself. The SLNX file is much cleaner and easier to read, with sensible defaults mean you only add exceptions when needed.

## Getting started

### Prerequisites

To get started with the new SLNX format, you need to update your .NET SDK to version `9.0.200` or later. You can verify your current version by running the following command:

```bash
dotnet --version
```

If you need to update, you can download the latest version from [https://get.dot.net/](https://get.dot.net/) or update using the Visual Studio installer.

For Visual Studio you might still need to enable the new format under `Tools` -> `Options` -> `Environment` -> `Preview Features` -> `Use Solution File Persistence Model`.

![Visual Studio Tools Options - Enable SLNX](https://cdn.devlead.se/clipimg-vscode/2025/02/24/a21f4139-6e77-b134-9d45-efefaced787e.png?sv=2025-01-05&st=2025-02-23T07%3A22%3A06Z&se=2035-02-24T07%3A22%3A06Z&sr=b&sp=r&sig=IPOH7kSyA1xh2njkGtiwEA6OPmalNNGKd2pJz348ZKw%3D)

### Create a new SLNX file

To create a new `SLNX` file, you can use the following command:

```bash
dotnet new sln --format slnx
```

This will create a new solution file with the SLNX format.

### Convert an existing `.sln` file to SLNX

To convert an existing `.sln` file to SLNX, you can use the following command:

```bash
dotnet sln migrate <SLN_FILE>
```
i.e.

```bash
dotnet sln migrate HelloWorld.sln
```

This will create a new solution file with the SLNX format based on the existing `.sln` file.

## Working with SLNX programmatically

Microsoft provides the [Microsoft.VisualStudio.SolutionPersistence](https://www.nuget.org/packages/Microsoft.VisualStudio.SolutionPersistence) NuGet package, which provides a clean API for working with both traditional `.sln` and new `.slnx` files programmatically.

The entry point to serializers can be found on the `SolutionSerializers` static class. This has the helper `GetSerializerByMoniker` that can pick the serializer for a file extension, or a specific serializers can be used.

Here's a simple example of how to read and write SLNX files:

```csharp
using Microsoft.VisualStudio.SolutionPersistence.Serializer;

// Open and deserialize the SLNX file
var solution = await SolutionSerializers.SlnXml.OpenAsync("HelloWorld.slnx", cancellationToken);

// Iterate through all projects in the solution
foreach (var project in solution.SolutionProjects)
{
        // Print the file path of each project
        Console.WriteLine(project.FilePath);
}
```

More examples can be found in the project GitHub wiki at [github.com/microsoft/vs-solutionpersistence/wiki/Samples](https://github.com/microsoft/vs-solutionpersistence/wiki/Samples).


## Conclusion

The new SLNX format is a great step forward, bringing clean, convention-based defaults while still allowing explicit configuration when needed. I belive this longterm will improve tooling and maintainability of solution files. It will also improve the developer experience by simplifying authoring and maintenance, reducing merge conflicts, and making it easier to sort them when they occur.

So if you haven't already, give it a try and let me know what you think!
