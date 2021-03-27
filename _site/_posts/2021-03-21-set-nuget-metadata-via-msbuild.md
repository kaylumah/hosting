---
title: 'Set NuGet metadata via MSBuild'
description: "Discover how to use MSBuild to set your NuGet package's metadata."
cover_image: '/assets/images/posts/20210321/nuget-metadata/cover_image.png'
image: '/assets/images/posts/20210321/nuget-metadata/cover_image.png'
tags:
    - MSBuild
    - NuGet
---
For .NET, the standard mechanism for sharing packages is NuGet. A `.nupkg` file is an archive that contains your compiled code (DLLs), other files related to your code, and a manifest containing metadata ([source](https://docs.microsoft.com/en-us/nuget/what-is-nuget)). This blog post will show you how data in this manifest can be controlled by using MSBuild.

For simplification purposes, my sample project will consist of only a single class library project. I like you to keep in mind that this would scale to many projects as Microsoft did with the ["Microsoft.Extensions packages"](https://github.com/dotnet/runtime).The sky is the limit.

## Setup

There are bits of this demo that work cross-platform and bits that require you to run on Windows. For example, I like the control the [.NET CLI](https://docs.microsoft.com/en-us/dotnet/core/tools/) gives me when creating a new project. If you prefer to use [Visual Studio](https://visualstudio.microsoft.com/vs/), the result will remain the same.

```shell
$ dotnet new sln

The template "Solution File" was created successfully.

$ dotnet new classlib --framework netstandard2.0 --output src/Kaylumah.Logging.Extensions.Abstractions

The template "Class library" was created successfully.

Processing post-creation actions...
Running 'dotnet restore' on src/Kaylumah.Logging.Extensions.Abstractions\Kaylumah.Logging.Extensions.Abstractions.csproj...
  Determining projects to restore...
  Restored C:\Projects\NugetMetadata\src\Kaylumah.Logging.Extensions.Abstractions\Kaylumah.Logging.Extensions.Abstractions.csproj (in 84 ms).
Restore succeeded.

$ dotnet sln add src/Kaylumah.Logging.Extensions.Abstractions/Kaylumah.Logging.Extensions.Abstractions.csproj

Project `src\Kaylumah.Logging.Extensions.Abstractions\Kaylumah.Logging.Extensions.Abstractions.csproj` added to the solution.
```

I chose `Kaylumah.Logging.Extensions.Abstractions` to keep inline and in style with the extension packages Microsoft provides. By default, the namespace of the assembly sets the unique package identifier. Of course, this only matters when publishing the package to a NuGet source like `https://nuget.org`. That is not this article's scope, as publishing the default template with only the empty `Class1.cs` file would not benefit anyone by sharing it.

## Why do we even need metadata in our packages?

Before showing you how I set metadata, I like to show you what happens without specifying any metadata. You can run the command [`dotnet pack`](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-pack#description) for a single project or an entire solution. If you do it for the solution, only projects that are `<IsPackable>true</IsPackable>` generate a package. The class library we created uses the `Microsoft.NET.Sdk` and is packable by default.

```shell
$ dotnet pack

Microsoft (R) Build Engine version 16.8.3+39993bd9d for .NET
Copyright (C) Microsoft Corporation. All rights reserved.

  Determining projects to restore...
  All projects are up-to-date for restore.
  Kaylumah.Logging.Extensions.Abstractions -> C:\Projects\NugetMetadata\src\Kaylumah.Logging.Extensions.Abstractions\bin\Debug\netstandard2.0\Kaylumah.Logging.Extensions.Abstractions.dll
  Successfully created package 'C:\Projects\NugetMetadata\src\Kaylumah.Logging.Extensions.Abstractions\bin\Debug\Kaylumah.Logging.Extensions.Abstractions.1.0.0.nupkg'.
```

This command generated the package in my bin folder. Since I did not specify a configuration, it chose the default configuration, which is Debug. So how do we inspect `Kaylumah.Logging.Extensions.Abstractions.1.0.0.nupkg`? My prefered way is the [NuGet Package Explorer](https://github.com/NuGetPackageExplorer/NuGetPackageExplorer), which is unfortunately only available on Windows.

![Without Metadata in Package Explorer](/assets/images/posts/20210321/nuget-metadata/001_npe_initial_metadata.png)

There seems to be no metadata set by default. Let's, for a quick moment, compare it to what Microsoft adds to its packages. We can do this by downloading [the package](https://www.nuget.org/api/v2/package/Microsoft.Extensions.Logging.Console/3.1.13) from nuget.org and view it like we just did for `Kaylumah.Logging.Extensions.Abstractions.1.0.0.nupkg`. Alternatively, the NuGet Package Explorer also supports viewing metadata from remote sources such as nuget.org.

![Microsoft Extensions Logging Metadata in Package Explorer](/assets/images/posts/20210321/nuget-metadata/002_console_logger_info.png)

Now that is what I call metadata. Remember that `.nupkg` files are archives; this means we can easily verify what the explorer was telling us about our package.  You can do this by changing the extension from `.nupkg` to `.zip` and then extracting it. It contains `Kaylumah.Logging.Extensions.Abstractions.nuspec`, which is the manifest I was talking about in the introduction. At the moment, it looks like this:

```xml
<?xml version="1.0" encoding="utf-8"?>
<package xmlns="http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd">
  <metadata>
    <id>Kaylumah.Logging.Extensions.Abstractions</id>
    <version>1.0.0</version>
    <authors>Kaylumah.Logging.Extensions.Abstractions</authors>
    <requireLicenseAcceptance>false</requireLicenseAcceptance>
    <description>Package Description</description>
    <dependencies>
      <group targetFramework=".NETStandard2.0" />
    </dependencies>
  </metadata>
</package>
```

So as expected, it matches what NuGet Package Explorer shows us. The default for both id and authors is the assembly namespace, whereas description defaults to "Package Description", which tells our users nothing about what the package does.

## How do we set metadata?

Now that we have covered our basis, we can finally explain how we can set metadata via MSBuild.

### Set metadata from csproj

Since we are working on a single project, the logical place to set metadata is by editing our .csproj file. I will not cover every property today, so I refer you to [this](https://docs.microsoft.com/en-us/nuget/reference/msbuild-targets#pack-target) link. I will, however, cover properties I often use in my projects.

So behind the scenes, what happens is that specific MSBuild properties map to properties in the .nuspec file. We have to either edit the existing `PropertyGroup` in our file or add one to set properties. In my opinion, every package should contain branding (like authors, company and copyright information), a helpful description and categorized by a series of tags. So in the example below, I have set these values.

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>netstandard2.0</TargetFramework>
    <Authors>Max Hamulyák</Authors>
    <Company>Kaylumah</Company>
    <Description>Logging abstractions for Kaylumah.</Description>
    <PackageTags>logging;abstractions</PackageTags>
    <Copyright>Copyright (c) 2021 Kaylumah</Copyright> 
  </PropertyGroup>
</Project>
```

If we run `dotnet pack` now, we can immediately see that our package no longer has empty metadata.

![With Author Metadata in Package Explorer](/assets/images/posts/20210321/nuget-metadata/003_npe_author_metadata.png)

You can also verify this in Visual Studio by checking your projects properties and clicking on the `Package` tab.

![With Author Metadata in VS2019](/assets/images/posts/20210321/nuget-metadata/004_vs2019_author_metadata.png)

In the introduction, I talked about what exactly is a NuGet package. We are now at the part regarding other files. Since we already took care of branding, let us also add an icon. Our code is under license; how do we include it in the package?

Add files named `Logo.png` and `LICENSE` to the folder containing our project. We can then use the tags `PackageIcon` and `PackageLicenseFile` respectfully. We also need to tell MSBuild that these files should be part of the package. The updated project file looks like this:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>netstandard2.0</TargetFramework>
    <Authors>Max Hamulyák</Authors>
    <Company>Kaylumah</Company>
    <Description>Logging abstractions for Kaylumah.</Description>
    <PackageTags>logging;abstractions</PackageTags>
    <Copyright>Copyright (c) 2021 Kaylumah</Copyright>
    <PackageIcon>Logo.png</PackageIcon>
    <PackageLicenseFile>LICENSE</PackageLicenseFile>
  </PropertyGroup>

  <ItemGroup>
    <None Include="Logo.png" Pack="true" PackagePath="" />
    <None Include="LICENSE" Pack="true" PackagePath=""/>
  </ItemGroup>

</Project>
```

![initial metadata](/assets/images/posts/20210321/nuget-metadata/005_npe_includingfiles_metadata.png)


### Set metadata for multiple projects

So lets for a moment, assume our project is a huge success. We are creating more and more extension libraries. Think about the vast number of packages in `dotnet/runtime`. Even if we would only include an implementation for `.Abstractions` package, it would be very time consuming to do this for every project. It would also violate the [DRY principle](https://en.wikipedia.org/wiki/Don%27t_repeat_yourself).

To get started, create a file called `Directory.Build.props` at the root of your solution. The way Microsoft handles this file, and in precisely that casing, is starting from your project folder; it goes up till it finds a match or it reaches the root of your drive. This `Directory.Build.props` file follows the same syntax we use in our `.csproj` files. To demonstrate, remove only the `Copyright` tag from the project and recreate it in the `Directory.Build.props` file. Now is the perfect moment to also demonstrate something I have not yet told you. We are using MSBuild to populate our metadata, and thus we can use the full force of MSBuild. For example, we can reference other variables and even use built-in functions. So the thing about our current Copyright implementation is that if after `31/12/2021` I want to release the next version, I have to remember to update my copyright notice. We can achieve this by setting the copyright tag like below.

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project>
    <PropertyGroup>
        <Copyright>Copyright © $(Company) $([System.DateTime]::Now.Year)</Copyright>
    </PropertyGroup>
</Project>
```

![initial metadata](/assets/images/posts/20210321/nuget-metadata/006_npe_buildpropsv1.png)

What happened? Something is wrong; why do I see the copyright year 2021, but not my company name? Before explaining it, let me prove it by adding a company tag to the `Directory.Build.props` with a different value. For example:

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project>
    <PropertyGroup>
        <Company>NotKaylumah</Company>
        <Copyright>Copyright © $(Company) $([System.DateTime]::Now.Year)</Copyright>
    </PropertyGroup>
</Project>
```

This time do not remove the tag from the `.csproj` file. The result, this time, is a little different.

![initial metadata](/assets/images/posts/20210321/nuget-metadata/007_npe_buildpropsv2.png)

Now it appears that I have two different values for `Company`; this happens because `Directory.Build.props` gets imported before your project, and `Directory.Build.targets` gets imported after. The latest registration wins. That is why the value for `Company` is "Kaylumah", but when we set `Copyright`, it is still "NotKaylumah". You can verify this behaviour by running the preprocess command (`dotnet build -pp:fullproject.xml`). See [here](https://docs.microsoft.com/en-us/visualstudio/msbuild/msbuild-command-line-reference?view=vs-2019) for an explanation.

> Word of caution, you should not set every property this way. You should only set the values that are shared cross-project. For example, `Company` and `Copyright` are likely to be the same for every project. The `Authors` and `PackageTags` could be project-specific; heck, even `Description` could be reused if so desired. One thing for sure is that `Id` can not be recycled since every package requires a unique Id.

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project>
    <PropertyGroup>
        <Authors>Max Hamulyák</Authors>
        <Company>Kaylumah</Company>
        <Description>Logging abstractions for Kaylumah.</Description>
        <Copyright>Copyright © $(Company) $([System.DateTime]::Now.Year)</Copyright>
        <PackageTags>logging;abstractions</PackageTags>
        <PackageIcon>Logo.png</PackageIcon>
        <PackageLicenseFile>LICENSE</PackageLicenseFile>
    </PropertyGroup>

    <ItemGroup>
        <None Include="$(MSBuildThisFileDirectory)Logo.png" Pack="true" PackagePath="" />
        <None Include="$(MSBuildThisFileDirectory)LICENSE" Pack="true" PackagePath="" />
    </ItemGroup>

</Project>
```

![initial metadata](/assets/images/posts/20210321/nuget-metadata/008_npe_buildpropsv3.png)

### Bonus Chapter

I have referred to the list of properties before. There are a couple of handy ones we have not yet discussed. I am talking about the repository fields, making sure that an artefact can always trace back to a specific revision of your source code.

| NuSpec | MSBuild | Description |
| - | - | - |
| Repository/Url | RepositoryUrl | URL where sourcecode is located i.e. `https://github.com/NuGet/NuGet.Client.git` |
| Repository/Type | RepositoryType | The repository type i.e. `git` |
| Repository/Branch | RepositoryBranch | Optional repository branch info i.e. `main` |
| Repository/Commit | RepositoryCommit | Optional commit information i.e. `0e4d1b598f350b3dc675018d539114d1328189ef` |

Before I explain this, I am getting a bit tired of running `dotnet pack` every time. Lucky for me, there is a way to generate a package on build. Update the `.csproj` file to look like this:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>netstandard2.0</TargetFramework>
    <GeneratePackageOnBuild>true</GeneratePackageOnBuild>
  </PropertyGroup>

</Project>
```

So back to repository info. MSBuild itself is not aware of things like source control. Fortunately, we can pass parameters from the outside to use inside MSBuild. For this, we have the `-p` or `-property` switch. The following script retrieves the URL, branch name and SHA1 hash from the current commit.

```shell
#!/bin/sh -x

REPO_URL=$(git config --get remote.origin.url)
REPO_BRANCH=$(git branch --show-current)
REPO_COMMIT=$(git rev-parse HEAD)
dotnet build -p:RepositoryUrl="$REPO_URL" -p:RepositoryBranch="$REPO_BRANCH" -p:RepositoryCommit="$REPO_COMMIT" -p:RepositoryType="git"
```

Remember, we now generate a package on build. Let us verify we see repo info by opening the created package in NuGet Package Explorer.

![initial metadata](/assets/images/posts/20210321/nuget-metadata/009_npe_repoinfo.png)

Even though it is OK to add repo metadata this way, there is a better alternative. This alternative does more than add metadata; it also enables source code debugging from NuGet packages. How cool is that? This technology is called [Source Link](https://github.com/dotnet/sourcelink).

Like before with the properties, I have no wish to add source link to every package separately. For this, create `Directory.Build.targets`, which looks like this:

```xml
<?xml version="1.0" encoding="utf-8"?>
 <Project>
     <ItemGroup>
         <PackageReference Include="Microsoft.SourceLink.GitHub" Version="1.0.0" PrivateAssets="all" IsImplicitlyDefined="true" />
     </ItemGroup>
 </Project>
```

To configure source link, we need to update `Directory.Build.props` as well.

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project>
    <PropertyGroup>
        <Authors>Max Hamulyák</Authors>
        <Company>Kaylumah</Company>
        <Description>Logging abstractions for Kaylumah.</Description>
        <Copyright>Copyright © $(Company) $([System.DateTime]::Now.Year)</Copyright>
        <PackageTags>logging;abstractions</PackageTags>
        <PackageIcon>Logo.png</PackageIcon>
        <PackageLicenseFile>LICENSE</PackageLicenseFile>
    </PropertyGroup>

    <ItemGroup>
        <None Include="$(MSBuildThisFileDirectory)Logo.png" Pack="true" PackagePath="" />
        <None Include="$(MSBuildThisFileDirectory)LICENSE" Pack="true" PackagePath="" />
    </ItemGroup>

    <PropertyGroup>
        <PublishRepositoryUrl>true</PublishRepositoryUrl>
        <EmbedUntrackedSources>true</EmbedUntrackedSources>
        <IncludeSymbols>true</IncludeSymbols>
        <SymbolPackageFormat>snupkg</SymbolPackageFormat>
    </PropertyGroup>

</Project>
```

To prove that it is still working here is the entire `.nuspec` file after adding Source Link

```xml
<?xml version="1.0" encoding="utf-8"?>
<package xmlns="http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd">
  <metadata>
    <id>Kaylumah.Logging.Extensions.Abstractions</id>
    <version>1.0.0</version>
    <authors>Max Hamulyák</authors>
    <requireLicenseAcceptance>false</requireLicenseAcceptance>
    <license type="file">LICENSE</license>
    <licenseUrl>https://aka.ms/deprecateLicenseUrl</licenseUrl>
    <icon>Logo.png</icon>
    <description>Logging abstractions for Kaylumah.</description>
    <copyright>Copyright © Kaylumah 2021</copyright>
    <tags>logging abstractions</tags>
    <repository type="git" url="https://github.com/Kaylumah/NugetMetadataDemo.git" commit="3378cf33e0061b234c1f58e060489efd81e08586" />
    <dependencies>
      <group targetFramework=".NETStandard2.0" />
    </dependencies>
  </metadata>
</package>
```

## Closing Thoughts

We looked at setting metadata via MSBuild and sharing metadata between projects. You can take this even further by using MSBuild tasks to verify that packages must have a description like [this](https://github.com/dotnet/arcade/blob/9a72efb067b74bb9147f9413ade6173b568ea1af/src/Microsoft.DotNet.Arcade.Sdk/tools/Workarounds.targets#L79). It is also possible to create an entire SDK as Microsoft did with [Arcade](https://github.com/dotnet/arcade). Of course, Arcade goes much further than just specifying some metadata. You can read about how / why Microsoft did that [here](https://devblogs.microsoft.com/dotnet/the-evolving-infrastructure-of-net-core/). I experimented with a custom SDK heavily inspired by Arcade, but that is a blog post for another day.
