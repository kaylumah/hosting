---
title: 'Using C# code in your git hooks'
description: 'Getting started with C# script in your client-side git hooks'
image: /assets/images/posts/20190907/githooks/cover_image.png
coverimage:
    DEFAULT: '/assets/images/posts/20190907/githooks/cover_image.png'
    WEB: '/assets/images/posts/20190907/githooks/cover_image.webp'
tags:
    - csharp
    - git
modifieddate: '2021-03-21'
commentid: '12'
featured: true
---

## Why use hooks?

We, as developers, love platforms like GitHub, GitLab, Atlassian, Azure DevOps etc., as our managed git system and collaboration platform. We also love clean code and keep inventing new linters and rules to enforce it. In my opinion, every commit should allow the codebase to deploy to production. There is nothing worse than commits like “fixed style errors” or “fixed build”. These are often small mistakes you want to know as early as possible in your development cycle. You don’t want to break the build for the next developer because he pulled your ‘mistake’ or waste precious build minutes of your CI server. Say you have asked your teammate to review your code; in the meantime, the build server rejects your code. That means you have to go back and fix this, and your teammate has to come back and possibly review again after the changes (i.e., approvals reset on new commit). Doing so would waste a lot of time and effort.

> **note**: I favour server-side hooks, but when using a SaaS solution, this is not always a possibility. I know I would not want someone to run arbitrary code on my servers. Unfortunately, a developer can bypass the client-side hooks. Until we can run, possibly sandboxed, server-side hooks on our prefered platform, we have to make the best of it by using client-side hooks.

