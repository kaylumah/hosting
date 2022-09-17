---
title: How to use Azurite for testing Azure Storage in dotnet?
description: ...
image: /assets/images/posts/20220917/azurite/cover_image.png
tags:
  - csharp
  - azure
  - testing
publishedtime: '23:30'
commentid: '137'
---
## How to test a dependency on Azure?

A recent project tasked me with integrating an application with Azure Blob Storage. Due to my post "Working with Azure SDK for .NET", I knew all about the current SDK to interface with Azure. The team in charge of the dotnet SDK has done a great job with providing accessible samples. My previous post did not focus on the testability aspects of the System, mainly because it was a simple demo for production code that is, of course, a no-go.

There are a few ways we can go about testing this:
1. Create a mock or fake implementation of every Storage API required.
2. Hide the blob implementation behind an internal interface and mock that in your tests.
3. Create a real storage account (per developer) in Azure.
4. Emulate storage account.

The argument to go with option 1 / 2 is that you, the developer, are not responsible for testings Azure's internal components. Option 3 has the challenge of cost and test repeatability. Because option three hosts the dependency externally, you need to set up and teardown for anything done in your tests. For example, you cannot create a file with the same name twice. Option 4 has the problem: any emulator does not guarantee to be 100% equal to the real deal.

While I agree with the argument for the first two options, the point here is to test if we can successfully integrate with Azure (as opposed to asserting their SDK works as expected). You can debate if testing with emulators or Azure is still a unit test. Using EntityFramework's DbContext in a test would warrant the same definition question.

> **Important**: if you only remember one thing from this post, let it be that every option except the third requires you to test in Azure. All other options are not the actual integration, and your application can behave differently once deployed.

## How can Azurite help by emulating Azure Storage?

