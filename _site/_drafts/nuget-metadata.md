---
name: 'Setup NuGet with different publishing sources'
description: "It was a dog. It was a big dog. Does it come in black? My anger outweights my guilt. I'll be standing where l belong. Between you and the peopIe of Gotham."
---

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <!--To inherit the global NuGet package sources remove the <clear/> line below -->
    <clear />
    <add key="nuget" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
```








NuGet Package Sources
The cool thing Microsoft did with NuGet packages that you can share code without the use of nuget.org. If you so desire, you can use other registries such as https://docs.microsoft.com/en-us/azure/devops/artifacts/get-started-nuget?view=azure-devops or https://docs.github.com/en/packages/learn-github-packages/about-github-packages. The cool thing is that if you want to share code between private repositories is also possible with the concept of local packages.

Setup your NuGet sources
Package sources are called feeds. These feeds can be registered globally on your machine or as part of your project via NuGet.config.
You can create a NuGet.config using the dotnet cli by running "dotnet new nugetconfig" Pay attention to line 4-5. It is possible to inherrit global configuration, but imo this makes the "runs on my machine" 



Local Source
A local source 























![structure](https://docs.microsoft.com/en-us/nuget/media/nuget-roles.png)















There are a couple of ways code can be shared with other developers. One of those is to distribute code via packages, which are bundles containing your code. For .NET the most common way of sharing packages is via NuGet.

A NuGet package is more than just the dlls.

https://github.com/NuGet/docs.microsoft.com-nuget/blob/main/docs/quickstart/create-and-publish-a-package-using-the-dotnet-cli.md

https://docs.microsoft.com/en-us/nuget/reference/msbuild-targets#pack-target
https://docs.microsoft.com/en-us/nuget/quickstart/create-and-publish-a-package-using-the-dotnet-cli

### Local

### GitHub

### Azure Devops

### Nuget.org

https://docs.microsoft.com/en-us/nuget/create-packages/overview-and-workflow







### SourceLink

While it is certainly possible to do it like above, there is an alternative that I prefer. It goes further than just setting some values. It is called [sourcelink](). Just like we used `Directory.Build.props` to add metadata to every project file in the solution, we can also add nuget packages to every project.
This time we use `Directory.Build.targets` which runs after the project. Since I host my projects on GitHub I use the `Microsoft.SourceLink.GitHub` package.























TODO (Screenshots)
 - https://www.nuget.org/packages/Microsoft.Extensions.Logging.Console/3.1.13
 - https://www.nuget.org
 - https://www.fuget.org/packages/Microsoft.Extensions.Logging.Console/3.1.13
 - NuGetPackageExplorer

---


> TODO include error section 
#

dotnet pack

```output
Microsoft (R) Build Engine version 16.8.3+39993bd9d for .NET
Copyright (C) Microsoft Corporation. All rights reserved.

  Determining projects to restore...
  All projects are up-to-date for restore.
  Kaylumah.Logging.Extensions.Abstractions -> C:\Projects\NugetMetadata\src\Kaylumah.Logging.Extensions.Abstractions\bin\Debug\netstandard2.0\Kaylumah.Logging.Extensions.Abstractions.dll
C:\Program Files\dotnet\sdk\5.0.103\Sdks\NuGet.Build.Tasks.Pack\build\NuGet.Build.Tasks.Pack.targets(207,5): error NU5046: The icon file 'Logo.png' does not exist in the package. [C:\Projects\NugetMetadata\src\Kaylumah.Logging.Extensions.Abstractions\Kaylumah.Logging.Extensions.Abstractions.csproj]
```


Based on https://docs.microsoft.com/en-us/nuget/create-packages/creating-a-package-dotnet-cli




 


>