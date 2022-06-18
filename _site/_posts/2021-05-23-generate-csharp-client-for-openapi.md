---
title: Generate C# client for OpenAPI
description: A look at using OpenAPI clients in C#
image: /assets/images/posts/20210523/generate-csharp-client-for-openapi/cover_image.png
coverimage:
    DEFAULT: '/assets/images/posts/20210523/generate-csharp-client-for-openapi/cover_image.png'
    WEB: '/assets/images/posts/20210523/generate-csharp-client-for-openapi/cover_image.webp'
tags:
  - "csharp"
  - "nswag"
  - "openapi"
  - "swashbuckle"
commentid: '15'
---
I've recently worked on a project where I was the consumer of a third party API. Luckily for me, we decided on an Open API specification which made integrating services a breeze. If you have been following my content, you know I often use C# in my projects. So I needed a type-safe client for use in my C# code base.

To accomplish my goals, I used the [NSwag library](https://github.com/RicoSuter/NSwag/wiki/NSwag.MSBuild) created by Rico Suter. This project provides me with an MSBuild task for generating clients. In my case, I used a JSON file version to generate my client. NSwag is not limited to just one way of working.

## What is OpenAPI

First, a quick recap of what is an OpenAPI. According to the [official definition](https://swagger.io/specification/):

> The OpenAPI Specification (OAS) defines a standard, language-agnostic interface to RESTful APIs which allows both humans and computers to discover and understand the capabilities of the service without access to source code, documentation, or through network traffic inspection. When properly defined, a consumer can understand and interact with the remote service with a minimal amount of implementation logic.
>
> An OpenAPI definition can then be used by documentation generation tools to display the API, code generation tools to generate servers and clients in various programming languages, testing tools, and many other use cases.

That's pretty cool. Also, if you are wondering about the difference between OpenAPI / Swagger, Swagger is part of the OpenAPI initiative since 2015. But in short OpenAPI = specification, Swagger = Tooling. In this article, I am not going into much detail in setting up your API, but Microsoft [described](https://docs.microsoft.com/en-us/aspnet/core/tutorials/web-api-help-pages-using-swagger?view=aspnetcore-5.0#openapi-vs-swagger) three versions on how to combine it with .NET Core.

## Generate client from file

The first version uses a file to generate our code. In our case, we will use a [JSON file](https://petstore.swagger.io/v2/swagger.json) from the [PetStore](https://petstore.swagger.io/) example project as provided by the swagger team.

```shell
dotnet new classlib --framework netstandard2.0 --output src/Sdks/PetStore --name Kaylumah.GenerateCSharpClientForOpenAPI.Sdks.PetStore
dotnet add package NSwag.MSBuild
dotnet add package System.ComponentModel.Annotations
dotnet add package Newtonsoft.Json
```

Safe the pet store OpenAPI JSON in the project we just created under the name `swagger.json`. We also need a `nswag.json` file with the following contents:

```json
{
    "runtime": "NetCore31",
    "documentGenerator": {
        "fromDocument": {
            "json": "swagger.json"
        }
    },
    "codeGenerators": {
        "openApiToCSharpClient": {
            "output": "Client.g.cs"
        }
    }
}
```

We use an MSBuild task that calls NSwag. Update the `...Sdks.Petstore.csproj` project file to look like this.

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

  <Target Name="GenerateSdk" BeforeTargets="Build">
    <Exec Command="$(NSwagExe_Core31) run nswag.json " />
  </Target>

</Project>
```

After building the project, we have a file named `Client.g.cs` containing everything we need to consume the PetStore API. We can use a console application to verify that we can make API calls.

```shell
dotnet new console --framework netcoreapp3.1 --output src/Client/ApiClient --name Kaylumah.GenerateCSharpClientForOpenAPI.Client.ApiClient
```

An example call we can make with our API looks like this:

```cs
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Kaylumah.GenerateCSharpClientForOpenAPI.Client.ApiClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var httpClient = new HttpClient();
            var apiClient = new MyNamespace.Client(httpClient);
            var result = await apiClient.GetInventoryAsync();
            Console.WriteLine(string.Join("|", result.Keys));
        }
    }
}

