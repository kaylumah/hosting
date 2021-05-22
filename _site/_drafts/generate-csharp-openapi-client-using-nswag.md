---
title: "NSWAG (WIP)"
description: "Someone like you. Someone who'll rattle the cages. Does it come in black? Bruce Wayne, eccentric billionaire. Hero can be anyone. Even a man knowing something as simple and reassuring as putting a coat around a young boy shoulders to let him know the world hadn't ended."
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

---

# Pocket ERP SDK

Go to https://propaz-stage.herokuapp.com/api/tenant/swagger/

# NSwag Configuration

[ClientGeneratorBaseSettings](https://github.com/RicoSuter/NSwag/wiki/ClientGeneratorBaseSettings)

[CSharpGeneratorBaseSettings](https://github.com/RicoSuter/NSwag/wiki/CSharpGeneratorBaseSettings)

[CSharpClientGeneratorSettings](https://github.com/RicoSuter/NSwag/wiki/CSharpClientGeneratorSettings)

## Settings of Note

| Param | Default Value | Our Value | Reason |
| :-: | :-: | :-: | :-: |
| className | {controller}Client | $(ClientName)Client | Use name of the client as defined in csproj |
| operationGenerationMode | MultipleClientsFromOperationId | SingleClientFromOperationId | we want a single client. |
| namespace | MyNamespace | $(ClientNamespace) | Pass correct namespace from csproj |
| output | - | $(GeneratedClientFile) | Set the correct value for output from csproj |
| GenerateClientInterfaces | false | true | Hide API behind interface |
| InjectHttpClient | true | true | Constructor with HttpClient, which allows us to use HttpClientFactory. |
| generateOptionalParameters | false | true | One operation including CancellationToken. |
| generateBaseUrlProperty & useBaseUrl | true/true | false/false | we want to set based on IConfiguration |
| exceptionClass | ApiException | $(ClientName)ApiException | Custom exception per ERP |
| generateContractsOutput | false | true | split contract and client definition |
| contractsNamespace | - | $(ClientNamespace) | split contract and client definition |
| contractsOutputFilePath | - | $(GeneratedClientContractsFile) | split contract and client definition|






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
            "output": "Client.g.cs"
        }
    }
}







  <Target Name="GenerateNSwagClient" BeforeTargets="PrepareForBuild">
    <PropertyGroup>
      <InputSwagger>swagger.json</InputSwagger>
    </PropertyGroup>
    <Exec Command="dotnet swagger tofile --output swagger.json ../MyApi/bin/Debug/net5.0/MyApi.dll v1" />
    <Exec Command="$(NSwagExe_Core31) run nswag.json /variables:InputSwagger=$(InputSwagger),Configuration=$(Configuration)" />
  </Target>


   <Target Name="NSwag" BeforeTargets="PrepareForBuild">
    <PropertyGroup>
      <InputSwagger>swagger.json</InputSwagger>
    </PropertyGroup>
    <Exec Command="dotnet swagger tofile --output $(InputSwagger) ../Api/bin/Debug/netcoreapp3.1/Api.dll v1" />
    <Exec Command="$(NSwagExe_Core31) run nswag.json /variables:InputSwagger=$(InputSwagger),Configuration=$(Configuration)" />
  </Target>



    <ItemGroup>
    <PackageReference Include="Newtonsoft.Json" Version="13.0.1" />
    <PackageReference Include="NSwag.MSBuild" Version="13.11.1">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="System.ComponentModel.Annotations" Version="5.0.0" />
  </ItemGroup>

  <Target Name="NSwag" BeforeTargets="PrepareForBuild">
    <PropertyGroup>
      <InputSwagger>petstore.json</InputSwagger>
    </PropertyGroup>
    <Exec Command="$(NSwagExe_Core31) run nswag.json /variables:InputSwagger=$(InputSwagger),Configuration=$(Configuration)" />
  </Target>





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
            "output": "Client.g.cs"
        }
    }
}

{
    "runtime": "NetCore31",
    "documentGenerator": {
        "aspNetCoreToOpenApi": {
            "project": "../../Api/NSwagApi/Kaylumah.GenerateCSharpOpenApiClient.Api.NSwagApi.csproj"
        }
    },
    "codeGenerators": {
        "openApiToCSharpClient": {
            "output": "Client.g.cs"
        }
    }
}

propaz