The test solution I picked was using the popular open-source emulator called [Azurite](https://docs.microsoft.com/en-us/azure/storage/common/storage-use-azurite). The Azurite tool offers a local environment for Azure Blob, Azure Queue and Azure Table services. In the past, we also had Microsofts own Storage Account Emulator, but it appears that development on that has stopped, and the focussed shifted to Azurite.

There are several ways to run Azurite (i.e. Docker or NPM).

```shell
# install Azurite
npm install -g azurite

# run Azurite
azurite --silent --location c:\azurite --debug c:\azurite\debug.log
```

If we create a demo app and install blob storage via `dotnet add package Azure.Storage.Blobs`. We can connect with the following snippet:

```csharp
using Azure.Storage.Blobs;

var connectionString = "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";
var blobServiceClient = new BlobServiceClient(connectionString);
var properties = await blobServiceClient.GetPropertiesAsync().ConfigureAwait(false);
```

The snippet works because the connection string we provided is the default connection string for Azurite. It contains the default account known as `devstoreaccount1` and connects over HTTP. The default connection string also assumes you are running blob, queue and table services. For example, on NPM you could run:

```shell
# Run only Blob
azurite-blob --silent --location c:\azurite --debug c:\azurite\debug.log
# Run only Queue
azurite-queue --silent --location c:\azurite --debug c:\azurite\debug.log
# Run only Table
azurite-table --silent --location c:\azurite --debug c:\azurite\debug.log
```

Most would stop here because what I have shown so far is more than enough to use BlobServices from test code. It even works in [Azure Pipelines](https://docs.microsoft.com/en-us/azure/storage/blobs/use-azurite-to-run-automated-tests#run-tests-on-azure-pipelines). I, however, am still not entirely happy with it.

## How to use Azurite without a ConnectionString?

In the post ["Working with Azure SDK for .NET"](https://kaylumah.nl/2022/02/21/working-with-azure-sdk-for-dotnet.html) I made a point that connection strings should be a thing of the past. The `TokenCredential` should be the way forward (`dotnet add package Azure.Identity`).

```csharp
using Azure.Identity;
using Azure.Storage.Blobs;

var endpoint = new Uri("http://127.0.0.1:10000/devstoreaccount1");
var credential = new DefaultAzureCredential();
var blobServiceClient = new BlobServiceClient(endpoint, credential, new BlobClientOptions());
var properties = await blobServiceClient.GetPropertiesAsync().ConfigureAwait(false);
```

Based on the default configuration, the above snippet should have worked. However, you get the following error `System.ArgumentException: Cannot use TokenCredential without HTTPS.`

Azurite has an overload to provide HTTPS support. You can use a tool called mkcert to generate the required files.

```shell
# Run once
mkcert 127.0.0.1

# Run over HTTPs
azurite --silent --location c:\azurite --debug c:\azurite\debug.log --cert 127.0.0.1.pem --key 127.0.0.1-key.pem
```

Update the endpoint Uri to HTTPS:

```csharp
using Azure.Identity;
using Azure.Storage.Blobs;

var endpoint = new Uri("https://127.0.0.1:10000/devstoreaccount1");
var credential = new DefaultAzureCredential();
var blobServiceClient = new BlobServiceClient(endpoint, credential, new BlobClientOptions());
var properties = await blobServiceClient.GetPropertiesAsync().ConfigureAwait(false);
```

If we run our test example now, it will fail with the warning that an SSL connection cannot be established. We can solve this with generating a CA certificate from mkcert with `mkcert --install`. However, even with a valid SSL certificate TokenCredential will still fail. For TokenCredential to work we need to pass `--oath basic` to Azurite.

```shell
azurite --silent --location c:\azurite --debug c:\azurite\debug.log --cert 127.0.0.1.pem --key 127.0.0.1-key.pem --oauth basic
```

## Can I use Azurite HTTPS connection string in CICD pipelines?

Now that we can use TokenCredential, I am happy. The test instance of our BlobServiceClient is almost identical to the production configuration. We have established that it works locally, but how about a CICD environment? I modified the example pipeline to add the mkcert bits.

```yaml
steps:
  - bash: |
      choco install mkcert
      npm install -g azurite
      mkdir azurite
      cd azurite
      mkcert --install
      mkcert 127.0.0.1
      azurite --oauth basic --cert 127.0.0.1.pem --key 127.0.0.1-key.pem --silent --location data --debug data\debug.log &
    displayName: "Install and Run Azurite"
```

Unfortunately, adding a certificate to the trust store requires a password prompt. On an Azure-hosted agent, this does not work and causes the agent [to be stuck](https://github.com/FiloSottile/mkcert/issues/286). To me, this could mean one of two things. Either the Azure team does not test over HTTPS, or they have a different set of test tooling. As it turns out, they have a set of helpers to construct the service clients and disable SSL verification. Like this:

```csharp
using Azure.Core.Pipeline;
using Azure.Identity;
using Azure.Storage.Blobs;

var endpoint = new Uri("https://127.0.0.1:10000/devstoreaccount1");
var credential = new DefaultAzureCredential();
var blobServiceClient = new BlobServiceClient(endpoint, credential, new BlobClientOptions()
{
    Transport = new HttpClientTransport(new HttpClient(new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    }))
});
var properties = await blobServiceClient.GetPropertiesAsync().ConfigureAwait(false);
```

It can now be used in Azure Pipelines like this (note the lack of mkcert --install)

```yaml
steps:
  - bash: |
      choco install mkcert
      npm install -g azurite
      mkdir azurite
      cd azurite
      mkcert 127.0.0.1
      azurite --oauth basic --cert 127.0.0.1.pem --key 127.0.0.1-key.pem --silent --location data --debug data\debug.log &
    displayName: "Install and Run Azurite"
```

## How to use Azurite in my project?

The testing helpers have more to it than disabling SSL but are not present on NuGet. So naturally, I raised [an issue](https://github.com/Azure/azure-sdk-for-net/issues/30751) to the SDK team if they have any plans in that direction. Unfortunately, at this point, they have no interest in releasing their internal test tooling. The techniques I mentioned thus far can be used standalone. I, however, felt this was an excellent opportunity to create my first NuGet Package. The package cannot assume how anybody runs Azurite, so I introduced two classes. You can use `AzuriteAccountBuilder` to configure how things are run, like the account or the ports being used. The `AzuriteAccount` class provides access to stuff like the connection string. For convenience the package also creates helper methods to create `BlobServiceClient`, `TableServiceClient` or `QueueServiceClient` form an `AzuriteAccount`.

My package is designed for use in a test project so let us create a new test project (i.e. `dotnet new xunit`) and add my package to it `dotnet add package Kaylumah.Testing.Azurite --version 1.0.0`.
The most explicit way to create an AzuriteAccount is with the following code:

```csharp
[Fact]
public async Task Test1()
{
    var account = new AzuriteAccountBuilder()
        .WithProtocol(secure: false)
        .WithDefaultAccount()
        .WithDefaultBlobEndpoint()
        .WithDefaultQueueEndpoint()
        .WithDefaultTableEndpoint()
        .Build();

    var blobServiceClient = account.CreateBlobServiceViaConnectionString();
    await blobServiceClient.GetPropertiesAsync();
}
```

The snippet above creates a connection string based on the default settings. That means it should match the connection string when someone runs Azurite without parameters. For convenience, I have also added a helper class that creates this default account for you.

```csharp
[Fact]
public async Task Test2()
{
    var account = AzuriteHelper.CreateDefaultAzuriteAccountBuilder().Build();
    var blobServiceClient = account.CreateBlobServiceViaSharedKeyCredential();
    await blobServiceClient.GetPropertiesAsync();
}
```

The package offers the same convenience helpers for Queue and Table storage. You can use the connection string, shared key, azure sas key or token credential with the helpers.

## Closing Thoughts

I started this journey with knowledge about Azurite and the dotnet SDK for Azure. I knew from experience that I no longer wanted to work with managed identity instead of connection strings. I needed a way to have repeatable tests on local and CI/CD environments. The funny thing is that after I had everything working the way I wanted, I could not use the required API I needed. For a moment, I had forgotten Azurite is an emulator, and not all features are supported. So I had to fall back on shared key credentials, which work fine over HTTPS and could already be used in pipelines. Luckily I designed the package to work with a variety of configurations.

Usually, this is where I post a link to the posts GitHub repo. This time, the source code is the NuGet package on this [GitHub Repo](https://github.com/kaylumah/Kaylumah.Testing.Azurite). As always, if you have any questions, feel free to reach out. Do you have suggestions or alternatives? I would love to hear about them. Especially since this is my first NuGet package let me know if it helped you out.