```

## Influence created output

We have established that we have a working C# client for the PetStore API. Let us look at the generated result. We got DTO's for every definition in the definitions part of the specification. We also got a class named `Client` with methods as `GetInventoryAsync`. All the generated code in `Client.g.cs` is part of the namespace `MyNamespace`; this is not helpful if I wanted to create a project with many API clients.

Two things influence the generated code. First, how you specify your fields has the most influence. For example, are your fields required, are they nullable and which kind of values are allowed. You cannot always influence this as sometimes you consume an external API; such is the case with our PetStore implementation. Luckily we can control the output by tuning values in our NSwag configuration. An eagle-eyed reader will have noticed that we are already doing this. Our nswag.json is responsible for the result. In this case, we are using the `output` variable to control the generated file's name.

We control the output by using an NSwag configuration document usually called `*.nswag` or `nswag.json`. It can be generated via NSwagStudio or manually. Over at the [NSwag Wiki](https://github.com/RicoSuter/NSwag/wiki/NSwag-Configuration-Document) you can read all about it. It's outside of the scope of this article to go into all options, so I will demonstrate a couple of changes I like to make in my projects.

> **Note**: You can generate a nswag configuration file by running `<Exec Command="$(NSwagExe_Core31) new" />`.

I encourage you to take a look at the documentation to see all configuration options. Some options apply to every generator, and some only to C# clients. See the table below for links to every section. Every section describes the options and default values if applicable.

| Settings | Description |
| - | - |
| [ClientGeneratorBaseSettings](https://github.com/RicoSuter/NSwag/wiki/ClientGeneratorBaseSettings) | Common settings for all client code generators. |
| [CSharpGeneratorBaseSettings](https://github.com/RicoSuter/NSwag/wiki/CSharpGeneratorBaseSettings) | Base settings for all C# code generators. |
| [CSharpClientGeneratorSettings](https://github.com/RicoSuter/NSwag/wiki/CSharpClientGeneratorSettings) | Settings for C# clients. |

If you look closely at your build log, you see the following line `Executing file 'nswag.json' with variables ''...`. So how do we pass variables to NSwag? Update the statement to `$(NSwagExe_Core31) run nswag.json /variables:Configuration=$(Configuration)` . Here we define a variable named Configuration and assign it the MSBuild value for $(Configuration). If we build our project, the logline reads `Executing file 'nswag.json' with variables 'Configuration=Debug'...`. You also have the option to supply default values in your NSwag configuration. This way, you don't see it as part of your build log, but it helps omit parts from the command.

| Property | Description |
| - | - |
| `namespace` and `contractsNamespace` | Control the namespace of the generated code |
| `generateContractsOutput` and `contractsOutputFilePath` | Control seperation of contract and implementation |
| `generateClientInterfaces` | create an interface |
| `exceptionClass` and `className` | control classnames |
| `operationGenerationMode` | how to create client for multiple endpoints |

After our modifications, our NSwag file looks like this.

```json
{
    "runtime": "NetCore31",
    "defaultVariables": "Configuration=Debug",
    "documentGenerator": {
        "fromDocument": {
            "json": "$(InputDocument)"
        }
    },
    "codeGenerators": {
        "openApiToCSharpClient": {
            "generateClientInterfaces": true,
            "exceptionClass": "$(SdkName)ApiException",
            "useBaseUrl": true,
            "generateBaseUrlProperty": true,
            "generateContractsOutput": true,
            "contractsNamespace": "$(SdkNamespace).Interface",
            "contractsOutputFilePath": "$(GeneratedContractFile)",
            "className": "$(SdkName)Client",
            "operationGenerationMode": "SingleClientFromOperationId",
            "namespace": "$(SdkNamespace).Service",
            "output": "$(GeneratedClientFile)"
        }
    }
}
```

To pass all the values to NSwag, we update our csproj file to look like this. For demonstration purposes, I show that the name of the MSBuild variable does not need to match the NSwag variable. Do take care that the variable names passed to NSwag need to match the name in nswag.json

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

  <Target Name="GenerateSdk" BeforeTargets="Build">
    <PropertyGroup>
        <OpenAPIDocument>swagger.json</OpenAPIDocument>
        <NSwagConfiguration>nswag.json</NSwagConfiguration>

        <SdkNamespace>$(RootNamespace)</SdkNamespace>
        <SdkName>PetStore</SdkName>
        <GeneratedInterfaceFile>$(SdkName).Interface.g.cs</GeneratedInterfaceFile>
        <GeneratedServiceFile>$(SdkName).Service.g.cs</GeneratedServiceFile>

    </PropertyGroup>
    <Error Text="The OpenAPI document '$(OpenAPIDocument)' does not exists!" Condition="!Exists('$(OpenAPIDocument)')" />
    <Error Text="The NSwag configuration '$(NSwagConfiguration)' does not exists!" Condition="!Exists('$(NSwagConfiguration)')" />
    <Exec Command="$(NSwagExe_Core31) run $(NSwagConfiguration) /variables:Configuration=$(Configuration),InputDocument=$(OpenAPIDocument),SdkName=$(SdkName),SdkNamespace=$(SdkNamespace),GeneratedClientFile=$(GeneratedServiceFile),GeneratedContractFile=$(GeneratedInterfaceFile)" />
  </Target>

</Project>
```

