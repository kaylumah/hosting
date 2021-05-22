---
title: "C# OpenAPI Client using NSwag - Part 1: From File"
description: "-"
# cover_image: '/assets/images/posts/20210411/approach-to-writing-mocks/cover_image.png'
# image: '/assets/images/posts/20210411/approach-to-writing-mocks/cover_image.png'
tags:
  - "CSharp"
  - "OpenAPI"
  - "NSwag"
series: "C# OpenAPI Client Generation"
---
I've recently worked on a project where I was the consumer of a third party API. Luckily for me, we decided on an Open API specification which made integrating services a breeze. If you have been following my content, you know I often use C# in my projects. So I needed a type-safe client for use in my C# code base.

To accomplish my goals, I used the NSwag library created by Rico Suter. This project provides me with an MSBuild task for generating clients. In my case, I used a JSON file version to generate my client. NSwag is not limited to just one way of working. So instead of writing a long article, this will be my first series.

In this introductory post, we will set up our project and focus on the first variant. Throughout the series, we will go over:

1. Using a JSON file
2. Using an ASP.NET Core WebAPI project
3. Combining Swashbuckle and NSwag

## What is OpenAPI

First, a quick recap of what is an OpenAPI. According to the [official definition](https://swagger.io/specification/):

> The OpenAPI Specification (OAS) defines a standard, language-agnostic interface to RESTful APIs which allows both humans and computers to discover and understand the capabilities of the service without access to source code, documentation, or through network traffic inspection. When properly defined, a consumer can understand and interact with the remote service with a minimal amount of implementation logic.
>
> An OpenAPI definition can then be used by documentation generation tools to display the API, code generation tools to generate servers and clients in various programming languages, testing tools, and many other use cases.

That's actually pretty cool. Also if you are wondering about the difference between OpenAPI / Swagger, Swagger is actually part of the OpenAPI iniative since 2015. But in short OpenAPI = specication, Swagger = Tooling. In this series I am not going into much detail in how to setup you API but Microsoft described three versions on how to combine it with .NET Core (https://docs.microsoft.com/en-us/aspnet/core/tutorials/web-api-help-pages-using-swagger?view=aspnetcore-5.0#openapi-vs-swagger)

## Project Setup

For our demonstration, we are going to need a couple of projects.
We need a client application to verify that we can run and test our API.  We will also need a classlibrary project for every variant. It's not required to create separate projects, but I see each generated client as an SDK for that specific API, so I like to keep them separate. Keeping them separate will also allow us to publish the generated code as part of a NuGet package.

```shell
dotnet new webapi --framework netcoreapp3.1 --output src/Api/NSwagApi --name Kaylumah.GenerateCSharpOpenApiClient.Api.NSwagApi
dotnet new webapi --framework netcoreapp3.1 --output src/Api/SwashbuckleApi --name Kaylumah.GenerateCSharpOpenApiClient.Api.SwashbuckleApi

dotnet new classlib --framework netstandard2.0 --output src/Sdk/FromFile --name Kaylumah.GenerateCSharpOpenApiClient.Sdk.FromFile
dotnet new classlib --framework netstandard2.0 --output src/Sdk/FromNSwag --name Kaylumah.GenerateCSharpOpenApiClient.Sdk.FromNSwag
dotnet new classlib --framework netstandard2.0 --output src/Sdk/FromSwashbuckle --name Kaylumah.GenerateCSharpOpenApiClient.Sdk.FromSwashbuckle

dotnet new console --framework netcoreapp3.1 --output src/Client/Demo --name Kaylumah.GenerateCSharpOpenApiClient.Client.Demo

dotnet sln add **/*.csproj

dotnet build
```

Note that I am specifying the optional --framework option for creating the projects; this has two reasons. First, I prefer to use LTS versions of the Microsoft SDK and secondly, Microsoft made changes to the webapi template in the NET5 SDK that makes it opt-out to use OpenAPI and defaults to Swashbuckle, which I don't want in this case.

## From File

For our file demo, we will use the [PetStore example](https://petstore.swagger.io/v2/swagger.json) provided to use by swagger. 

(In from file project...) / Note Build Twice

```sh
dotnet add package NSwag.MSBuild
dotnet add package System.ComponentModel.Annotations
dotnet add package Newtonsoft.Json
```

## From API Project

```sh
dotnet add package NSwag.MSBuild
dotnet add package System.ComponentModel.Annotations
dotnet add package Newtonsoft.Json
```

dotnet new sln
dotnet new webapi --framework netcoreapp3.1 --output Api
dotnet new classlib --framework netstandard2.0 --output Sdk
dotnet new console --framework netcoreapp3.1 --output Demo

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>netstandard2.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Newtonsoft.Json" Version="13.0.1" />
    <PackageReference Include="NSwag.MSBuild" Version="13.11.1">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="System.ComponentModel.Annotations" Version="5.0.0" />
  </ItemGroup>

  <Target Name="NSwag" BeforeTargets="PrepareForBuild">
    <Exec Command="$(NSwagExe_Core31) run nswag.json /variables:Configuration=$(Configuration)" />
  </Target>

</Project>
```

```json
{
    "runtime": "NetCore31",
    "documentGenerator": {
        "aspNetCoreToOpenApi": {
            "project": "../Api/Api.csproj"
        }
    },
    "codeGenerators": {
        "openApiToCSharpClient": {
            "output": "Client.g.cs"
        }
    }
}
```

```output
Microsoft (R) Build Engine version 16.9.0+57a23d249 for .NET
Copyright (C) Microsoft Corporation. All rights reserved.

  Determining projects to restore...
  All projects are up-to-date for restore.
  NSwag command line tool for .NET Core NetCore31, toolchain v13.11.1.0 (NJsonSchema v10.4.3.0 (Newtonsoft.Json v12.0.0.0))
  Visit http://NSwag.org for more information.
  NSwag bin directory: /Users/maxhamulyak/.nuget/packages/nswag.msbuild/13.11.1/tools/NetCore31
  
  Executing file 'nswag.json' with variables 'Configuration=Debug'...
  Launcher directory: /Users/maxhamulyak/.nuget/packages/nswag.msbuild/13.11.1/tools/NetCore31
  System.Reflection.TargetInvocationException: Exception has been thrown by the target of an invocation.
   ---> System.InvalidOperationException: No service for type 'NSwag.Generation.IOpenApiDocumentGenerator' has been registered.
     at Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService(IServiceProvider provider, Type serviceType)
     at Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService[T](IServiceProvider provider)
     at NSwag.Commands.Generation.AspNetCore.AspNetCoreToSwaggerCommand.GenerateDocumentWithDocumentProviderAsync(IServiceProvider serviceProvider) in C:\projects\nswag\src\NSwag.Commands\Commands\Generation\AspNetCore\AspNetCoreToOpenApiCommand.cs:line 299
     at NSwag.Commands.Generation.AspNetCore.AspNetCoreToSwaggerCommand.GenerateDocumentAsync(AssemblyLoader assemblyLoader, IServiceProvider serviceProvider, String currentWorkingDirectory) in C:\projects\nswag\src\NSwag.Commands\Commands\Generation\AspNetCore\AspNetCoreToOpenApiCommand.cs:line 289
     at NSwag.Commands.Generation.AspNetCore.AspNetCoreToOpenApiGeneratorCommandEntryPoint.<>c__DisplayClass0_0.<<Process>b__0>d.MoveNext() in C:\projects\nswag\src\NSwag.Commands\Commands\Generation\AspNetCore\AspNetCoreToOpenApiGeneratorCommandEntryPoint.cs:line 30
  --- End of stack trace from previous location where exception was thrown ---
     at NSwag.Commands.Generation.AspNetCore.AspNetCoreToOpenApiGeneratorCommandEntryPoint.Process(String commandContent, String outputFile, String applicationName) in C:\projects\nswag\src\NSwag.Commands\Commands\Generation\AspNetCore\AspNetCoreToOpenApiGeneratorCommandEntryPoint.cs:line 29
     --- End of inner exception stack trace ---
     at System.RuntimeMethodHandle.InvokeMethod(Object target, Object[] arguments, Signature sig, Boolean constructor, Boolean wrapExceptions)
     at System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
     at System.Reflection.MethodBase.Invoke(Object obj, Object[] parameters)
     at NSwag.AspNetCore.Launcher.Program.Main(String[] args) in C:\projects\nswag\src\NSwag.AspNetCore.Launcher\Program.cs:line 170
  System.InvalidOperationException: Swagger generation failed with non-zero exit code '1'.
     at NSwag.Commands.Generation.AspNetCore.AspNetCoreToSwaggerCommand.RunAsync(CommandLineProcessor processor, IConsoleHost host) in C:\projects\nswag\src\NSwag.Commands\Commands\Generation\AspNetCore\AspNetCoreToOpenApiCommand.cs:line 234
     at NSwag.Commands.NSwagDocumentBase.GenerateSwaggerDocumentAsync() in C:\projects\nswag\src\NSwag.Commands\NSwagDocumentBase.cs:line 280
     at NSwag.Commands.NSwagDocument.ExecuteAsync() in C:\projects\nswag\src\NSwag.Commands\NSwagDocument.cs:line 81
     at NSwag.Commands.Document.ExecuteDocumentCommand.ExecuteDocumentAsync(IConsoleHost host, String filePath) in C:\projects\nswag\src\NSwag.Commands\Commands\Document\ExecuteDocumentCommand.cs:line 86
     at NSwag.Commands.Document.ExecuteDocumentCommand.RunAsync(CommandLineProcessor processor, IConsoleHost host) in C:\projects\nswag\src\NSwag.Commands\Commands\Document\ExecuteDocumentCommand.cs:line 32
     at NConsole.CommandLineProcessor.ProcessSingleAsync(String[] args, Object input)
     at NConsole.CommandLineProcessor.ProcessAsync(String[] args, Object input)
     at NConsole.CommandLineProcessor.Process(String[] args, Object input)
     at NSwag.Commands.NSwagCommandProcessor.Process(String[] args) in C:\projects\nswag\src\NSwag.Commands\NSwagCommandProcessor.cs:line 56
```

Ok, fair enough we first need to make our project run NSwag.
To do this we enable NSwag.AspNetCore

dotnet add package NSwag.AspNetCore

https://github.com/RicoSuter/NSwag#usage-in-c

```csharp
using System;
using System.Net.Http;
using System.Threading.Tasks;
using MyNamespace;

namespace Demo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var httpClient = new HttpClient();
            var baseUrl = "https://localhost:5001/";
            var client = new WeatherForecastClient(baseUrl, httpClient);
            var result = await client.GetAsync();
            Console.WriteLine($"Got {result.Count} WeatherForecast");
        }
    }
}

```

## From Swashbuckle

dotnet new webapi --framework netcoreapp3.1 --output Api
dotnet new classlib --framework netstandard2.0 --output Sdk
dotnet new console --framework netcoreapp3.1 --output Demo

https://github.com/domaindrivendev/Swashbuckle.AspNetCore#getting-started
dotnet add package Swashbuckle.AspNetCore

https://github.com/domaindrivendev/Swashbuckle.AspNetCore#swashbuckleaspnetcorecli
dotnet new tool-manifest
dotnet tool install --version 6.1.4 Swashbuckle.AspNetCore.Cli

```sh
dotnet add package NSwag.MSBuild
dotnet add package System.ComponentModel.Annotations
dotnet add package Newtonsoft.Json
```
https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/2006


In another post I will go into how I configure my swashbuckle...


https://docs.microsoft.com/en-us/aspnet/core/release-notes/aspnetcore-5.0?view=aspnetcore-5.0#openapi-specification-on-by-default





https://github.com/RicoSuter/NSwag/wiki/NSwag.MSBuild