---
title: Generate C# client for OpenAPI
description: A look at using OpenAPI clients in C#
image: /assets/images/posts/20230414/openapi/cover_image.png
tags:
  - csharp
  - nswag
  - openapi
  - swashbuckle
commentid: '143'
---
I am working on an article for the blog that relies on a C# generated Open API client. I wrote an article on that a few years ago called ["Generate C# client for OpenAPI"](https://kaylumah.nl/2021/05/23/generate-csharp-client-for-openapi.html). So I decided to check if the advice from that post would still be valid today. Combined with the fact that, according to analytics, it is one of my most popular articles to date, this post was born. 

The solution provided relied on using an MSBuild task to generate the API on build using a tool called NSwag. However, even back then, in 2021, an alternative was already available. Steve Collins, another dotnet content creator, published an article called ["Using OpenApiReference To Generate Open API Client Code"](https://stevetalkscode.co.uk/openapireference-commands). The alternative directly adds OpenAPI support to the project while still using NSWag under the hood. Back then, Steve mentioned that there was little documentation, and I was already familiar with doing it manually, so I decided to stick with that. Today I wanted to compare doing it manually or via the built-in mechanism.

## Safe OpenAPI specification on build

The purpose of the post is not to detail how to configure an OpenAPI spec for your project since the standard template already supports Swashbuckle. You can find more information on that over [at Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-7.0&tabs=visual-studio). One thing I like to add to the standard template, is that I want the specification to be part of the project output. We can achieve that with the Swashbuckle CLI, which you can install with the command `dotnet tool install --local Swashbuckle.AspNetCore.Cli --version 6.4.0`. Not that the version of the CLI must match the version of Swashbuckle used in the API project. After you install the tool, you can modify the csproj to look like this.

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

The `swagger` command takes the output location (OutputApiDocument), the DLL for the specification (ApiDll) and the document name (OpenAPIDocumentName) as input parameters. The default name of the API document is `v1`. We use some existing MSBuild properties to populate these parameters, so in our case, `OutputPath` looks like `bin/Debug/net7.0/` and `AssemblyName` is `Demo`. That means that after the project builds, a file `bin/Debug/net7.0/Demo.json` will contain our Open API Specification.

Note that as part of the `bin` folder, the specification is not under source control. Sometimes I place it in the project root to track any changes made to the specification. Doing so is especially useful for monitoring unexpected or unintended changes to the specification.

## Use NSwag the classic way...

To add NSwag manually to our project, we need the `NSwag.MSBuild` NuGet package. Which we can install via `dotnet add package NSwag.MSBuild --version 13.18.2`. The process is mostly the same as I detailed in 2021; one of the few changes is the target framework to use. Modify the csproj as follows:

```xml
<Target Name="NSwag" AfterTargets="PostBuildEvent" Condition=" '$(Configuration)' == 'Debug' ">
    <!--https://github.com/RicoSuter/NSwag/wiki/NSwag.MSBuild-->
    <!-- <Exec Command="$(NSwagExe_Net70) new" /> -->
    <PropertyGroup>
      <OpenApiDocument>../../Api/Demo/bin/Debug/net7.0/Demo.json</OpenApiDocument>
      <NSwagConfiguration>nswag.json</NSwagConfiguration>
      <GeneratedOutput>Client.g.cs</GeneratedOutput>
    </PropertyGroup>
    <Exec Command="$(NSwagExe_Net70) run $(NSwagConfiguration) /variables:OpenApiDocument=$(OpenApiDocument),GeneratedOutput=$(GeneratedOutput)" />
</Target>
```

You can uncomment `$(NSwagExe_Net70) new` to generate a fresh nswag.json, the configuration file used for NSwag. After you have the config file, you still need to specify the runtime, the document, and the output location. Abbreviated the change to the file looks like this:

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

I don't remember it being possible back then, but you can now change the JSON serializer used in the generated client. You can do so by modifying the value of `codeGenerators/openApiToCSharpClient/jsonLibrary` to `SystemTextJson`. If you do not do this, you must install the `Newtonsoft.Json` package, or the generated code will not compile.

## Using OpenAPI Reference

### Using OpenAPI reference from Visual Studio

You can right-click on any project to add a connected service to the project. 

![Microsoft Visual Studio - Add Service reference](/assets/images/posts/20230414/openapi/01_add_service_reference.png){width=1546 height=1000}

![Microsoft Visual Studio - Select service reference type](/assets/images/posts/20230414/openapi/02_select_service_type.png){width=1443 height=925}

Note by choosing the option "Service reference..." instead of "Connected Service" you get the second prompt at once. By choosing "Connected service" you get the overview for all connected service for the project and then need to an extra click to add the service reference.

On the third screen we can customize the input for the msbuild task. 

![Microsoft Visual Studio - Add service reference OpenAPI](/assets/images/posts/20230414/openapi/03_add_openapi.png){width=1547 height=923}

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