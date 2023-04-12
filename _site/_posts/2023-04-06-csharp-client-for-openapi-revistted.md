---
title: Generate C# client for OpenAPI
description: A look at using OpenAPI clients in C#
image: /assets/images/posts/20210523/generate-csharp-client-for-openapi/cover_image.png
tags:
  - csharp
  - nswag
  - openapi
  - swashbuckle
commentid: '143'
---
In my previous article, ["Adventures with Mock"](https://kaylumah.nl/2021/04/11/an-approach-to-writing-mocks.html)




# Generating Specification on build
In this post, I won't detail setting up an API that offers the OpenAPI specification. The starter web api template (dotnet new webapi via the command line) is sufficient for our cases. I will, however, explain how to generate the specification as part of every build. The template uses `Swashbuckle.AspNetCore` for setting up all the openapi stuff. We can use a corresponding  swashbuckle cli tool for interacting with our build system. The version needs to be the same, so in my case

```
dotnet tool install --local Swashbuckle.AspNetCore.Cli --version 6.4.0
```

We can now change the project file and add a custom MSBuild target. For the command to work you need a build DLL and the name of of the api document. We take care of the dll bit by specifying `AfterTargets="Build"` and the document name has a default name "v1"

```xml
<Target Name="Generate OpenAPI Specification Document" AfterTargets="Build">
  <PropertyGroup>
    <OpenApiDocumentName>v1</OpenApiDocumentName>
  </PropertyGroup>
  <Exec Command="dotnet swagger tofile --output $(OutputPath)$(AssemblyName).json $(OutputPath)$(AssemblyName).dll $(OpenApiDocumentName)" ContinueOnError="true" />
</Target>
```