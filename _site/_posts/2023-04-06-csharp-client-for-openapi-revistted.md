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




## Generating Specification on build
In this post, I won't detail setting up an API that offers the OpenAPI specification. The starter web api template (dotnet new webapi via the command line) is sufficient for our cases. I will, however, explain how to generate the specification as part of every build. The template uses `Swashbuckle.AspNetCore` for setting up all the openapi stuff. We can use a corresponding  swashbuckle cli tool for interacting with our build system. The version needs to be the same, so in my case

```
dotnet tool install --local Swashbuckle.AspNetCore.Cli --version 6.4.0
```

We can now change the project file and add a custom MSBuild target. For the command to work you need a build DLL and the name of of the api document. We take care of the dll bit by specifying `AfterTargets="Build"` and the document name has a default name "v1"

```xml
  <Target Name="Generate OpenAPI Specification Document" AfterTargets="Build">
    <PropertyGroup>
      <OpenApiDocumentName>v1</OpenApiDocumentName>
      <ApiDll>$(OutputPath)$(AssemblyName).dll</ApiDll>
      <OutputApiDocument>$(OutputPath)$(AssemblyName).json</OutputApiDocument>
    </PropertyGroup>
    <Exec Command="dotnet swagger tofile --output $(OutputApiDocument) $(ApiDll) $(OpenApiDocumentName)" ContinueOnError="true" />
  </Target>
```

In our case the variable `OutputPath` resolves as `bin/Debug/net7.0/` and `AssemblyName` to `Demo`.  After the project is build you will have a file named `bin/Debug/net7.0/Demo.json` which is the API specification for the project. For our purpose putting the file in the bin is sufficient, in some cases I do prefer to put the swagger specification under source control. This helps track unexpected changes to the API. 

## Use NSwag the classic way...

Fortunately not much has changed in the way we do it in the old way.

```
dotnet add package NSwag.MSBuild --version 13.18.2
```

```xml
<Target Name="NSwag" AfterTargets="PostBuildEvent" Condition=" '$(Configuration)' == 'Debug' ">
    <!--https://github.com/RicoSuter/NSwag/wiki/NSwag.MSBuild-->
    <Exec Command="$(NSwagExe_Net70) new" />
    <PropertyGroup>
      <OpenApiDocument>../../Api/Demo/bin/Debug/net7.0/Demo.json</OpenApiDocument>
      <NSwagConfiguration>nswag.json</NSwagConfiguration>
      <GeneratedOutput>Client.g.cs</GeneratedOutput>
    </PropertyGroup>
    <Exec Command="$(NSwagExe_Net70) run $(NSwagConfiguration) /variables:OpenApiDocument=$(OpenApiDocument),GeneratedOutput=$(GeneratedOutput)" />
</Target>
```


Uncomment `<Exec Command="$(NSwagExe_Net70) new" />` to generate a fresh `nswag.json`
We then need to modify the top half to specify the correct runtime and source document like so:

```json
{
  "runtime": "Net70",
  "defaultVariables": null,
  "documentGenerator": {
    "fromDocument": {
      "json": "$(OpenApiDocument)"
    }
  },
  "codeGenerators": {
     "openApiToCSharpClient": { 
      // ...
      "output": "$(GeneratedOutput)"
      // ...
     }
  }
}
```

> Unless you change the value of `codeGenerators/openApiToCSharpClient/jsonLibrary` to `SystemTextJson` you also need to install Newtonsoft.