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








=====================
# NugetMetadata

Based on https://docs.microsoft.com/en-us/nuget/create-packages/creating-a-package-dotnet-cli

https://docs.microsoft.com/en-us/nuget/consume-packages/finding-and-choosing-packages#license-url-deprecation
https://licenses.nuget.org/MIT


## Package without Metadata

dotnet pack

```output
Microsoft (R) Build Engine version 16.8.3+39993bd9d for .NET
Copyright (C) Microsoft Corporation. All rights reserved.

  Determining projects to restore...
  All projects are up-to-date for restore.
  Kaylumah.Logging.Extensions.Abstractions -> C:\Projects\NugetMetadata\src\Kaylumah.Logging.Extensions.Abstractions\bin\Debug\netstandard2.0\Kaylumah.Logging.Extensions.Abstractions.dll
  Successfully created package 'C:\Projects\NugetMetadata\src\Kaylumah.Logging.Extensions.Abstractions\bin\Debug\Kaylumah.Logging.Extensions.Abstractions.1.0.0.nupkg'.
```

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>netstandard2.0</TargetFramework>
  </PropertyGroup>

</Project>
```

![no-metadata](images/vs2019_nometadata.png)

## Package with author Metadata

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

![author-metadata](images/vs2019_author_metadata.png)

## Package with Icon/License information

dotnet pack

```output
Microsoft (R) Build Engine version 16.8.3+39993bd9d for .NET
Copyright (C) Microsoft Corporation. All rights reserved.

  Determining projects to restore...
  All projects are up-to-date for restore.
  Kaylumah.Logging.Extensions.Abstractions -> C:\Projects\NugetMetadata\src\Kaylumah.Logging.Extensions.Abstractions\bin\Debug\netstandard2.0\Kaylumah.Logging.Extensions.Abstractions.dll
C:\Program Files\dotnet\sdk\5.0.103\Sdks\NuGet.Build.Tasks.Pack\build\NuGet.Build.Tasks.Pack.targets(207,5): error NU5046: The icon file 'Logo.png' does not exist in the package. [C:\Projects\NugetMetadata\src\Kaylumah.Logging.Extensions.Abstractions\Kaylumah.Logging.Extensions.Abstractions.csproj]
```

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

## Package via Directory.Build.Props

![](images/vs2019_metadata_partial_buildprops.png)