---
title: 'Set NuGet metadata via MSBuild'
description: 'TODO Write summary'
cover_image: '/assets/images/posts/20210321/nuget-metadata/cover_image.png'
tags:
    - MSBuild
    - NuGet
---

Nuget packages are the way to share bundles of your code with other developers. A `.nupkg` file is basically an archive that contains your dll's and metadata about your code. Most commonly this is data relating to owernship/copyright and data about the repository where the package originated.

## Scenario

Starting with .netcore Microsoft started with a library of extension points called `Microsoft.Extensions`. This library consists of interfaces and some implementations for common things like logging or dependency injection. Keeping up with that trend we decided that we can do something similiar and want to share our code with the world.

For this demo I am using the [.NET CLI](https://docs.microsoft.com/en-us/dotnet/core/tools/) to create my project. Since the CLI is not the focus of this post I am not going to delve deeply in the inner workings of the tool. The important bit is that you end up with a `classlibrary` project, so if you prefer to use VisualStudio to accomplish this go ahead and do so.

The following commands are grouped for convenience.

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

For the purpose of this demo it is fine that we don't create any implementation. In the real world of course you would also populate the code.

## Package Metadata

I said that we were going to enrich the package with MSBuild data. Before we do that I want to show you what you get before we changed anything for the defaults.

```shell
$ dotnet pack

Microsoft (R) Build Engine version 16.8.3+39993bd9d for .NET
Copyright (C) Microsoft Corporation. All rights reserved.

  Determining projects to restore...
  All projects are up-to-date for restore.
  Kaylumah.Logging.Extensions.Abstractions -> C:\Projects\NugetMetadata\src\Kaylumah.Logging.Extensions.Abstractions\bin\Debug\netstandard2.0\Kaylumah.Logging.Extensions.Abstractions.dll
  Successfully created package 'C:\Projects\NugetMetadata\src\Kaylumah.Logging.Extensions.Abstractions\bin\Debug\Kaylumah.Logging.Extensions.Abstractions.1.0.0.nupkg'.
```

The package gets created in the bin folder, since the default configuration is `Debug` our file is located under `bin/Debug`. We have a couple of ways to look at the resulting package. My prefered way is to use [NuGetPackageExplorer](https://github.com/NuGetPackageExplorer/NuGetPackageExplorer), unfortunatly that is only available on windows.

If we open `Kaylumah.Logging.Extensions.Abstractions.1.0.0.nupkg` it looks like the screenshot below.

![initial metadata in nuget package explorer](/assets/images/posts/20210321/nuget-metadata/npe_initial_metadata.png)

It does not look very promising yet. Similar we can use Visual Studio to look at the data that is already set.
It looks like PackageId, Authors, Company and Product all default to the assembly name.

![initial metadata](/assets/images/posts/20210321/nuget-metadata/vs2019_initial_metadata.png)

### Set data from csproj

#### The basics

So how do we change the fields? What other fields can we change? For the full list of mappings I reffer you to [Nuget MSBuld Targets](https://docs.microsoft.com/en-us/nuget/reference/msbuild-targets#pack-target). For now we will focus on some common fields like who authored the package, and what copyright applies.

The only thing we need to do is modify the project file `src\Kaylumah.Logging.Extensions.Abstractions\Kaylumah.Logging.Extensions.Abstractions.csproj`.
Add the properties you want to change to the existing PropertyGroup, or create a second one under the `Project` node. For example if I were to create a package it would look something like this.

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

![initial metadata](/assets/images/posts/20210321/nuget-metadata/vs2019_author_metadata.png)
![initial metadata](/assets/images/posts/20210321/nuget-metadata/npe_author_metadata.png)

#### Including License / Branding

TODO write text...

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



##### Alternative License

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
    <PackageLicenseExpression>MIT</PackageLicenseExpression>
  </PropertyGroup>

  <ItemGroup>
    <None Include="Logo.png" Pack="true" PackagePath="" />
  </ItemGroup>

</Project>
```

![initial metadata](/assets/images/posts/20210321/nuget-metadata/vs2019_licenseexpression_metadata.png)
![initial metadata](/assets/images/posts/20210321/nuget-metadata/npe_licenseexpression_metadata.png)

### Set data for multiple projects

So the approach I demonstrated works fine if you have a single project. But for a moment suppose you want to release multiple packages from a single repo. You can of course do this on a per project basis, but maintaining it would become annoying really fast. Luckily we have the full power of MSBuild at our command. Let's say you want to set the same Copyright notice for all projects in the solution. Remove the `<Copyright>` tag from the csproj file, and create a new file called `Directory.Build.props` in the root of your repository.

The build props file follows the same syntax we know from our csproj files. So we can add a PropertyGroup and add the Copyright tag to it. This time we will reference the Company tag, and use MSBuild to set the year. I don't know about you, but I prefer to not update the copyright every year.
This would look like this:

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project>
    <PropertyGroup>
        <Copyright>Copyright © $(Company) $([System.DateTime]::Now.Year)</Copyright>
    </PropertyGroup>
</Project>
```

If we create the package again (`dotnet pack`) it would look like this:

![initial metadata](/assets/images/posts/20210321/nuget-metadata/npe_buildpropsv1.png)

Something went wrong here. Why is it just showing `Copyright © 2021` instead of `Copyright © Kaylumah 2021` as expected. This is because of how MSBuild operates. I will demonstrate with another example. For this we also set the `Company` tag in `Directory.Build.props` just before the `Copyright` tag. But this time keep it in the `.csproj` file. This time I explicitly set it to something other than my actual company name.

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project>
    <PropertyGroup>
        <Company>NotKaylumah</Company>
        <Copyright>Copyright © $(Company) $([System.DateTime]::Now.Year)</Copyright>
    </PropertyGroup>
</Project>
```

![initial metadata](/assets/images/posts/20210321/nuget-metadata/npe_buildpropsv2.png)

As you can see the csproj overwrites the value

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

![initial metadata](/assets/images/posts/20210321/nuget-metadata/vs2019_buildpropsv3.png)
![initial metadata](/assets/images/posts/20210321/nuget-metadata/npe_buildpropsv3.png)

For more details see [here](https://docs.microsoft.com/en-us/visualstudio/msbuild/customize-your-build?view=vs-2019)







### Repo Info

When looking [here](https://docs.microsoft.com/en-us/nuget/reference/msbuild-targets#pack-target) some other intressting fields caught my eye.

| NuSpec | MSBuild | Description |
| - | - | - |
| Repository/Url | RepositoryUrl | URL where sourcecode is located i.e. https://github.com/NuGet/NuGet.Client.git |
| Repository/Type | RepositoryType | The repository type i.e. git |
| Repository/Branch | RepositoryBranch | Optional repository branch info i.e. main |
| Repository/Commit | RepositoryCommit | Optional commit information |






```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>netstandard2.0</TargetFramework>
    <GeneratePackageOnBuild>true</GeneratePackageOnBuild>
  </PropertyGroup>

</Project>
```

```shell
#!/bin/sh -x

REPO_URL=$(git config --get remote.origin.url)
REPO_BRANCH=$(git branch --show-current)
REPO_COMMIT=$(git rev-parse HEAD)
dotnet build -p:RepositoryUrl="$REPO_URL" -p:RepositoryBranch="$REPO_BRANCH" -p:RepositoryCommit="$REPO_COMMIT" -p:RepositoryType="git"
```

### SourceLink

While it is certainly possible to do it like above, there is an alternative that I prefer. It goes further than just setting some values. It is called [sourcelink](https://github.com/dotnet/sourcelink). Just like we used `Directory.Build.props` to add metadata to every project file in the solution, we can also add nuget packages to every project.
This time we use `Directory.Build.targets` which runs after the project. Since I host my projects on GitHub I use the `Microsoft.SourceLink.GitHub` package.


```xml
<?xml version="1.0" encoding="utf-8"?>
 <Project>
     <ItemGroup>
         <PackageReference Include="Microsoft.SourceLink.GitHub" Version="1.0.0" PrivateAssets="all" IsImplicitlyDefined="true" />
     </ItemGroup>
 </Project>
```

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

![initial metadata](/assets/images/posts/20210321/nuget-metadata/vs2019_buildpropsv2.png)
![initial metadata](/assets/images/posts/20210321/nuget-metadata/npe_buildpropsv2.png)

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

https://docs.microsoft.com/en-us/nuget/consume-packages/finding-and-choosing-packages#license-url-deprecation
https://licenses.nuget.org/MIT
