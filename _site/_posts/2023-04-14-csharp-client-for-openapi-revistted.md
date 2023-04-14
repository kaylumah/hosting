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
I am working on an article for the blog that relies on a C# generated Open API client. I wrote an article on that a few years ago called ["Generate C# client for OpenAPI"](https://kaylumah.nl/2021/05/23/generate-csharp-client-for-openapi.html). So I decided to check if the advice from that post would still be valid today. Combined with the fact that, according to analytics, it is one of my most popular articles to date, this post was born. 

The solution provided relied on using an MSBuild task to generate the API on build using a tool called NSwag. However, even back then, in 2021, an alternative was already available. Steve Collins, another dotnet content creator, published an article called ["Using OpenApiReference To Generate Open API Client Code"](https://stevetalkscode.co.uk/openapireference-commands). The alternative directly adds OpenAPI support to the project while still using NSWag under the hood. Back then, Steve mentioned that there was little documentation, and I was already familiar with doing it manually, so I decided to stick with that. Today I wanted to compare doing it manually or via the built-in mechanism.

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

## Using OpenAPI Reference

### Using OpenAPI reference from Visual Studio

You can right-click on any project to add a connected service to the project. 

[IMAGE HERE]

[IMAGE HERE]

Note by choosing the option "Service reference..." instead of "Connected Service" you get the second prompt at once. By choosing "Connected service" you get the overview for all connected service for the project and then need to an extra click to add the service reference.

On the third screen we can customize the input for the msbuild task. 

[IMAGE HERE]

The result looks like this:

```xml
<ItemGroup>
  <OpenApiReference Include="..\..\Api\Demo\bin\Debug\net7.0\Demo.json" 
                    CodeGenerator="NSwagCSharp"
                    Link="OpenAPIs\Demo.json" />
</ItemGroup>

<ItemGroup>
  <PackageReference Include="Microsoft.Extensions.ApiDescription.Client" Version="3.0.0">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
  </PackageReference>
  <PackageReference Include="Newtonsoft.Json" Version="13.0.1" />
  <PackageReference Include="NSwag.ApiDescription.Client" Version="13.0.5">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
  </PackageReference>
</ItemGroup>
```

If you build the project the generated code will be in the `obj` folder for the project.

### Using OpenAPI reference from command line

If you do not have Visual Studio you can use a package called `Microsoft.dotnet-openapi` which is available as a dotnet tool.
You can install it with the command `dotnet tool install --local Microsoft.dotnet-openapi --version 7.0.4`

You can then add the `OpenApiReference` by running the following command from your project directory

```shell
dotnet dotnet-openapi add file ..\..\Api\Demo\bin\Debug\net7.0\Demo.json
```

The result looks like this:

```xml
<ItemGroup>
  <PackageReference Include="Newtonsoft.Json" Version="12.0.2" />
  <PackageReference Include="NSwag.ApiDescription.Client" Version="13.0.5" />
</ItemGroup>
<ItemGroup>
  <OpenApiReference Include="..\..\Api\Demo\bin\Debug\net7.0\Demo.json" />
</ItemGroup>
```

My expectation would have been


### Customizing...

## Conclusion

I am not sure I prefer one option of the other. Adding a custom build target feels like magic, but its explici