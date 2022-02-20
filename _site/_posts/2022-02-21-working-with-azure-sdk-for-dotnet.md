---
title: "Working with Azure SDK for .NET"
description: "Learn how a simple Roslyn Analyzer can improve code consistency"
cover_image:
    DEFAULT: '/assets/images/posts/20220221/working-with-azure-sdk-for-dotnet/cover_image.png'
    WEB: '/assets/images/posts/20220221/working-with-azure-sdk-for-dotnet/cover_image.webp'
image: '/assets/images/posts/20220221/working-with-azure-sdk-for-dotnet/cover_image.png'
tags:
  - "csharp"
  - "azure"
comment_id: '67'
publishedtime: '21:45'
---
February 2022 marks the 20th anniversary of the dotnet platform, which is quite a milestone. It gave me pause and time to reflect; I have been working professionally for almost six years and using .NET during the four years before that in my studies. For a dotnet blogger like myself, I could not stand idly by and let this pass without a post. February 2022 also marks another milestone for me. My first ever open-source contribution has been released into the wild. I made a [small contribution](https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/servicebus/Azure.Messaging.ServiceBus/CHANGELOG.md#760-2022-02-08) to the `Azure SDK for .NET`. So in honour of both, I wrote this article with small tips and tricks I picked up when working with the SDK.

## Which Azure SDK should I use?
Since [July 2019](https://devblogs.microsoft.com/azure-sdk/state-of-the-azure-sdk-2021/), Microsoft has made a design effort to unify the SDKs for the different services. There are shared concepts between the libraries like authentication and diagnostics. The libraries follow the pattern `Azure.{service}.{library}`.
My contribution was to the ServiceBus SDK, so today's article focus is the service bus. Almost everything described is transferable to the other SDKs; only a few bits are ServiceBus specific. The NuGet package we need is the Azure.Messaging.Service` package.

## How to set up Azure Service Bus with Azure CLI?
I think the local development aspect of any service is as important as ease of use in production. Unfortunately, there is no way to emulate the service bus locally; Jimmy Bogard wrote about that in [this article](https://jimmybogard.com/local-development-with-azure-service-bus/). Without emulating, we need to set up our resources in Azure, even for our development environment. There are a few possible options to create resources in Azure:
- Manually via the Azure Portal
- Infrastructure as Code (ARM, Bicep, etc.)
- Scripting (Azure CLI, Azure Powershell Module)

For prototypes such as this article, I prefer Azure CLI since the commands are repeatable and, more importantly, easy to understand.
> **NOTE**:
>
> When I work with the Azure CLI, I use the [Azure CLI Tools](https://marketplace.visualstudio.com/items?itemName=ms-vscode.azurecli) extension for VS Code. It provides Intellisense and snippets to work with the CLI.

```sh
azure cli snippet
```

## How does the Azure Service Bus SDK work?

For simplicity, this demo will consist of a single XUnit project that demonstrates the various scenarios for the article. Since the scope of the demo is the general use of the SDK, we will use one of the sample scenarios listed in the [official GitHub repo](https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/servicebus/Azure.Messaging.ServiceBus).
I created the following extension method to avoid repeating the boilerplate in every demo scenario. In a real-world application sending and receiving messages using the ServiceBusClient would not be hidden behind a single extension method.

```csharp
extension method
```

The default method described by the docs is to pass the ServiceBusConnection string to the ServiceBusClient and create it as needed.

```csharp
demo 1
```

> **Warning**
>
> Never store credentials in source control! 

## How to use Azure SDK without connection strings?
Working with secrets like our connection string provides extra overhead. Luckily this incarnation of the Azure SDK embraces token authentication via TokenCredential. For this, we need to install the package `Azure.Identity`. Using this method is the prefered method of authenticating the Azure SDK.
The easiest way to use this SDK is by creating a DefaultAzureCredential, which attempts to authenticate with a couple of common authentication mechanisms in order.

```csharp
snippet 2
```

Seeing the snippet, you might wonder how is providing yournamespace.servicebus.windows.net any better than a connection string? It's a good question; you still should not store something like that as plain text in source control. For one thing, it will probably be environment-specific. We still need it because we need an address so our application can communicate with Azure. The big difference here is that our address does not contain the key; the address alone is not enough to provide access to our resources.

Depending on how your organization handles roles and access management in Azure, you can now run this test and achieve the same result as before, without those pesky connection strings. 
For example, since I created a service bus, my user is the owner of that bus. Being the service bus instance owner is not enough to authenticate and successfully run our scenario. I require one of the service bus specific data roles. You can find a list of supported under Access Control (IAM) in the portal. I opted to use the "Azure Service Bus Data Owner" role for this tutorial.
The tricky bit is that role management in Azure is very granular. When I assign a role, I need to select a scope:
subscription
resourceGroup
resource (i.e. ServiceBusNamespace)
child resource (i.e. queue)
Scopes are inherited, so if I assign my user a role on a resource group, all resources (if applicable) in that resource group will provide me with the same access.

We can update our Azure CLI script to provide the logged-in user access to the resource.

```sh
cli snippet 2
```

## How can I use the Azure SDK with Dependency Injection?

One thing that always bothered me with the code I have shown so far is creating clients on the fly. I prefer to receive my service bus client from the dependency injection container. Discovering that this was a viable solution caused me to submit that PR to the Azure SDK repo. The team had already provided the normal ServiceBusClient, so I recreated the extension method to make ServiceBusAdministrationClient available via DI. It's time to install our third NuGet package, `Microsoft.Extensions.Azure` which provides the necessary bits.

After installing the package, we get the `AddAzureClients` extension method on IServiceCollection. It provides access to the `AzureClientFactoryBuilder` on which we can register everything Azure SDK related. In the case of ServiceBus we get `AddServiceBusClient` and `AddServiceBusClientWithNamespace`. I like that these methods are much more explicit than the constructor.

You might wonder why the FQN one does not need credentials this time around. That's because the Azure SDK can take care of this by default. As mentioned in the previous section, "DefaultAzureCredential" is the easiest way to hit the ground running. There are two ways we can customize this behaviour. We can either provide a default credential for all Azure Clients or on a per-client basis.