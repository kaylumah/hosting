---
title: 'Share NuGet packages via local feed.'
description: ''
---
NuGet is the Microsoft-supported package manager for .NET. A NuGet package is a single zip file with the `.nupkg` extension.
It contains the dlls and other files related to the code, such as a descriptive manifest.

NuGet package can be shared via both public and private hosts. This way you can share code that is exclusive to your organization. In fact the package is just a shareable unit of code, but the means of sharing is up to you. For example, you might want to use a local copy of a package to test it in a project before sharing it with others.

Today we are looking at local package feeds.


-------

image https://github.com/NuGet/Home/blob/dev/README.md

[image https://docs.microsoft.com/en-us/nuget/nuget-org/overview-nuget-org]

https://docs.microsoft.com/en-us/nuget/what-is-nuget

https://en.wikipedia.org/wiki/NuGet

https://github.com/features/packages

https://docs.github.com/en/packages/learn-github-packages/about-github-packages

https://docs.github.com/en/packages/guides/configuring-dotnet-cli-for-use-with-github-packages

-------

# NuGet / MSBuild | Clever Title ???

- dotnet new classlib --framework netstandard2.1
- dotnet pack --configuration Release
- dotnet nuget push (error: Please specify the path to the package.)
- dotnet nuget push bin/Release/Temp.1.0.0.nupkg  (error: Source parameter was not specified.)
- dotnet nuget push bin/Release/Temp.1.0.0.nupkg --source "Local Feed" (error: The specified source 'Local Feed' is invalid. Provide a valid source.)
- dotnet nuget add source ./packages --name "Local Feed"
- dotnet nuget push bin/Release/Temp.1.0.0.nupkg --source "Local Feed"
    - Pushing Temp.1.0.0.nupkg to '/Users/maxhamulyak/.nuget/NuGet/packages'...
    - error: Could not find a part of the path '/Users/maxhamulyak/.nuget/NuGet/packages/Temp.1.0.0.nupkg'.
- dotnet new nugetconfig
- dotnet nuget add source ./packages --name "Local Feed"
- dotnet nuget add source ./packages --name "Local Feed" (error: The name specified has already been added to the list of available package sources. Provide a unique name.)
- dotnet nuget push bin/Release/Temp.1.0.0.nupkg --source "Local Feed"
    - Pushing Temp.1.0.0.nupkg to '/Users/maxhamulyak/Downloads/Development/Temp/packages'...
    - error: Could not find a part of the path '/Users/maxhamulyak/Downloads/Development/Temp/packages/Temp.1.0.0.nupkg'.
- mkdir packages
- dotnet nuget push bin/Release/Temp.1.0.0.nupkg --source "Local Feed"
    - Pushing Temp.1.0.0.nupkg to '/Users/maxhamulyak/Downloads/Development/Temp/packages'...
    - Your package was pushed.

-------

https://github.com/NuGetPackageExplorer/NuGetPackageExplorer 


`<GeneratePackageOnBuild>true</GeneratePackageOnBuild>`

## Step ? Create a NuGet Config.

For any project I do I create a NuGet Config in the root of my project. By doing so I make sure that any NuGet configuration on my machine does not influence the build result. So if I have a private nuget feed configured for my project, its right there in my project instead of hidden in some global nuget config.

You can create a `nuget.config` by running `dotnet new nugetconfig` Which looks likes the sample below when created.

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <!--To inherit the global NuGet package sources remove the <clear> line below -->
    <clear />
    <add key="nuget" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
```

The cool thing is that you don't need a server to try out new packages. NuGet supports the use of a local feed. You can create a local feed by running `dotnet nuget add source ./packages --name "Local Feed"` whichs adds the following line to the package sources in the NuGet config. `<add key="Local Feed" value="./packages" />`

----------

# Arcade

> This repository is based on https://github.com/dotnet/arcade and customizations from https://github.com/dotnet/runtime

## Reading Material

dotnet msbuild -pp:fullproject.xml
https://github.com/maxhamulyak/nswag-test/blob/master/Directory.Build.props

- https://github.com/jerriepelser-blog/AnalyzeDotNetProject/blob/master/Program.cs
- https://github.com/dotnet/docs/pull/22277
- https://webcache.googleusercontent.com/search?q=cache:zgvWC1R5e98J:https://docs.microsoft.com/en-us/dotnet/core/tools/csproj+&cd=2&hl=nl&ct=clnk&gl=nl&client=safari
- https://github.com/dotnet/arcade/pull/6798/files
- https://github.com/dotnet/iot/blob/master/Directory.Build.props
- https://github.com/dotnet/arcade/commit/c60e650b76eb71dfc63d74152fa5a1da8d83f02c
- 
- https://devblogs.microsoft.com/dotnet/the-evolving-infrastructure-of-net-core/
- https://devblogs.microsoft.com/dotnet/a-deep-dive-into-how-net-builds-and-ships/
- https://docs.microsoft.com/en-us/dotnet/core/project-sdk/overview
- https://docs.microsoft.com/en-us/visualstudio/msbuild/customize-your-build?view=vs-2019
- https://docs.microsoft.com/en-us/dotnet/core/project-sdk/msbuild-props
- https://docs.microsoft.com/en-us/visualstudio/msbuild/common-msbuild-project-properties?view=vs-2019


## Customize Package Assets

### Package Icon

| Element | Outputs |
| - | - |
| `<PackageIcon>` | The name of the icon file in the NuGet Package. |
| `<PackageIconFullPath>` | Resolves to `SDK/Assets/DotNetPackageIcon.png`.|

### Package License

| Element | Outputs |
| - | - |
| `<PackageLicenseFile>` | The name of the license file in the NuGet Package. |
| `<PackageLicenseFileFullPath>` | The full path to the LICENSE file, tries to resolve via `$(RepoRoot)/License.txt` or defaults to `SDK/Assets/License.txt`. |
