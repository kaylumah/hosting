---
name: 'Setup NuGet with different publishing sources'
description: '...'
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

https://docs.microsoft.com/en-us/nuget/reference/msbuild-targets#pack-target
https://docs.microsoft.com/en-us/nuget/quickstart/create-and-publish-a-package-using-the-dotnet-cli

### Local

### GitHub

### Azure Devops

### Nuget.org

https://docs.microsoft.com/en-us/nuget/create-packages/overview-and-workflow