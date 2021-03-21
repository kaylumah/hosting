---
title: 'Set NuGet metadata via MSBuild'
description: '...'
---

TODO write short introducition...

## Setup example project

We are going to create a new project via the dotnet cli.

```shell
dotnet new sln
```

```output
The template "Solution File" was created successfully.
```

Add a classlibrary to the project.

```shell
dotnet new classlib --framework netstandard2.0 --output src/Kaylumah.Logging.Extensions.Abstractions
```

```output
The template "Class library" was created successfully.

Processing post-creation actions...
Running 'dotnet restore' on src/Kaylumah.Logging.Extensions.Abstractions\Kaylumah.Logging.Extensions.Abstractions.csproj...
  Determining projects to restore...
  Restored C:\Projects\NugetMetadata\src\Kaylumah.Logging.Extensions.Abstractions\Kaylumah.Logging.Extensions.Abstractions.csproj (in 84 ms).
Restore succeeded.
```

```shell
dotnet sln add src/Kaylumah.Logging.Extensions.Abstractions/Kaylumah.Logging.Extensions.Abstractions.csproj
```

```output
Project `src\Kaylumah.Logging.Extensions.Abstractions\Kaylumah.Logging.Extensions.Abstractions.csproj` added to the solution.
```

## Setting Metadata

### Defaults

For starters let see what we get by default

```shell
dotnet pack
```

```output
Microsoft (R) Build Engine version 16.8.3+39993bd9d for .NET
Copyright (C) Microsoft Corporation. All rights reserved.

  Determining projects to restore...
  All projects are up-to-date for restore.
  Kaylumah.Logging.Extensions.Abstractions -> C:\Projects\NugetMetadata\src\Kaylumah.Logging.Extensions.Abstractions\bin\Debug\netstandard2.0\Kaylumah.Logging.Extensions.Abstractions.dll
  Successfully created package 'C:\Projects\NugetMetadata\src\Kaylumah.Logging.Extensions.Abstractions\bin\Debug\Kaylumah.Logging.Extensions.Abstractions.1.0.0.nupkg'.
```

![initial metadata](/assets/images/posts/20210321/nuget-metadata/vs2019_initial_metadata.png)
![initial metadata](/assets/images/posts/20210321/nuget-metadata/npe_initial_metadata.png)

### Setting data...

For a complete list of mappings see [Nuget MSBuld Targets](https://docs.microsoft.com/en-us/nuget/reference/msbuild-targets#pack-target).

In this example I am only setting a couple of the values.

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

#### Including License / Branding

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

![initial metadata](/assets/images/posts/20210321/nuget-metadata/vs2019_author_metadata.png)
![initial metadata](/assets/images/posts/20210321/nuget-metadata/npe_author_metadata.png)

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

### Directory.Build.props

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project>
    <PropertyGroup>
        <Copyright>Copyright © $(Company) $([System.DateTime]::Now.Year)</Copyright>
    </PropertyGroup>
</Project>
```

![initial metadata](/assets/images/posts/20210321/nuget-metadata/vs2019_buildpropsv1.png)
![initial metadata](/assets/images/posts/20210321/nuget-metadata/npe_buildpropsv1.png)

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project>
    <PropertyGroup>
        <Company>NotKaylumah</Company>
        <Copyright>Copyright © $(Company) $([System.DateTime]::Now.Year)</Copyright>
    </PropertyGroup>
</Project>
```

![initial metadata](/assets/images/posts/20210321/nuget-metadata/vs2019_buildpropsv2.png)
![initial metadata](/assets/images/posts/20210321/nuget-metadata/npe_buildpropsv2.png)

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


=====================
# NugetMetadata

Based on https://docs.microsoft.com/en-us/nuget/create-packages/creating-a-package-dotnet-cli

https://docs.microsoft.com/en-us/nuget/consume-packages/finding-and-choosing-packages#license-url-deprecation
https://licenses.nuget.org/MIT

### Repo Info

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