Githooks are scripts that can execute on certain parts of the git lifecycle. Hooks must be executable, but other than that, hooks' power is only limited to the developer's imagination. I have seen many samples of hooks written in JavaScript (node) using tools like [husky](https://github.com/typicode/husky) and [commitlint](https://github.com/conventional-changelog/commitlint) to enforce a certain way of working. When I was browsing the changes in the upcoming .NET Core 3.0 release, the concept of [local-tools](https://docs.microsoft.com/en-us/dotnet/core/whats-new/dotnet-core-3-0#local-dotnet-tools) got me thinking. I knew of the existence of [dotnet-script](https://www.hanselman.com/blog/CAndNETCoreScriptingWithTheDotnetscriptGlobalTool.aspx), would that make it possible to C# in my GitHooks?

> **note**: in the past I have used a set-up with node since I occasionally work with front-end frameworks like Angular. Since I had node installed I could use it even in my pure backend projects to enforce commit messages and such. For me it felt dirty, since that would require team members to have node installed. Using the dotnet cli feels less as a forced decision since members are likely to have it installed already.

## Let’s get started!

When creating a git repository there is a folder called hooks where all the git hooks are placed. For every event there is a sample post-fixed with .sample that shows the possibility of each hook. This directory is not under source control and we are going to create our own directory to be able to share the hooks with the team.

```bash
mkdir git-hooks-example  
cd git-hooks-example  
git init  
dotnet new gitignore  
dotnet new tool-manifest  
dotnet tool install dotnet-script  
dotnet tool install dotnet-format  
mkdir .githooks
```

### Pre-Commit Hook

To demonstrate we are going to create a plain hook. To check if it is working **git commit -m “”** (using empty commit message will abort the commit). You should see the line pre-commit hook printed.

```csharp
#!/usr/bin/env dotnet dotnet-script
Console.WriteLine("pre-commit hook");
```

To make it executable run:

```bash
find .git/hooks -type f -exec rm {} \;
find .githooks -type f -exec chmod +x {} \;
find .githooks -type f -exec ln -sf ../../{} .git/hooks/ \;
```

Since we can reference other files (and even load nuget packages) in our csx we will first create a couple of files so we can have code-reuse between the hooks.

Create a file called **logger.csx**

```csharp
public class Logger
{
    public static void LogInfo(string message)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.Error.WriteLine(message);
    }
    public static void LogError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Error.WriteLine(message);
    }
}
```

Create a file called **command-line.csx**

```csharp
#load "logger.csx"
public class CommandLine
{
    public static string Execute(string command)
    {
        // according to: https://stackoverflow.com/a/15262019/637142
        // thans to this we will pass everything as one command
        command = command.Replace("\"", "\"\"");
        var proc = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = "-c \"" + command + "\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            }
        };
        proc.Start();
        proc.WaitForExit();
        if (proc.ExitCode != 0)
        {
            Logger.LogError(proc.StandardOutput.ReadToEnd());
            return proc.ExitCode.ToString();
        }
        return proc.StandardOutput.ReadToEnd();
    }
}
```

Create a file called **dotnet-commands.csx**

```csharp
#load "logger.csx"
#load "command-line.csx"
public class DotnetCommands
{
    public static int FormatCode() => ExecuteCommand("dotnet format");
    public static int BuildCode() => ExecuteCommand("dotnet build");

    public static int TestCode() => ExecuteCommand("dotnet test");

    private static int ExecuteCommand(string command)
    {
        string response = CommandLine.Execute(command);
        Int32.TryParse(response, out int exitCode);
        return exitCode;
    }

}
```

Create a file called **git-commands.csx**

```csharp
#load "logger.csx"
#load "command-line.csx"
public class GitCommands
{
    public static void StashChanges()
    {
        CommandLine.Execute("git stash -q --keep-index");
    }
    public static void UnstashChanges()
    {
        CommandLine.Execute("git stash pop -q");
    }
}
```

We now have a utility in place for Logging and running GIT and dotnet commands. Next we are going to start with out pre-commit hook. Create a file called **pre-commit** The difference between this file and the others we just made is that we don’t specify the extension, and that using Shebang we explicitly load dotnet-script. For an explanation of each hook see the article posted below.

[Git Hooks | Atlassian Git Tutorial](https://www.atlassian.com/git/tutorials/git-hooks)

```csharp
#!/usr/bin/env dotnet dotnet-script
#load "logger.csx"
#load "git-commands.csx"
#load "dotnet-commands.csx"

// We'll only runchecks on changes that are a part of this commit so let's stash others
GitCommands.StashChanges();

int buildCode = DotnetCommands.BuildCode();

// We're done with checks, we can unstash changes
GitCommands.UnstashChanges();
if (buildCode != 0) {
    Logger.LogError("Failed to pass the checks");
    Environment.Exit(-1);
}
// All checks have passed
```

If we run **git commit -m “”** again this time we get an error saying Failed to pass the checks, which makes sense since we don’t have a project yet. We are going to create a simple sln consisting of a classlibary and a test libary.

```bash
dotnet new sln  
dotnet new classlib --framework netstandard2.1 --langVersion 8 --name SomeLib --output src/SomeLib  
dotnet new xunit --output tests/SomeLibTests  
dotnet sln add **/*.csproj 
cd tests/SomeLibTests/  
dotnet add reference ../../src/SomeLib/SomeLib.csproj  
cd ../../  
dotnet build
```

If we use git commit -m “” one more time, we get the message about aborting the commit again. We now know that every commit will at least compile :-) If for example we remove the namespace ending curly brace from Class1 we get the error **Class1.cs(7,6): error CS1513: }**. If we extend our pre-commit hook even further we can have [dotnet-format](https://www.hanselman.com/blog/EditorConfigCodeFormattingFromTheCommandLineWithNETCoresDotnetFormatGlobalTool.aspx) and dotnet-test running on every commit. If we purposely write a failing test (1 equals 0 or something like that) the build won’t pass.

```csharp
#!/usr/bin/env dotnet dotnet-script
#load "logger.csx"
#load "git-commands.csx"
#load "dotnet-commands.csx"

Logger.LogInfo("pre-commit hook");

// We'll only runchecks on changes that are a part of this commit so let's stash others
GitCommands.StashChanges();

int formatCode = DotnetCommands.FormatCode();
int buildCode = DotnetCommands.BuildCode();
int testCode = DotnetCommands.TestCode();

// We're done with checks, we can unstash changes
GitCommands.UnstashChanges();
int exitCode = formatCode + buildCode + testCode;
if (exitCode != 0) {
    Logger.LogError("Failed to pass the checks");
    Environment.Exit(-1);
}
// All checks have passed
```

### Prepare-commit-message hook

Thus far, we have not really used anything we need C# for; Admittedly we are using C# to execute shell commands. For our next hook we are going to use System.IO. Imagine as a team you have a commit-message convention. Let's say you want each commit message to include a reference to your issue tracker.

```text
type(scope?): subject  #scope is optional
```

Create a file **prepare-commit-msg** in this hook we can provide a convenient commit message place holder if the user did not supply a message. To actual enforce the message, you need the **commit-msg** hook. In this example, we only create a message for feature branches.

```csharp
#!/usr/bin/env dotnet dotnet-script
#load "logger.csx"
#load "util.csx"
#load "git-commands.csx"

Logger.LogInfo("prepare-commit-msg hook");

string commitMessageFilePath = Util.CommandLineArgument(Args, 0);
string commitType = Util.CommandLineArgument(Args, 1);
string commitHash = Util.CommandLineArgument(Args, 2);

if (commitType.Equals("message")) {
    // user supplied a commit message, no need to prefill.
    Logger.LogInfo("commitType message");
    Environment.Exit(0);
}

string[] files = GitCommands.ChangedFiles();
for(int i = 0; i < files.Length; i++) {
    // perhaps determine scope based on what was changed.
    Logger.LogInfo(files[i]);
}

string branch = GitCommands.CurrentBranch();
if (branch.StartsWith("feature")) {
    string messageToBe = "feat: ISS-XXX";
    PrepareCommitMessage(commitMessageFilePath, messageToBe);
}

public static void PrepareCommitMessage(string messageFile, string message)
{
     string tempfile = Path.GetTempFileName();
    using (var writer = new StreamWriter(tempfile))
    using (var reader = new StreamReader(messageFile))
    {
        writer.WriteLine(message);
        while (!reader.EndOfStream)
            writer.WriteLine(reader.ReadLine());
    }
    File.Copy(tempfile, messageFile, true);
}
```

Create a new helper called **util.csx**

```csharp
public class Util
{
    public static string CommandLineArgument(IList<string> Args, int position)
    {
        if (Args.Count() >= position + 1)
        {
            return Args[position];
        }
        return string.Empty;
    }

}
```

### Commit-msg Hook

The final local git hook I took for a spin is the commit-msg hook. It uses a regex to make sure the commit message is according the specified format.

```csharp
#!/usr/bin/env dotnet dotnet-script
#load "logger.csx"
#load "util.csx"
#load "git-commands.csx"
using System.Text.RegularExpressions;

Logger.LogInfo("commit-msg hook");

string commitMessageFilePath = Util.CommandLineArgument(Args, 0);
string branch = GitCommands.CurrentBranch();
Logger.LogInfo(commitMessageFilePath);
Logger.LogInfo(branch);
string message = GetCommitedMessage(commitMessageFilePath);
Logger.LogInfo(message);

const string regex = @"\b(feat|bug)\b(\({1}\b(core)\b\){1})?(:){1}(\s){1}(ISS-[0-9]{0,3}){1}";
var match = Regex.Match(message, regex);

if (!match.Success) {
    Logger.LogError("Message does not match commit format");
    Environment.Exit(1);
}

public static string GetCommitedMessage(string filePath) {
    return File.ReadAllLines(filePath)[0];
}
```

### pre push Hook

It is even possible to use NuGet packages in our hooks. Let say we want to prevent pushes to master (perhaps not even commits?). We can read a config file using Newtonsoft.Json and look for a protected branch and abort.

```csharp
#!/usr/bin/env dotnet dotnet-script
#r "nuget: Newtonsoft.Json, 12.0.2"
#load "logger.csx"
#load "config.csx"
#load "git-commands.csx"
using Newtonsoft.Json;

string currentBranch = GitCommands.CurrentBranch().Trim();
Config currentConfig = GetConfig();
bool lockedBranch = currentConfig.ProtectedBranches.Contains(currentBranch);

if (lockedBranch) {
    Logger.LogError($"Trying to commit on protected branch '{currentBranch}'");
    Environment.Exit(1);
}

public static Config GetConfig()
{
    return JsonConvert.DeserializeObject<Config>(File.ReadAllText(".githooks/config.json"));
}
```

## Conclusion

My current hooks are far from the best, and perhaps C# is not the fastest language to use in git hook. I do, however consider the experiment a success. I much rather code in C# than in shell script. Ideas for further improvement include

*   based on the list of changes, determine the scope of the change (i.e. only one directory changed we might know the scope)
*   configure the regex, allowed scopes, allowed types
*   improve pre-commit-msg for more scenarios
*   enforce users to use the hooks
*   managing versions of the hooks, on checkout old / different version of pull (with an update of the hooks) sync the directory. [(perhaps githook location)](https://www.viget.com/articles/two-ways-to-share-git-hooks-with-your-team/)

Let me know what you think :-)

[maxhamulyak/git-hooks-example](https://github.com/maxhamulyak/git-hooks-example)

Happy Coding 🍻
