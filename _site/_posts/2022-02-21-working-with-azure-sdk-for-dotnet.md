---
title: "Working with Azure SDK for .NET"
description: "The latest iteration of the Azure SDK for dotnet has several cool features baked into its design. We take a look at some common scenarios"
cover_image:
    DEFAULT: '/assets/images/posts/20220221/working-with-azure-sdk-for-dotnet/cover_image.png'
    WEB: '/assets/images/posts/20220221/working-with-azure-sdk-for-dotnet/cover_image.webp'
image: '/assets/images/posts/20220221/working-with-azure-sdk-for-dotnet/cover_image.png'
tags:
  - "csharp"
  - "azure"
comment_id: '70'
publishedtime: '21:30'
---
February 2022 marks the 20th anniversary of the dotnet platform, which is quite a milestone. I found it the perfect time to reflect; I have been working professionally for almost six years and using .NET during the four years before that in my studies. For a dotnet blogger like myself, I could not stand idly by and let this pass without a post. February 2022 also marks another milestone for me. My first ever open-source contribution has been released into the wild. I made a [small contribution](https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/servicebus/Azure.Messaging.ServiceBus/CHANGELOG.md#760-2022-02-08) to the `Azure SDK for .NET`. So in honour of both, I wrote this article with small tips and tricks I picked up when working with the SDK.

## Which Azure SDK should I use?
Since [July 2019](https://devblogs.microsoft.com/azure-sdk/state-of-the-azure-sdk-2021/), Microsoft has made a design effort to unify the SDKs for the different services. There are shared concepts between the libraries like authentication and diagnostics. The libraries follow the pattern `Azure.{service}.{library}`.
My contribution was to the ServiceBus SDK, so today's article focus is the service bus. Almost everything described is transferable to the other SDKs; only a few bits are ServiceBus specific. The NuGet package we need is the `Azure.Messaging.Service` package.

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
AzureSubscriptionId="<subscription-id>"
AzureTenantId="<tenant-id>"
AzureResourceGroup="demorg001"
AzureLocation="westeurope"

# Sign in to Azure using device code - After login session is scoped to Subscription in Tenant
az login --use-device-code --tenant $AzureTenantId
az account set --subscription $AzureSubscriptionId

# Set default values for location and resource group
az config set defaults.location=$AzureLocation defaults.group=$AzureResourceGroup

# Create resource group and capture resource group identifier
ResourceGroupId=$(az group create --name $AzureResourceGroup --query "id" --output tsv)

# Generate Unique ID based on ResourceGroupId
UniqueId=$(echo -n $ResourceGroupId | md5sum | cut -c-13)

# Create ServiceBus and Queue
ServiceBusNamespace="sbdemo0001$UniqueId"
QueueName="demoqueue"
echo "Going to create ServiceBus $ServiceBusNamespace and Queue $QueueName"
AzureServiceBusId=$(az servicebus namespace create --name $ServiceBusNamespace --sku Basic --query id -o tsv)
AzureServiceBusQueueId=$(az servicebus queue create --name $QueueName --namespace-name $ServiceBusNamespace --default-message-time-to-live P0Y0M0DT0H0M30S --query id -o tsv)

# Fetch ServiceBus Connectionstring
PrimaryConnectionString=$(az servicebus namespace authorization-rule keys list \
    --namespace-name $ServiceBusNamespace \
    --name "RootManageSharedAccessKey" \
    --query "primaryConnectionString" \
    --output tsv)

echo "$PrimaryConnectionString"
```

> **Note**
>
> The above snippet uses the default generated RootManageSharedAccessKey, which provides full access to your servicebus so use with caution!

## How does the Azure Service Bus SDK work?
A message bus is dependent on both a sender and receiver for communication. There are many examples in the [official GitHub repo](https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/servicebus/Azure.Messaging.ServiceBus), so I won't go into much more details regarding the bus itself.

This demo will focus on SDK features, so I created an Xunit project that runs multiple scenarios. Since all scenarios require some logic to communicate with the bus, I made the following extension method to avoid unnecessary boilerplate. In a real-world application sending and receiving messages using the ServiceBusClient would not be hidden behind a single extension method.

```csharp
using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using FluentAssertions;

namespace Test.Integration;

public static partial class ServiceBusClientTestExtensions
{
    public static async Task RunScenario(this ServiceBusClient client, string queueName, string scenarioName)
    {
        var sender = client.CreateSender(queueName);
        var receiver = client.CreateReceiver(queueName);

        var message = $"{scenarioName}-{DateTimeOffset.Now:s}";
        await sender.SendMessageAsync(new ServiceBusMessage(message));
        var receivedMessage = await receiver.ReceiveMessageAsync();

        receivedMessage.Body.ToString().Should().Be(message);
        await Task.Delay(TimeSpan.FromSeconds(35));
    }
}
```

The default method described by the docs is to pass the ServiceBusConnection string to the ServiceBusClient and create it as needed.

```csharp
public class UnitTest1
{
    private const string ConnectionString = "<your-connectionstring>";
    private const string QueueName = "demoqueue";

    [Fact]
    public async Task Test_Scenario01_UsePrimaryConnectionString()
    {
        await using var client = new ServiceBusClient(ConnectionString);
        var scenario = async () => await client.RunScenario(QueueName, nameof(Test_Scenario01_UsePrimaryConnectionString));
        await scenario();
    }
}
```

> **Warning**
>
> Never store credentials in source control! 

## How to use Azure SDK without connection strings?
Working with secrets like our connection string provides extra overhead. Luckily this incarnation of the Azure SDK embraces token authentication via TokenCredential. For this, we need to install the package `Azure.Identity`. Using this method is the preferred method of authenticating the Azure SDK.
The easiest way to use this SDK is by creating a `DefaultAzureCredential`, which attempts to authenticate with a couple of common authentication mechanisms in order.

1. Environment
2. Managed Identity
3. Visual Studio
4. Azure CLI
5. Azure Powershell

```csharp
public class UnitTest1
{
    private const string FullyQualifiedNamespace = "<your-namespace>.servicebus.windows.net";
    private const string QueueName = "demoqueue";

    [Fact]
    public async Task Test_Scenario02_UseFullyQualifiedNamespace()
    {
        await using var client = new ServiceBusClient(FullyQualifiedNamespace, new DefaultAzureCredential());
        var scenario = async () => await client.RunScenario(QueueName, nameof(Test_Scenario02_UseFullyQualifiedNamespace));
        await scenario();
    }
}
```

Seeing the snippet, you might wonder how is providing `your-namespace.servicebus.windows.net` any better than a connection string? It's a good question; you still should not store something like that as plain text in source control. For one thing, it will probably be environment-specific. We still need it because we need an address so our application can communicate with Azure. The big difference here is that our address does not contain the key; the address alone is not enough to provide access to our resources.

Depending on how your organization handles roles and access management in Azure, you can now run this test and achieve the same result as before, without those pesky connection strings. 
For example, since I created a service bus, my user is the owner of that bus. Being the service bus instance owner is not enough to authenticate and successfully run our scenario. I require one of the service bus specific data roles. You can find a list of supported under `Access Control (IAM)` in the portal. I opted to use the `"Azure Service Bus Data Owner"` role for this tutorial.
The tricky bit is that role management in Azure is very granular. When I assign a role, I need to select a scope:
- subscription
- resourceGroup
- resource (i.e. ServiceBusNamespace)
- child resource (i.e. queue)

Scopes are inherited, so if I assign my user a role on a resource group, all resources (if applicable) in that resource group will provide me with the same access.

We can update our Azure CLI script to provide the logged-in user access to the resource.

```sh
# Assign Role "Azure Service Bus Data Owner" for the current user
UserIdentity=$(az ad signed-in-user show --query objectId -o tsv)
az role assignment create --assignee $UserIdentity --role "Azure Service Bus Data Owner" --scope $AzureServiceBusId
```

Now you know why the previous script captured the AzureServiceBusId ;-)

One thing to note is that DefaultAzureCredential's intended use is to simplify getting started with development. In a real-world application, you would probably need a custom ChainedTokenCredential that uses ManagedIdentityCredential for production and AzureCliCredential for development.

## How can I use the Azure SDK with Dependency Injection?
One thing that always bothered me with the code I have shown so far is creating clients on the fly. I prefer to receive my service bus client from the dependency injection container. Discovering that this was a viable solution caused me to submit that PR to the Azure SDK repo. The team had already provided the normal ServiceBusClient, so I recreated the extension method to make ServiceBusAdministrationClient available via DI. It's time to install our third NuGet package, `Microsoft.Extensions.Azure` which provides the necessary bits.

After installing the package, we get the `AddAzureClients` extension method on IServiceCollection. It provides access to the `AzureClientFactoryBuilder` on which we can register everything Azure SDK related. In the case of ServiceBus we get `AddServiceBusClient` and `AddServiceBusClientWithNamespace`. I like that these methods are much more explicit than the constructor.

```csharp
public class UnitTest1
{
    private const string FullyQualifiedNamespace = "<your-namespace>.servicebus.windows.net";
    private const string ConnectionString = "<your-connectionstring>";
    private const string QueueName = "demoqueue";

    [Fact]
    public async Task Test_Scenario03_UseDependencyInjectionWithPrimaryConnectionString()
    {
        var services = new ServiceCollection();
        services.AddAzureClients(builder => {
            builder.AddServiceBusClient(ConnectionString);
        });
        var serviceProvider = services.BuildServiceProvider();
        var client = serviceProvider.GetRequiredService<ServiceBusClient>();
        var scenario = async () => await client.RunScenario(QueueName, nameof(Test_Scenario03_UseDependencyInjectionWithPrimaryConnectionString));
        await scenario();
    }

    [Fact]
    public async Task Test_Scenario04_UseDependencyInjectionWithFullyQualifiedNamespace()
    {
        var services = new ServiceCollection();
        services.AddAzureClients(builder => {
            builder.AddServiceBusClientWithNamespace(FullyQualifiedNamespace);
        });
        var serviceProvider = services.BuildServiceProvider();
        var client = serviceProvider.GetRequiredService<ServiceBusClient>();
        var scenario = async () => await client.RunScenario(QueueName, nameof(Test_Scenario04_UseDependencyInjectionWithFullyQualifiedNamespace));
        await scenario();
    }
}
```

You might wonder why the `FullyQualifiedNamespace` one does not need credentials this time around. That's because the Azure SDK can take care of this by default. As mentioned in the previous section, `DefaultAzureCredential` is the easiest way to hit the ground running. There are two ways we can customize this behaviour. We can either provide a default credential for all Azure Clients or on a per-client basis.

```csharp
public class UnitTest1
{
    private const string FullyQualifiedNamespace = "<your-namespace>.servicebus.windows.net";
    private const string ConnectionString = "<your-connectionstring>";
    private const string QueueName = "demoqueue";

    [Fact]
    public async Task Test_Scenario05_DependencyInjectionChangeDefaultToken()
    {
        var services = new ServiceCollection();
        services.AddAzureClients(builder => {
            builder.AddServiceBusClientWithNamespace(FullyQualifiedNamespace);
            
            builder.UseCredential(new ManagedIdentityCredential());
        });
        var serviceProvider = services.BuildServiceProvider();
        var client = serviceProvider.GetRequiredService<ServiceBusClient>();
        var scenario = async () => await client.RunScenario(QueueName, nameof(Test_Scenario05_DependencyInjectionChangeDefaultToken));
        await scenario.Should().ThrowAsync<CredentialUnavailableException>();
    }

    [Fact]
    public async Task Test_Scenario06_DependencyInjectionChangeDefaultTokenOnClientLevel()
    {
        var services = new ServiceCollection();
        services.AddAzureClients(builder => {
            builder.AddServiceBusClientWithNamespace(FullyQualifiedNamespace)
                .WithCredential(new AzureCliCredential());
            
            builder.UseCredential(new ManagedIdentityCredential());
        });
        var serviceProvider = services.BuildServiceProvider();
        var client = serviceProvider.GetRequiredService<ServiceBusClient>();
        var scenario = async () => await client.RunScenario(QueueName, nameof(Test_Scenario06_DependencyInjectionChangeDefaultTokenOnClientLevel));
        await scenario();
    }
}
```

The first sample will not work since I have not set up ManagedIdentity in my environment. The second one also sets ManagedIdentityCredential as the default credential. However, since I set up AzureCliCredential on the client registration, it trumps the global one.

## Can we have different client config when using the Azure SDK?
Here is where things get cool. When you register a client with the SDK, a client named `Default` gets registered. If, for example, you retrieve `ServiceBusClient` from the dependency injection, what happens is that the AzureClientFactoy creates this client for you.

In the case of servicebus, you might have multiple different namespaces registered. Every registration provides access to a method `WithName`. To use named clients in your code, replace `ServiceBusClient` with `IAzureClientFactory<ServiceBusClient`.

```csharp
public class UnitTest1
{
    private const string FullyQualifiedNamespace = "<your-namespace>.servicebus.windows.net";
    private const string ConnectionString = "<your-connectionstring>";
    private const string QueueName = "demoqueue";

    [Fact]
    public async Task Test_Scenario07_MultipleClients()
    {
        var services = new ServiceCollection();
        services.AddAzureClients(builder =>
        {
            builder.AddServiceBusClient(ConnectionString);

            builder.AddServiceBusClientWithNamespace(FullyQualifiedNamespace)
                .WithName("OtherClient");
        });
        var serviceProvider = services.BuildServiceProvider();
        var clientFactory = serviceProvider.GetRequiredService<IAzureClientFactory<ServiceBusClient>>();
        
        var clientDefault = clientFactory.CreateClient("Default");
        var scenarioDefaultClient = async () => await clientDefault.RunScenario(QueueName, nameof(Test_Scenario07_MultipleClients) + "A");
        await scenarioDefaultClient();
        
        var otherClient = clientFactory.CreateClient("OtherClient");
        var scenarioOtherClient = async () => await otherClient.RunScenario(QueueName, nameof(Test_Scenario07_MultipleClients) + "B");
        await scenarioOtherClient();
    }
}
```

## Can I use configuration to create Azure SDK clients?
If I had one criticism of the SDK, it would be that the extension methods require the address right there in the call to the method. To be fair, there is an overload that uses IConfiguration, but that leaves everything up to the SDK to validate.

In my [previous article on validating IOptions](https://kaylumah.nl/2021/11/29/validated-strongly-typed-ioptions.html), I wrote about a way to make sure all configuration for my app is valid. 

That approach, of course, requires access to the dependency injection container. Luckily there is an additional method available.

```csharp
public class UnitTest1
{
    private const string FullyQualifiedNamespace = "<your-namespace>.servicebus.windows.net";
    private const string QueueName = "demoqueue";

    [Fact]
    public async Task Test_Scenario08_StronglyTypedOptions()
    {
        var services = new ServiceCollection();
        services.Configure<DemoOptions>(options =>
        {
            options.ServiceBusNamespace = FullyQualifiedNamespace;
        });
        services.AddAzureClients(builder =>
        {
            builder.AddClient<ServiceBusClient, ServiceBusClientOptions>((options, credential, provider) =>
            {
                var demoOptions = provider.GetRequiredService<IOptions<DemoOptions>>();
                return new ServiceBusClient(demoOptions.Value.ServiceBusNamespace, credential, options);
            });
        });
        var serviceProvider = services.BuildServiceProvider();
        var client = serviceProvider.GetRequiredService<ServiceBusClient>();
        var scenario = async () => await client.RunScenario(QueueName, nameof(Test_Scenario08_StronglyTypedOptions));
        await scenario();
    }
}
```

## Closing Thoughts
A single blog is too short for providing an overview of everything the Azure SDK offers. I like that authentication and interoperability with the dependency injection container are baked into the SDK. I have not even touched on diagnostics and testability, which are both great topics built into the entire SDK. Who knows, perhaps that is a topic for another time.

As always, if you have any questions, feel free to reach out. Do you have suggestions or alternatives? I would love to hear about them.

The corresponding source code for this article is on [GitHub](https://github.com/kaylumah/WorkingWithAzureSdkForDotnet).

See you next time, stay healthy and happy coding to all 🧸!

## Additional Resources

- [Azure SDK for Dotnet on Microsoft Docs](https://docs.microsoft.com/en-us/dotnet/azure/sdk/azure-sdk-for-dotnet)
- [Azure.Messaging.ServiceBus on GitHub](https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/servicebus/Azure.Messaging.ServiceBus/README.md)
- [Azure.Core on GitHub](https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/core/Azure.Core/README.md)
- [Azure.Identity on GitHub](https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/identity/Azure.Identity/README.md)
- [Microsoft.Extensions.Azure on GitHub](https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/extensions/Microsoft.Extensions.Azure/README.md)
- [Service bus on Microsoft Docs](https://docs.microsoft.com/en-us/azure/service-bus-messaging)
- [Best practices Azure SDK](https://devblogs.microsoft.com/azure-sdk/best-practices-for-using-azure-sdk-with-asp-net-core)
- [Best practices Azure CLI](https://docs.microsoft.com/en-gb/cli/azure/use-cli-effectively)