## Generate client from API in your project

Our second version generates the SDK based on a .NET Core API project in our solution, which can be very useful if you want to provide the client in a NuGet package to other projects/teams in your organization. The project setup will be almost identical to our file-based setup.

```shell
dotnet new classlib --framework netstandard2.0 --output src/Sdks/FromNswagApi --name Kaylumah.GenerateCSharpClientForOpenAPI.Sdks.FromNswagApi
dotnet add package NSwag.MSBuild
dotnet add package System.ComponentModel.Annotations
dotnet add package Newtonsoft.Json
```

If we are going to create an SDK we first need to generate our API project. We generate a webapi with the following command:

```sh
dotnet new webapi --framework netcoreapp3.1 --output src/Apis/Nswag/WeatherForecastApi --name Kaylumah.GenerateCSharpClientForOpenAPI.Apis.Nswag.WeatherForecastApi
```

Note that I am specifying the optional --framework option for creating the projects; this has two reasons. First, I prefer to use LTS versions of the Microsoft SDK and secondly, Microsoft made [changes](https://docs.microsoft.com/en-us/aspnet/core/release-notes/aspnetcore-5.0?view=aspnetcore-5.0#openapi-specification-on-by-default) to the webapi template in the NET5 SDK that makes it opt-out to use OpenAPI and defaults to Swashbuckle, which I don't want in this case.

```json
{
    "runtime": "NetCore31",
    "documentGenerator": {
        "aspNetCoreToOpenApi": {
            "project": "../../Apis/Nswag/WeatherForecastApi/Kaylumah.GenerateCSharpClientForOpenAPI.Apis.Nswag.WeatherForecastApi.csproj"
        }
    },
    "codeGenerators": {
        "openApiToCSharpClient": {
            "generateClientInterfaces": true,
            "exceptionClass": "$(SdkName)ApiException",
            "useBaseUrl": true,
            "generateBaseUrlProperty": true,
            "generateContractsOutput": true,
            "contractsNamespace": "$(SdkNamespace).Interface",
            "contractsOutputFilePath": "$(GeneratedContractFile)",
            "className": "$(SdkName)Client",
            "operationGenerationMode": "SingleClientFromOperationId",
            "namespace": "$(SdkNamespace).Service",
            "output": "$(GeneratedClientFile)"
        }
    }
}
```

Like before, we need a `GenerateSdk` target; the difference is that we don't have a `swagger.json`.

```xml
<Target Name="GenerateSdk" BeforeTargets="Build">
<PropertyGroup>
    <NSwagConfiguration>nswag.json</NSwagConfiguration>

    <SdkNamespace>$(RootNamespace)</SdkNamespace>
    <SdkName>Weather</SdkName>
    <GeneratedInterfaceFile>$(SdkName).Interface.g.cs</GeneratedInterfaceFile>
    <GeneratedServiceFile>$(SdkName).Service.g.cs</GeneratedServiceFile>

</PropertyGroup>
<Error Text="The NSwag configuration '$(NSwagConfiguration)' does not exists!" Condition="!Exists('$(NSwagConfiguration)')" />
<Exec Command="$(NSwagExe_Core31) run $(NSwagConfiguration) /variables:Configuration=$(Configuration),SdkName=$(SdkName),SdkNamespace=$(SdkNamespace),GeneratedClientFile=$(GeneratedServiceFile),GeneratedContractFile=$(GeneratedInterfaceFile)" />
</Target>
```

If we try to build our project now, we get an error.

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
```

The reason behind this error is that the tool requires NSwag in the API project. To do this, we need to install the NSwag.AspNetCore package with `dotnet add package NSwag.AspNetCore`.  The scope of this tutorial is not how to set up an API project with NSwag luckily, the [guide](https://github.com/RicoSuter/NSwag#usage-in-c) is straightforward. We modify the `ConfigureServices` method in Startup.cs with `services.AddOpenApiDocument();` and we add `app.UseOpenApi();` and `app.UseSwaggerUi3();` to the `Configure` method. We have an Open API specification for our WeatherForecast controller with these changes and can easily view and test it with Swagger UI.

Now we can successfully generate a client for the WeatherForecastAPI!

## Generate client from Swashbuckle project

The third and final version I will look at is a combination of both previous versions. I already hinted at it in the last section, but Microsoft made some [changes](https://docs.microsoft.com/en-us/aspnet/core/release-notes/aspnetcore-5.0?view=aspnetcore-5.0#openapi-specification-on-by-default) to the template to generate them by default using Swashbuckle.

```shell
dotnet new classlib --framework netstandard2.0 --output src/Sdks/FromSwashbuckleApi --name Kaylumah.GenerateCSharpClientForOpenAPI.Sdks.FromSwashbuckleApi
dotnet add package NSwag.MSBuild
dotnet add package System.ComponentModel.Annotations
dotnet add package Newtonsoft.Json
```

Like before, we also need a webapi project.

```sh
dotnet new webapi --framework netcoreapp3.1 --output src/Apis/Swashbuckle/WeatherForecastApi --name Kaylumah.GenerateCSharpClientForOpenAPI.Apis.Swashbuckle.WeatherForecastApi
```

Of course, we could launch the API project and browse to `https://localhost:5001/swagger/index.html` and download the specification from there. But I will opt for automating the process with a [CLI](https://github.com/domaindrivendev/Swashbuckle.AspNetCore#swashbuckleaspnetcorecli) provided as a dotnet tool by Swashbuckle.

Since we are using netcoreapp3.1 we can make use of a local tool manifest.

```sh
dotnet new tool-manifest
dotnet tool install --version 6.1.4 Swashbuckle.AspNetCore.Cli
```

This allows us to run

```sh
swagger tofile --output [output] [startupassembly] [swaggerdoc]`. For example, in the FromSwashbuckleApi folder we would run `dotnet swagger tofile --output swagger.json ../../Apis/Swashbuckle/WeatherForecastApi/bin/Debug/netcoreapp3.1/Kaylumah.GenerateCSharpClientForOpenAPI.Apis.Swashbuckle.WeatherForecastApi.dll v1
```

At the moment, this returns an error if you target a netcoreapp3.1 project when using a net5 SDK. This [issue](https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/2006) describes a change in 6.x of the tool. A workaround for this is using a global.json file.

```json
{
    "sdk": {
        "version": "3.1.406",
        "rollForward": "latestPatch"
    }
}
```

Similar to the NSwag version, we still need to add Swashbuckle to the webapi. Luckily just as with NSwag the [guide](https://github.com/domaindrivendev/Swashbuckle.AspNetCore#getting-started) is straightforward.

```xml
<Target Name="GenerateOpenAPI" BeforeTargets="GenerateSdk">
  <Exec Command="dotnet swagger tofile --output swagger.json ../../Apis/Swashbuckle/WeatherForecastApi/bin/Debug/netcoreapp3.1/Kaylumah.GenerateCSharpClientForOpenAPI.Apis.Swashbuckle.WeatherForecastApi.dll v1" />
</Target>

<Target Name="GenerateSdk" BeforeTargets="Build">
  <PropertyGroup>
    <OpenAPIDocument>swagger.json</OpenAPIDocument>
    <NSwagConfiguration>nswag.json</NSwagConfiguration>

    <SdkNamespace>$(RootNamespace)</SdkNamespace>
    <SdkName>Weather</SdkName>
    <GeneratedInterfaceFile>$(SdkName).Interface.g.cs</GeneratedInterfaceFile>
    <GeneratedServiceFile>$(SdkName).Service.g.cs</GeneratedServiceFile>
  </PropertyGroup>
  <Error Text="The OpenAPI document '$(OpenAPIDocument)' does not exists!" Condition="!Exists('$(OpenAPIDocument)')" />
  <Error Text="The NSwag configuration '$(NSwagConfiguration)' does not exists!" Condition="!Exists('$(NSwagConfiguration)')" />
  <Exec Command="$(NSwagExe_Core31) run $(NSwagConfiguration) /variables:Configuration=$(Configuration),InputDocument=$(OpenAPIDocument),SdkName=$(SdkName),SdkNamespace=$(SdkNamespace),GeneratedClientFile=$(GeneratedServiceFile),GeneratedContractFile=$(GeneratedInterfaceFile)" />
</Target>
```

Now that we generated a second version of our Weather API, let's quickly compare the two.

```cs
// Swashbuckle
[System.CodeDom.Compiler.GeneratedCode("NSwag", "13.11.1.0 (NJsonSchema v10.4.3.0 (Newtonsoft.Json v12.0.0.0))")]
public partial interface IWeatherClient
{
    /// <returns>Success</returns>
    /// <exception cref="WeatherApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<System.Collections.Generic.ICollection<WeatherForecast>> WeatherForecastAsync();

    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Success</returns>
    /// <exception cref="WeatherApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<System.Collections.Generic.ICollection<WeatherForecast>> WeatherForecastAsync(System.Threading.CancellationToken cancellationToken);

}

// NSwag
[System.CodeDom.Compiler.GeneratedCode("NSwag", "13.11.1.0 (NJsonSchema v10.4.3.0 (Newtonsoft.Json v12.0.0.0))")]
public partial interface IWeatherClient
{
    /// <exception cref="WeatherApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<System.Collections.Generic.ICollection<WeatherForecast>> WeatherForecast_GetAsync();

    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <exception cref="WeatherApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<System.Collections.Generic.ICollection<WeatherForecast>> WeatherForecast_GetAsync(System.Threading.CancellationToken cancellationToken);

}
```

Funnily enough, even in a specification as small as these, there can already be differences!

## Closing Thoughts

As we have seen, there are multiple ways to generate a client by using `NSwag.MSBuild`.
If I am writing an OpenAPI specification, I prefer the syntax of Swashbuckle for several things like API versioning. That, of course, is a personal preference, but since Microsoft now also offers Swashbuckle as a default, it is nice to know we can make Swashbuckle and NSwag play nice together. How I configure my API with OpenAPI, API Versioning, ProblemDetails will be part of a future blog post.

So, where do we go from here? I did not mention it in the article, but in every generated client, we need to inject `System.Net.HttpClient`, which means we can combine it with [HttpClientFactory](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests) and all the options it provides. Alas, that is also a topic for another day.

As always, if you have any questions, feel free to reach out. Do you have suggestions or alternatives? I would love to hear about them.

The corresponding source code for this article is on [GitHub](https://github.com/kaylumah/GenerateCSharpClientForOpenAPI).

See you next time, stay healthy and happy coding to all 🧸!

## Sources

- [NSwag GitHub](https://github.com/RicoSuter/NSwag/wiki/)
- [Swashbuckle GitHub](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)