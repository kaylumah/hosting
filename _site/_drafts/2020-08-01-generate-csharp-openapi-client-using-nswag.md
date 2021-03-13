---
title: "NSWAG WIP"
description: "..."
cover_image: "/assets/images/posts/drafts/nswag/ToolchainDiagram.png"
tags:
    - "C#" 
    - "OpenAPI"
    - "ASP.NET Core"
comments: false
source_url: "https://github.com/Kaylumah/lab-generate-csharp-openapi-client-using-nswag"
draft: true
sitemap: false
hidden: true
---

When integrating with a third party you rely on an interface defintion. If you are lucky the other party provides an easy SDK which you can use by installing a NuGet package. 
Perhaps you can get an OpenAPI defintion file. Using the awesome library [NSwag](https://github.com/RicoSuter/NSwag){: .external } you can generate clients.
In the past I have used openapi generators to go from Java Spring Boot -> TypeScript


> Note: At the time of writing `NET5.0` is still in preview, on my machine I have the preview SDK installed and am using it in a few projects. However for this blog I explicitly want to use a LTS release of `.NET CORE` that is why some commands are postfixed with a `--framework TFM` 

```sh
$ dotnet new sln --name Kaylumah.Labs.NSwag
The template "Solution File" was created successfully.
$ dotnet new webapi --framework netcoreapp3.1 --output src/NSwagLab/ApiProject --name Kaylumah.Labs.NSwag.ApiProject
The template "ASP.NET Core Web API" was created successfully.
$ dotnet new console --framework netcoreapp3.1 --output src/NSwagLab/Client/ConsoleApiClient --name Kaylumah.Labs.NSwag.ConsoleApiClient
$ dotnet new classlib --framework netstandard2.1 --output src/NSwagLab/SDK/FileBased --name Kaylumah.Labs.NSwag.SDK.FileBased
The template "Class library" was created successfully.
$ dotnet new classlib --framework netstandard2.1 --output src/NSwagLab/SDK/ApiBased --name Kaylumah.Labs.NSwag.SDK.ApiBased
The template "Class library" was created successfully.
$ dotnet sln add **/*.csproj
Project `src/NSwagLab/ApiProject/Kaylumah.Labs.NSwag.ApiProject.csproj` added to the solution.
Project `src/NSwagLab/Client/ConsoleApiClient/Kaylumah.Labs.NSwag.ConsoleApiClient.csproj` added to the solution.
Project `src/NSwagLab/SDK/ApiBased/Kaylumah.Labs.NSwag.SDK.ApiBased.csproj` added to the solution.
Project `src/NSwagLab/SDK/FileBased/Kaylumah.Labs.NSwag.SDK.FileBased.csproj` added to the solution.
```

What are we going to do?

## File Based

For the file based client we will be using https://petstore.swagger.io

-> dotnet add package NSwag.MSBuild
Determining projects to restore...
info : Adding PackageReference for package 'NSwag.MSBuild' into project '/Users/maxhamulyak/Downloads/Development/X/src/NSwagLab/SDK/FileBased/Kaylumah.Labs.NSwag.SDK.FileBased.csproj'.
info : Restoring packages for /Users/maxhamulyak/Downloads/Development/X/src/NSwagLab/SDK/FileBased/Kaylumah.Labs.NSwag.SDK.FileBased.csproj...

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>netstandard2.1</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Newtonsoft.Json" Version="12.0.3" />
    <PackageReference Include="NSwag.MSBuild" Version="13.6.2">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="System.ComponentModel.Annotations" Version="4.7.0" />
  </ItemGroup>
  <Target Name="GenerateNSwagClient">
    <PropertyGroup>
      <InputSwagger>swagger.json</InputSwagger>
    </PropertyGroup>
    <Exec Command="$(NSwagExe_Core31) run nswag.json /variables:InputSwagger=$(InputSwagger),Configuration=$(Configuration)" />
  </Target>
</Project>
  ```

```json
{
    "runtime": "NetCore31",
    "defaultVariables": null,
    "documentGenerator": {
        "fromDocument": {
            "json": "$(InputSwagger)"
        }
    },
    "codeGenerators": {
        "openApiToCSharpClient": {
            "generateClientInterfaces": true,
            "output": "$(GeneratedSwaggerClientFile)"
        }
    }
}
```


  dotnet build 2x
  > Newtonsoft.Json System.ComponentModel.Annotations

dotnet add package Newtonsoft.Json
dotnet add package System.ComponentModel.Annotations


ConsoleApiClient % dotnet add reference ../../SDK/FileBased/Kaylumah.Labs.NSwag.SDK.FileBased.csproj 
Reference `..\..\..\SDK\FileBased\Kaylumah.Labs.NSwag.SDK.FileBased.csproj` added to the project.

We can now use it

```cs
using System;
using System.Net.Http;
using MyNamespace;

namespace Kaylumah.Labs.NSwag.ConsoleApiClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var httpClient = new HttpClient();
            IClient petStoreClient = new Client(httpClient);
            var result = petStoreClient.GetInventoryAsync().Result;
            foreach(var item in result)
            {
                Console.WriteLine($"{item.Key}:{item.Value}");
            }
        }
    }
}
```


## WebAPI Based

So let's use the WebAPI we created earlier
`NSwag.AspNetCore`

dotnet add package NSwag.AspNetCore



```cs
public void ConfigureServices(IServiceCollection services)
{
    //.. ommited
    services.AddSwaggerDocument(settings =>
    {
        settings.PostProcess = document =>
        {
            document.Info.Version = "v1";
        };
    });

}
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
            // .. ommited
            app.UseOpenApi();
            app.UseSwaggerUi3();
}
```

We need another NSwag.json

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>netstandard2.1</TargetFramework>
  </PropertyGroup>

    <ItemGroup>
      <PackageReference Include="Newtonsoft.Json" Version="12.0.3" />
    <PackageReference Include="System.ComponentModel.Annotations" Version="4.7.0" />

    <PackageReference Include="NSwag.MSBuild" Version="13.6.2">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
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
            "project": "../../ApiProject/Kaylumah.Labs.NSwag.ApiProject.csproj"
        }
    },
    "codeGenerators": {
        "openApiToCSharpClient": {
            "output": "Client.g.cs"
        }
    }
}
```



## Configuration

> TODO

## Bonus: IHttpClientFactory

> TODO

{% include notice.html %}
