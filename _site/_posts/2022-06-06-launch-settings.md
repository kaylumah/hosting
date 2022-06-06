---
title: 'LaunchSettings Title'
description: 'LaunchSettings Description'
coverimage:
    DEFAULT: '/assets/images/posts/20220606/launch-settings/cover_image.png'
    WEB: '/assets/images/posts/20220606/launch-settings/cover_image.webp'
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

## Project Setup

For our sample application, we will create a new project using the "Console Template" with `dotnet new console`. Since it is a console, we must create a `Properties\launchSettings.json` by hand. At a minimum, the file would look like this.

```json
{
    "$schema": "https://json.schemastore.org/launchsettings.json",
    "profiles": {
        "DemoConsole.V0": {
            "commandName": "Project"
        }
    }
}
```

Since we are demoing features of `launchSettings.json`, it will not be a nice demo if we don't populate it.

```json
{
    "$schema": "https://json.schemastore.org/launchsettings.json",
    "profiles": {
        "DemoConsole.V0": {
            "commandName": "Project",
            "commandLineArgs": "",
            "environmentVariables": {
                "KAYLUMAH_ENVIRONMENT": "Development"
            }
        }
    }
}
```

The console app will build an `IConfiguration` and print it to the console. Since I don't feel like adding all my environment variables, I add only the ones prefixed with `KAYLUMAH_` kinda like how .NET automatically includes variables prefixed with `DOTNET_`.

```csharp
using Microsoft.Extensions.Configuration;

IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
configurationBuilder.AddEnvironmentVariables("KAYLUMAH_");
if (args is { Length: > 0 })
{
    configurationBuilder.AddCommandLine(args);
}
var configuration = configurationBuilder.Build();

if (configuration is IConfigurationRoot configurationRoot)
{
    Console.WriteLine(configurationRoot.GetDebugView());
}

Console.WriteLine("Done...");
Console.ReadLine();
```

If we run the project now the output should be:

```output
ENVIRONMENT=Development (EnvironmentVariablesConfigurationProvider Prefix: 'KAYLUMAH_')

Done...
```

## How to use Launch Settings from VS Code?

Since I initially started writing this article to use launch settings with `VS Code`, let's open with `VS Code`. When you open a .NET project in `VS Code`, you get prompted to create a `launch.json` file. If you missed the prompt, you could run the command `.NET: Generate Assets for Build and Debug`. A `launch.json` file is very similar to a `launchSettings.json`. Both options provide the means to choose a project, set command-line arguments and override environment variables.

The default `launch.json` does not pass any additional configuration to the project. So what would be the logical output of our command? Should it display anything? Yep, it should. Don't believe me? Run the project; the evidence will speak for itself.

```output
ENVIRONMENT=Development (EnvironmentVariablesConfigurationProvider Prefix: 'KAYLUMAH_')

Done...
```

 That is because you have secretly been using `launchSettings.json` the whole time. In May 2018, release [1.15.0](https://github.com/OmniSharp/omnisharp-vscode/blob/master/CHANGELOG.md#1150-may-10-2018) of the extension shipped `launchSettings.json` support. If you don't add `launchSettingsProfile` to your `launch.json`, it will use the first profile for a project that is of type `"commandName": "Project"`. Ever had unexplained variables in your project? Now you know why.

I recommend explicitly specifying `launchSettingsProfile` to make it clear that a) you are using it and b) if you change the order of profiles, you don't create unexpected changes for other developers.

The support for this feature comes with a few [restrictions](https://github.com/OmniSharp/omnisharp-vscode/blob/master/debugger-launchjson.md#launchsettingsjson-support):
1. Only profiles with "commandName": "Project" are supported.
2. Only environmentVariables, applicationUrl and commandLineArgs properties are supported
3. Settings in launch.json will take precedence over settings in launchSettings.json, so for example, if args is already set to something other than an empty string/array in launch.json then the launchSettings.json content will be ignored.



## Rider

How to use LaunchSettings in JetBrains Rider?

Rider [launched support](https://blog.jetbrains.com/dotnet/2018/11/08/using-net-core-launchsettings-json-rundebug-apps-rider/) for LaunchSettings.json way back in November 2018. 

## How to use Launch Settings from Dotnet CLI

Technically the Dotnet CLI is not an IDE, so consider this a small bonus chapter. I am including the CLI since it also uses launch profiles when running locally.

```pwsh
# prints USER=Max
dotnet run

# Sets env var for current session
$env:KAYLUMAH_COMMANDLINE="Session ENV var"
# prints USER=Max, COMMANDLINE=Session ENV var
dotnet run
```

```pwsh
dotnet run --no-launch-profile
```

```pwsh
dotnet run --launch-profile "DemoConsole.V2"
```


## Sources Used

- https://github.com/dotnet/project-system/blob/main/docs/launch-profiles.md
- https://www.jetbrains.com/help/rider/Run_Debug_Configuration_dotNet_Launch_Settings_Profile.html
- https://github.com/OmniSharp/omnisharp-vscode/blob/master/debugger-launchjson.md#launchsettingsjson-support
- https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-6.0#environment-variables-set-in-generated-launchsettingsjson
- https://docs.microsoft.com/en-us/aspnet/core/fundamentals/environments?view=aspnetcore-6.0#development-and-launchsettingsjson

