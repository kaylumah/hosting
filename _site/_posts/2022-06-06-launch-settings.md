---
title: 'LaunchSettings Title'
description: 'LaunchSettings Description'
---
Anno 2022, as `.NET` developers, we are spoilt with multiple options for our development environment. Of course, having a choice sparks the debate that my IDE is better than your IDE. I feel that after `bring your own device`, we are moving to a `bring your own IDE` workspace. Given the rise of tooling like `VS Code DevContainer` and `GitHub Codespaces`, I think more developers will likely opt for such tooling. Did you know that most of my blogs are written for use in dev containers and are available in GitHub Codespaces?

Each IDE has its perks but also its quirks. Who am I to tell you that tool X is better than Y. If you can work using the tool you prefer, you can be much more productive than a tool because the company said so. It does bring its challenges. For example, if you change a file in your IDE, I don't want it formatted when I open it in my IDE. My version control system will show more changes to a project than happened. Lucky for us, tools like `.editorConfig` help us a lot to streamline this process.

I switch back and forth a lot between VS Code and Visual Studio. For a recent customer project, my team was working with Rider. Keeping settings in sync between two IDEs was hard enough. So it made me wonder, is there an equivalent for `.editorConfig` but used for debug-configuration. I knew that `Visual Studio` has the concept of a `launchSettings.json` file. As I discovered, it is possible to make both `Rider` and `VS Code` play nice with `launchSettings.json`. It is by no means perfect, but at least for me, it solves some caveats in a `bring your own IDE` world. 

If you were wondering, "Max launchSettings.json has been around for years; why are you writing this article?" The answer to that is straightforward. It bugged me a lot that I had to repeat myself. When searching for how to configure my IDE, I came across the [ASP.NET Fundamentals - Environment](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/environments?view=aspnetcore-6.0#development-and-launchsettingsjson). It suggests using the `VS Code` variant of configuration but does not mention that you can reuse your `Visual Studio` one. Reading that article prompted me to write down what I learned so that someday someone might benefit from it.

## What is LaunchSettings?

Let me begin with a quick recap about `launchSettings.json`. A launch settings file contains [Launch Profiles](https://github.com/dotnet/project-system/blob/main/docs/launch-profiles.md). A `Launch Profile` is a kind of configuration that specifies how to run your project. Having these launch profiles allows you to switch between configurations easily. Typical scenarios are switching between Development and Production environments or enabling feature flags. Launch profiles are in the optional `Properties\launchSettings.json` file. For example, a freshly created console project will not have one, whereas a web API project will define one.

A launch profile has a couple of properties depending on the project type. I will highlight the ones that are relevant to this post.
- `commandName`: the only required setting which determines how a project is launched. To work in every IDE this settings needs to be `Project`.
- `commandLineArgs`: a string containing arguments to supply to the application.
- `environmentVariables`: A collection of name/value pairs, each specifying an environment variable and value to set.

A few important notes:
- Environment values set in launchSettings.json override values set in the system environment.
- The launchSettings.json file is only used on the local development machine.
- The launchSettings.json file shouldn't store secrets

## CommandLine

```pwsh
# prints USER=Max
dotnet run

# Sets env var for current session
$env:KAYLUMAH_COMMANDLINE="Hello World"
# prints USER=Max, COMMANDLINE=Hello World
dotnet run
```

## Sources Used

- https://github.com/dotnet/project-system/blob/main/docs/launch-profiles.md
- https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-6.0#environment-variables-set-in-generated-launchsettingsjson

