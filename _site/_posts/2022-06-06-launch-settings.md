---
title: 'Pick your own .NET IDE by sharing dev configuration'
description: 'LaunchSettings Description'
coverimage:
    DEFAULT: '/assets/images/posts/20220606/launch-settings/cover_image.png'
    WEB: '/assets/images/posts/20220606/launch-settings/cover_image.webp'
---
Anno 2022, as `.NET` developers, we are spoilt with multiple options for our development environment. Of course, having a choice sparks the debate that my IDE is better than your IDE. I feel that after `bring your own device`, we are moving to a `bring your own IDE` workspace. Given the rise of tooling like `VS Code DevContainer` and `GitHub Codespaces`, I think more developers will likely opt for such tooling. 

> Did you know that most of my blogs are written for use in dev containers and are available in GitHub Codespaces?

Each IDE has its perks but also its quirks. Who am I to tell you that tool X is better than Y. If you can work using the tool you prefer, you can be much more productive than a tool because the company said so. It does bring its challenges. For example, if you change a file in your IDE, I don't want it formatted when I open it in my IDE. My version control system will show more changes to a project than happened. Lucky for us, tools like `.editorConfig` help us a lot to streamline this process. I switch back and forth a lot between VS Code and Visual Studio. My team was working with `Rider` for a recent customer project. Keeping settings in sync between two IDEs was hard enough. So it made me wonder, is there an equivalent for `.editorConfig` but used for debug-configuration. I knew that `Visual Studio` has the concept of a `launchSettings.json` file. As I discovered, it is possible to make both `Rider` and `VS Code` play nice with `launchSettings.json`. It is by no means perfect, but at least for me, it solves some of the caveats in a `bring your own IDE` world. 

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
        },
        "DemoConsole.V1": {
            "commandName": "Project",
            "environmentVariables": {
                "KAYLUMAH_ENVIRONMENT": "Production"
            }
        },
        "DemoConsole.V2": {
            "commandName": "Project",
            "commandLineArgs": "--mysetting myvalue",
            "environmentVariables": {
                "KAYLUMAH_ENVIRONMENT": "Production"
            }
        },
        "DemoConsole.V3": {
            "commandName": "Project",
            "commandLineArgs": "--mysetting myvalue",
            "environmentVariables": {
                "KAYLUMAH_ENVIRONMENT": "Production",
                "KAYLUMAH_FROMVARIABLE1": "$(TargetFramework)",
                "KAYLUMAH_FROMVARIABLE2": "$(MyCustomProp)"
            }
        }
    }
}
```

The console app will build an `IConfiguration` and print it to the console. Since I don't feel like adding all my environment variables, I add only the ones prefixed with `KAYLUMAH_`, kinda like how .NET automatically includes variables prefixed with `DOTNET_`.

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

If we run the project now, the output should be:

```output
ENVIRONMENT=Development (EnvironmentVariablesConfigurationProvider Prefix: 'KAYLUMAH_')

