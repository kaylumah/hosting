---
title: 'Share NuGet packages via local feed.'
description: ''
---
NuGet is the Microsoft-supported package manager for .NET. A NuGet package is a single zip file with the `.nupkg` extension.
It contains the dlls and other files related to the code, such as a descriptive manifest.

NuGet package can be shared via both public and private hosts. This way you can share code that is exclusive to your organization. In fact the package is just a shareable unit of code, but the means of sharing is up to you. For example, you might want to use a local copy of a package to test it in a project before sharing it with others.

Today we are looking at local package feeds.


-------

https://docs.microsoft.com/en-us/nuget/what-is-nuget

https://en.wikipedia.org/wiki/NuGet


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