---
title: Just an environment variable away from sleep
tags:
    - .NET
    - Azure
    - C#
    - DevOps
    - Release Notes
author: devlead
published: 2019-12-06
canonical: https://medium.com/@devlead/just-an-environment-variable-away-from-sleep-15013adc8cb6
---

It’s a quarter past midnight, you should be going to sleep, but there’s that one unit test that fails only on GitHub Action macOS build agent — it’s mocking you so you stay awake just a bit longer…

![Failed MacOS build on GitHub PR](https://cdn.devlead.se/clipimg-vscode/2021/01/11/002a2a50-81cb-491e-145e-bc8ef7ceb2bd.png?sv=2019-12-12&st=2021-01-10T09%3A13%3A54Z&se=2031-01-11T09%3A13%3A54Z&sr=b&sp=r&sig=QDRIJM%2FTAGDrRbSk7zA%2FrdfRBNBxhZoIZuIkrmbWEQg%3D)

You bring out you Mac and execute tests, fortunately we can reproduce — the test fails on your machine too! A quick inspection of test output informs you that error only occurs on .NET Core.

Unfortunately, today neither VS Mac nor VSCode for some reason is your friend, solutions won’t build, tests aren’t found, break points aren’t hit and so on.

Probably not their fault at all, more likely a case of complex multi target solution, having preview versions and just being too tired.

Fortunately, as the .NET CLI “dotnet test” command executed by the build script compiled and executed the tests, one could leave the VS Mac/Code IDE tooling debugging for another day and trigger debugging from the command line.

This is achieved by setting the environment variable `VSTEST_HOST_DEBUG` to `1`.

### Bash

```bash
export VSTEST_HOST_DEBUG=1
```

### PowerShell

```powershell
$env:VSTEST_HOST_DEBUG=1
```

Now when executing the test (specifying only the framework I want to test)

```bash
dotnet test My.Tests.csproj —-framework=netcoreapp3.0
```

It’ll will pause and wait for a debugger to attach.

```bash
Microsoft (R) Test Execution Command Line Tool Version 16.3.0
Copyright (c) Microsoft Corporation.  All rights reserved.Starting test execution, please wait...A total of 1 test files matched the specified pattern.
Host debugging is enabled. Please attach debugger to testhost process to continue.
Process Id: 32723, Name: dotnet
Waiting for debugger attach…
Process Id: 32723, Name: dotnet
```

![Attach to Process in VSMac](https://cdn.devlead.se/clipimg-vscode/2021/01/11/3f6c41b2-be6d-1259-483f-0cb610799074.png?sv=2019-12-12&st=2021-01-10T09%3A45%3A48Z&se=2031-01-11T09%3A45%3A48Z&sr=b&sp=r&sig=OtBMnkTPJeuR93Eh9vo8MSGB5SwUiRnSH6UOw3WN4HY%3D)

and VS Code

![Attach to Process VSCode](https://cdn.devlead.se/clipimg-vscode/2021/01/11/0d7a04ed-add1-3db6-6c14-55e0e733ce9f.png?sv=2019-12-12&st=2021-01-10T09%3A46%3A29Z&se=2031-01-11T09%3A46%3A29Z&sr=b&sp=r&sig=NnZf%2Fdn1BQLaaj4Wos2%2Bn9QMDc4WLXK5AN8K6Cj8swo%3D)

Which allowed me to set break points, inspect variables and step through code, even though the IDEs themselves for some reason couldn’t compile and execute the tests.

![IDE test break points hit](https://cdn.devlead.se/clipimg-vscode/2021/01/11/fa197436-9d59-63e7-00dc-795b2df63c12.png?sv=2019-12-12&st=2021-01-10T09%3A47%3A13Z&se=2031-01-11T09%3A47%3A13Z&sr=b&sp=r&sig=W2frMVBD%2Bs4IH8a%2FCyPas4Aw2yVpzfzhF9%2FpXUT1VF0%3D)

Fairly quickly found the issue added a commit and builds were green and I could go to sleep.

![macOS build passes](https://cdn.devlead.se/clipimg-vscode/2021/01/11/f94d4dea-7f34-2d6b-2b98-b78a727ebe35.png?sv=2019-12-12&st=2021-01-10T09%3A47%3A48Z&se=2031-01-11T09%3A47%3A48Z&sr=b&sp=r&sig=tgplc9XhXuY5qBJLd%2FCSU4ZUKaz%2BgorxGG%2B9h0%2F2C60%3D)

Sometimes you just want to attack the problem and not debug tooling, then attacking the problem from another direction might get you there quicker, today it was setting `VSTEST_HOST_DEBUG` to `1`.