Done...
```

We also generate a project from the `webapi template`. We slightly modify it so it contains a second profile, so it looks like this.

```json
{
  "$schema": "https://json.schemastore.org/launchsettings.json",
  "iisSettings": {
    "windowsAuthentication": false,
    "anonymousAuthentication": true,
    "iisExpress": {
      "applicationUrl": "http://localhost:33652",
      "sslPort": 44325
    }
  },
  "profiles": {
    "DemoApi": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "launchUrl": "swagger",
      "applicationUrl": "https://localhost:7238;http://localhost:5200",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "DemoApi.Production": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "launchUrl": "swagger",
      "applicationUrl": "https://localhost:7238;http://localhost:5200",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Production"
      }
    },
    "IIS Express": {
      "commandName": "IISExpress",
      "launchBrowser": true,
      "launchUrl": "swagger",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

Depending on your chosen profile, you see a `Swagger UI` dashboard.

## Share debug configuration from Microsoft Visual Studio

I could not verify it online, but I think Visual Studio introduced launch settings as part of the first `ASP NET Core` release. Since launch profiles is a `Visual Studio` feature, I don't have much to add above the definition I've already given for the specification. One cool thing I like to mention is that running from `Visual Studio` `launchSettings` can reference `MSBuild` variables. That is a pretty handy way to provide something dynamic.

For our console, we see the following selection in Visual Studio:

![Microsoft Visual Studio - Console Launch Profile](/assets/images/posts/20220606/launch-settings/visualstudio_console_launchprofile.png){width=2640 height=796}

For our API, we see the following selection in Visual Studio:

![Microsoft Visual Studio - API Launch Profile](/assets/images/posts/20220606/launch-settings/visualstudio_api_launchprofile.png){width=2604 height=1080}

As you see, the WebAPI variant shows more than just our launch profiles.

Another aspect of development configuration is the ability to run more projects simultaneously. We can achieve this in `Visual Studio` by selecting multiple startup projects.  As far as I know, this function is user-specific, which would result in every developer repeating information. Luckily there is a handy plugin called [SwitchStartUpProject](https://marketplace.visualstudio.com/items?itemName=vs-publisher-141975.SwitchStartupProjectForVS2022).

We can quickly provide multiple configurations. We can provide a `ProfileName` for each project that matches one in our launch settings. It is that simple.

```json
{
    "Version": 3,
    "ListAllProjects": false,
    "MultiProjectConfigurations": {
        "Demo": {
            "Projects": {
                "DemoConsole": {
                    "ProfileName": "DemoConsole.V1"
                },
                "DemoApi": {
                    "ProfileName": "DemoApi"
                }
            },
            "SolutionConfiguration": "Release",
            "SolutionPlatform": "x64"
        }
    }
}
```

![Microsoft Visual Studio - Compound Settings](/assets/images/posts/20220606/launch-settings/visualstudio_compound_configuration.png){width=3848 height=348}

## Share debug configuration from JetBrains Rider

As it turns out, `launchSettings` has been supported in `Rider` for a long time. They first introduced it in [November 2018](https://blog.jetbrains.com/dotnet/2018/11/08/using-net-core-launchsettings-json-rundebug-apps-rider/). As a matter of fact, to use `launchSettings` inside `Rider` you don't need to do a thing. `Rider` [automatically detects](https://www.jetbrains.com/help/rider/Run_Debug_Configuration_dotNet_Launch_Settings_Profile.html#creating-run-debug-configurations-based-on-launch-profiles) if your projects are using `launchSettings`. Not all features are supported, but using profiles of `commandName project` are. If you did provide MSBuild variable in `launchSettings` `Rider` would correctly pass them along.

![JetBrains Rider - launch profiles](/assets/images/posts/20220606/launch-settings/rider_launchprofiles.png){width=964 height=904}

A thing I like about `Rider` is that I don't need an additional plugin to support multiple start up projects.

![JetBrains Rider - Compound Settings](/assets/images/posts/20220606/launch-settings/rider_compound_configuration.png){width=3456 height=2144}

It's important to check `Store as project file`; otherwise, you won't share it with your team. In this particular example, it will look like this:

```xml
<component name="ProjectRunConfigurationManager">
  <configuration default="false" name="Console and API" type="CompoundRunConfigurationType">
    <toRun name="DemoApi: DemoApi.Production" type="LaunchSettings" />
    <toRun name="DemoConsole: DemoConsole.V3" type="LaunchSettings" />
    <method v="2" />
  </configuration>
</component>
```

## Share debug configuration from Microsoft VS Code

Last but not least is `VS Code`, the reason I started this article. When you open a .NET project in `VS Code`, you get prompted to create a `launch.json` file. A `launch.json` file is very similar to a `launchSettings.json`. Both options provide the means to choose a project, set command-line arguments and override environment variables. The default `launch.json` does not pass any additional configuration to the project. So what would be the logical output of our command?
The answer might surprise you.

Given the following configuration in `launch.json`

```json
{
    "name": ".NET Core Launch (console)",
    "type": "coreclr",
    "request": "launch",
    "preLaunchTask": "build",
    "program": "${workspaceFolder}/bin/Debug/net6.0/DemoConsole.dll",
    "args": [],
    "cwd": "${workspaceFolder}",
    "console": "internalConsole",
    "stopAtEntry": false
}
```

The output will be:

```output
ENVIRONMENT=Development (EnvironmentVariablesConfigurationProvider Prefix: 'KAYLUMAH_')

Done...
```

 That is because you have secretly been using `launchSettings.json` the whole time. In May 2018, release [1.15.0](https://github.com/OmniSharp/omnisharp-vscode/blob/master/CHANGELOG.md#1150-may-10-2018) of the extension shipped `launchSettings.json` support. If you don't add `launchSettingsProfile` to your `launch.json`, it will use the first profile for a project that is of type `"commandName": "Project"`. Ever had unexplained variables in your project? This is likely the reason why. Remember our default profile set an environment variable, and variables from `launchSettings.json` win from system environment variables. I recommend explicitly specifying `launchSettingsProfile` to make it clear that a) you are using it and b) if you change the order of profiles, you don't create unexpected changes for other developers.

Like `Rider` the support for this feature comes with a few [restrictions](https://github.com/OmniSharp/omnisharp-vscode/blob/master/debugger-launchjson.md#launchsettingsjson-support):
1. Only profiles with "commandName": "Project" are supported.
2. Only environmentVariables, applicationUrl and commandLineArgs properties are supported
3. Settings in launch.json will take precedence over settings in launchSettings.json, so for example, if args is already set to something other than an empty string/array in launch.json then the launchSettings.json content will be ignored.

Since you can provide arguments and environment variables in both `launch.json` and `launchSettings.json`, let's look at an example.

```json
{
    "name": ".NET Core Launch (console)",
    "type": "coreclr",
    "request": "launch",
    "preLaunchTask": "build",
    "program": "${workspaceFolder}/bin/Debug/net6.0/DemoConsole.dll",
    "cwd": "${workspaceFolder}",
    "console": "internalConsole",
    "stopAtEntry": false,
    "launchSettingsProfile": "DemoConsole.V2",
    "args": [
        "--othersetting",
        "vscode"
    ],
    "env": {
        "KAYLUMAH_ENVIRONMENT": "Development",
        "KAYLUMAH_OTHER": "From target"
    }
}
```

```output
ENVIRONMENT=Development (EnvironmentVariablesConfigurationProvider Prefix: 'KAYLUMAH_')
OTHER=From target (EnvironmentVariablesConfigurationProvider Prefix: 'KAYLUMAH_')
othersetting=vscode (CommandLineConfigurationProvider)

Done...
```

There are a few things that happen:
1. Since `launch.json` specified args the commandLineArgs from `launchSettings.json` are ignored.
2. Since `launch.json` specified env and `launchSettings.json` specified `environmentVariables` both sets get merged.
3. Since `launch.json` will win, the value for `KAYLUMAH_ENVIRONMENT` is `Development`.

The default configuration for our web api looks slightly different because it adds support to open the browser after the project starts.
Our base URL comes from the `launchSettings.json`, but the `launchUrl` gets ignored. You can achieve the same behaviour by updating the generated `serverReadyAction` with an `uriFormat`.

```json
{
    "version": "0.2.0",
    "configurations": [
        {
            "name": ".NET Core Launch (web)",
            "type": "coreclr",
            "request": "launch",
            "preLaunchTask": "build",
            "program": "${workspaceFolder}/bin/Debug/net6.0/DemoApi.dll",
            "args": [],
            "cwd": "${workspaceFolder}",
            "stopAtEntry": false,
            "serverReadyAction": {
                "action": "openExternally",
                "pattern": "\\bNow listening on:\\s+(https?://\\S+)",
                "uriFormat": "%s/swagger"
            },
            "env": {
                "ASPNETCORE_ENVIRONMENT": "Development"
            },
            "sourceFileMap": {
                "/Views": "${workspaceFolder}/Views"
            }
        },
        {
            "name": ".NET Core Attach",
            "type": "coreclr",
            "request": "attach"
        }
    ]
}
```

Of the three IDEs, `VS Code` has the easiest way to share compound configurations. Just add the following to your `launch.json`:

```json
"compounds": [
    {
        "name": "Console + API",
        "configurations": [
            "Launch WebAPI",
            "Launch Console"
        ]
    }
]
```

![Microsoft VS Code - launch profiles](/assets/images/posts/20220606/launch-settings/vscode_launchprofiles.png){width=680 height=620}

## Bonus use Launch Settings from Dotnet CLI

Technically the Dotnet CLI is not an IDE, so consider this a small bonus chapter. I am including the CLI since it also uses launch profiles when running locally.

As it turns out the CLI also defaults to the first project in `Properties\launchSettings.json` so in our case `DemoConsole.V0`. Just like VS Code did. The following example uses a bit of `PowerShell` to run the CLI.

```pwsh
# prints the default
dotnet run

# Sets env var for current session
$env:KAYLUMAH_COMMANDLINE="Session ENV var"
# prints COMMANDLINE + the default
dotnet run
```

If we don't want any launch profile just run `dotnet run --no-launch-profile` and to specify a profile run `dotnet run --launch-profile "DemoConsole.V2"`

## Closing Thoughts

## Sources Used

- https://github.com/dotnet/project-system/blob/main/docs/launch-profiles.md
- https://www.jetbrains.com/help/rider/Run_Debug_Configuration_dotNet_Launch_Settings_Profile.html
- https://github.com/OmniSharp/omnisharp-vscode/blob/master/debugger-launchjson.md#launchsettingsjson-support
- https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-run
- https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-6.0#environment-variables-set-in-generated-launchsettingsjson
- https://docs.microsoft.com/en-us/aspnet/core/fundamentals/environments?view=aspnetcore-6.0#development-and-launchsettingsjson

