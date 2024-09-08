---
title: Capture Logs in Unit Tests
description: A guide to capturing logs in Xunit
image: /assets/images/posts/20211114/capture-logs-in-unit-tests/cover_image.png
tags:
  - csharp
  - testing
  - xunit
publishedtime: '20:30'
commentid: '141'
---
In application code, we are used to writing log statements primarily for diagnostic purposes. For instance, we use logs to capture unexpected error flows. Therefore it is not uncommon to want to capture the log output in our unit tests. You have three distinctive options to handle log output in unit tests, as far as I can tell.

## Scenario

Our test scenario is a service or system under test (SUT) that takes a string input and returns it without modification. We rely on `Microsoft Extensions` for our logging purposes. As the test framework, we will be using `Xunit`.

```cs
public interface IEchoService
{
    Task<string> Echo(string input);
}
```

The initial implementation of our SUT could look like this:

```cs
public class EchoService : IEchoService
{
    private readonly ILogger<EchoService> _logger;

    public EchoService(ILogger<EchoService> logger)
    {
        _logger = logger;
    }

    public Task<string> Echo(string input)
    {
        _logger.LogInformation("echo was invoked");
        return Task.FromResult(input);
    }
}
```

For this article, the snippet above would be more than sufficient. But in a real-life application, I prefer to log the input as well. If, however, we would use simple string interpolation, we immediately get a Code-Analysis warning about it. The recommendation here is to use LoggerMessage that enables the use of [high-performance logging](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/loggermessage?view=aspnetcore-6.0). I've always found that implementing the LoggerMessage pattern required quite a bit of boilerplate. Luckily in .NET 6, this is a lot easier. We can [generate](https://docs.microsoft.com/en-us/dotnet/core/extensions/logger-message-generator) all the boilerplate we need. As per usual, Andrew Lock [wrote a piece](https://andrewlock.net/exploring-dotnet-6-part-8-improving-logging-performance-with-source-generators/) about this new feature already.

After applying our `LoggerMessage` changes to the SUT it looks like the snippet below. Please note that in order for this to work the class `EchoService` it self is now marked as `partial`.

```cs
public partial class EchoService : IEchoService
{
    private readonly ILogger<EchoService> _logger;

    public EchoService(ILogger<EchoService> logger)
    {
        _logger = logger;
    }

    public Task<string> Echo(string input)
    {
        //_logger.LogInformation("echo was invoked");

        // The logging message template should not vary between calls to ... csharp(CA2254)
        // _logger.LogInformation($"echo was invoked with {input}");

        LogEchoCall(input);

        return Task.FromResult(input);
    }

    [LoggerMessage(1000, LogLevel.Information, "echo was invoked '{EchoInput}'")]
    partial void LogEchoCall(string echoInput);
}
```

## Option 1
First up is doing absolutely nothing. Yeah, you read that correctly. You might find it silly to start this piece with the first option being nothing, but doing nothing with log statements in your test code is perfectly fine. Heck, even doing nothing comes in two flavours.

If we use Dependency Injection in our test, we have access to "AddLogging()". If we don't provide a logging provider, our code will run just fine. Otherwise, if you have already set up a logging provider or provided one explicitly, it will log to zero or more providers depending on your current configuration. For instance, you could use the ConsoleLoggerProvider to log to the console during the test. I often use the DI variant in my test since I am writing extension methods on IServiceCollection to write up my code anyway, so using the same extension method in test code simplifies matters.

```cs
[Fact]
public async Task Test_DependencyInjection_EmptyLoggingBuilder()
{
    var configuration = new ConfigurationBuilder().Build();
    var serviceProvider = new ServiceCollection()
        .AddLogging() // could also be part of AddEcho to make sure ILogger is available outside ASP.NET runtime
        .AddEcho(configuration)
        .BuildServiceProvider();
    var sut = serviceProvider.GetRequiredService<IEchoService>();
    var testInput = "Scenario: empty logging builder";
    var testResult = await sut.Echo(testInput).ConfigureAwait(false);
    testResult.Should().Be(testInput, "the input should have been returned");
}
```

![VS Code - Dotnet Debugger - No ILogger Registered](/assets/images/posts/20211114/capture-logs-in-unit-tests/001_NoLogger.png){width=1564 height=814}

```cs
[Fact]
public async Task Test_DependencyInjection_ConsoleLoggingBuilder()
{
    var configuration = new ConfigurationBuilder().Build();
    var serviceProvider = new ServiceCollection()
        .AddLogging(loggingBuilder => {
            loggingBuilder.AddConsole();
        })
        .AddEcho(configuration)
        .BuildServiceProvider();
    var sut = serviceProvider.GetRequiredService<IEchoService>();
    var testInput = "Scenario: console logging builder";
    var testResult = await sut.Echo(testInput).ConfigureAwait(false);
    testResult.Should().Be(testInput, "the input should have been returned");
}
```

![VS Code - Dotnet Debugger - console ILogger registered](/assets/images/posts/20211114/capture-logs-in-unit-tests/002_ConsoleLogger.png){width=1564 height=814}

If, however, you cannot rely on dependency injection in your tests, you have the alternative of manual creating your SUT and relevant dependencies. The only dependency of our EchoService is an instance of ILogger. For testing purposes, you can use the NullLoggerFactory, which creates a logger that logs into the void.

```cs
[Fact]
public async Task Test_Manuel_NullLoggingFactory()
{
    var sut = new EchoService(NullLogger<EchoService>.Instance);
    var testInput = "Scenario: null logger factory";
    var testResult = await sut.Echo(testInput).ConfigureAwait(false);
    testResult.Should().Be(testInput, "the input should have been returned");
}
```

![VS Code - Dotnet Debugger - null ILogger registered](/assets/images/posts/20211114/capture-logs-in-unit-tests/003_NullLogger.png){width=1564 height=814}

> As you can see in the screenshot above, and empty logger and a NullLogger are not the same thing.

## Option 2
The second method uses the Moq framework, which makes it possible to hide the logger behind a Mock, which means it's a fake version of ILogger. In my previous article, ["Adventures with Mock"](/2021/04/11/an-approach-to-writing-mocks.html), I touched upon my preferred method of writing mocks. I even included an initial version of the LoggerMock. Since then, I have fleshed out the concept more, so here is an updated version of the Logger Mock.

```cs
public class LoggerMock<TCategoryName> : Mock<ILogger<TCategoryName>>
{
    private readonly List<LogMessage> logMessages = new();

    public ReadOnlyCollection<LogMessage> LogMessages => new(logMessages);

    protected LoggerMock()
    {
    }

    public static LoggerMock<TCategoryName> CreateDefault()
    {
        return new LoggerMock<TCategoryName>()
            .SetupLog()
            .SetupIsEnabled(LogLevel.Information);
    }

    public LoggerMock<TCategoryName> SetupIsEnabled(LogLevel logLevel, bool enabled = true)
    {
        Setup(x => x.IsEnabled(It.Is<LogLevel>(p => p.Equals(logLevel))))
            .Returns(enabled);
        return this;
    }

    public LoggerMock<TCategoryName> SetupLog()
    {
        Setup(logger => logger.Log(
            It.IsAny<LogLevel>(),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => true),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)
        ))
        .Callback(new InvocationAction(invocation => {
            var logLevel = (LogLevel)invocation.Arguments[0];
            var eventId = (EventId)invocation.Arguments[1];
            var state = invocation.Arguments[2];
            var exception = (Exception?)invocation.Arguments[3];
            var formatter = invocation.Arguments[4];

            var invokeMethod = formatter.GetType().GetMethod("Invoke");
            var actualMessage = (string?)invokeMethod?.Invoke(formatter, new[] { state, exception });

            logMessages.Add(new LogMessage {
                EventId = eventId,
                LogLevel = logLevel,
                Message = actualMessage,
                Exception = exception,
                State = state
            });
        }));
        return this;
    }
}
```

Any `Mock` created with Moq will provide you with the ability to assert invocations made to the mocked class. Since my approach makes the mock stateful, I can capture any request made against it. We can make concrete assertions because we can access information like `EventId` and `LogLevel`. If, for instance, you have alerts written against business events, you want to validate that the correct information passes into your logging system.

```cs
[Fact]
public async Task Test_Moq_DefaultMockedLogger()
{
    var loggerMock = LoggerMock<EchoService>.CreateDefault();
    var sut = new EchoService(loggerMock.Object);
    var testInput = "Scenario: mocked logger";
    var testResult = await sut.Echo(testInput).ConfigureAwait(false);
    testResult.Should().Be(testInput, "the input should have been returned");

    loggerMock.LogMessages.Should().NotBeEmpty().And.HaveCount(1);
    loggerMock.VerifyEventWasLogged(new EventId(1000));
}

[Fact]
public async Task Test_Moq_LogLevelDisabledMockedLogger()
{
    var loggerMock = LoggerMock<EchoService>.CreateDefault().SetupIsEnabled(LogLevel.Information, enabled: false);
    var sut = new EchoService(loggerMock.Object);
    var testInput = "Scenario: log level disabled mocked logger";
    var testResult = await sut.Echo(testInput).ConfigureAwait(false);
    testResult.Should().Be(testInput, "the input should have been returned");

    loggerMock.LogMessages.Should().BeEmpty();
}
```

![VS Code - Dotnet Debugger - mock ILogger registered](/assets/images/posts/20211114/capture-logs-in-unit-tests/004_MockLogger.png){width=1564 height=814}

## Options 3
Thus far, we have discussed options that would work outside `Xunit`. The third technique is not limited to `Xunit`, but its implementation is restricted to use in a `Xunit` project because we will now rely on Xunit's `ITestOutputHelper` mechanism. In most cases, we would use `ITestOutputHelper` to log lines inside the test case itself; it is, however, possible to create an `ILogger` that writes to `ITestOutputHelper` so we can also capture logs our SUT produces.

Microsoft has [well-written documentation](https://docs.microsoft.com/en-us/dotnet/core/extensions/custom-logging-provider) on how to create a custom logger provider. We start with a configuration class for our `XunitLogger`. We will have no custom settings in this demo, but putting the configuration in place makes it easier to add settings later. The `ConsoleLogger`, for example, uses configuration to control LogScope inclusion and timestamp formats.

```cs
public class XunitLoggerConfiguration
{
}
```

Next up is our Xunit logger itself. The ColoredConsole sample from the docs does nothing with scope, but to not limit ourselves later, we changed the implementation of `BeginScope` to use `IExternalScopeProvider`. To print the log line, we need the last argument of `Log<TState>`, which is the formatter. We then pass it the Xunit's ITestOutputHelper to [capture output](https://xunit.net/docs/capturing-output). Depending on your specific needs, you can log the logger's category (name), event, log level, scope or even exception. For now, let's keep it simple.

```cs
public class XunitLogger : ILogger
{
    private readonly string _loggerName;
    private readonly Func<XunitLoggerConfiguration> _getCurrentConfig;
    private readonly IExternalScopeProvider _externalScopeProvider;
    private readonly ITestOutputHelper _testOutputHelper;

    public XunitLogger(string loggerName, Func<XunitLoggerConfiguration> getCurrentConfig, IExternalScopeProvider externalScopeProvider, ITestOutputHelper testOutputHelper)
    {
        _loggerName = loggerName;
        _getCurrentConfig = getCurrentConfig;
        _externalScopeProvider = externalScopeProvider;
        _testOutputHelper = testOutputHelper;
    }

    public IDisposable BeginScope<TState>(TState state) => _externalScopeProvider.Push(state);

    public bool IsEnabled(LogLevel logLevel) => LogLevel.None != logLevel;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

         var message = formatter(state, exception);
         _testOutputHelper.WriteLine(message);
    }
}
```

An `ILoggerProvider` is responsible for creating `ILogger` instances; this means we also need the custom `XunitLoggerProvider` to take care of making our `XunitLogger`.

```cs
public sealed class XunitLoggerProvider : ILoggerProvider
{
    private readonly IDisposable _configurationOnChangeToken;
    private XunitLoggerConfiguration _currentConfiguration;
    private readonly ConcurrentDictionary<string, XunitLogger> _loggers = new();
    private readonly IExternalScopeProvider _externalScopeProvider = new LoggerExternalScopeProvider();
    private readonly ITestOutputHelper _testOutputHelper;

    public XunitLoggerProvider(IOptionsMonitor<XunitLoggerConfiguration> optionsMonitor, ITestOutputHelper testOutputHelper)
    {
        _currentConfiguration = optionsMonitor.CurrentValue;
        _configurationOnChangeToken = optionsMonitor.OnChange(updatedConfiguration => _currentConfiguration = updatedConfiguration);
        _testOutputHelper = testOutputHelper;
    }

    public ILogger CreateLogger(string categoryName)
    {
        var logger = _loggers.GetOrAdd(categoryName, name => new XunitLogger(name, GetCurrentConfiguration, _externalScopeProvider, _testOutputHelper));
        return logger;
    }

    public void Dispose()
    {
        _loggers.Clear();
        _configurationOnChangeToken.Dispose();
    }

    private XunitLoggerConfiguration GetCurrentConfiguration() => _currentConfiguration;
}
```

The final puzzle piece is an extension method that allows us to register the new logger type. Note that we also add `ITestOutputHelper` to the DI container of the LoggingBuilder; that is why the `XunitLoggingProvider` in the previous snippet can retrieve it from the dependency injection container.

```cs
public static class XunitLoggingBuilderExtensions
{
    public static ILoggingBuilder AddXunit(this ILoggingBuilder builder, ITestOutputHelper testOutputHelper)
    {
        builder.AddConfiguration();

        builder.Services.TryAddSingleton(testOutputHelper);

        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Singleton<ILoggerProvider, XunitLoggerProvider>());

        LoggerProviderOptions.RegisterProviderOptions
            <XunitLoggerConfiguration, XunitLoggerProvider>(builder.Services);

        return builder;
    }

    public static ILoggingBuilder AddXunit(this ILoggingBuilder builder, ITestOutputHelper testOutputHelper, Action<XunitLoggerConfiguration> configure)
    {
        builder.AddXunit(testOutputHelper);
        builder.Services.Configure(configure);

        return builder;
    }
}
```

The usage is the same as the ConsoleLogger example we did previously.

```cs
[Fact]
public async Task Test_Custom_XunitLoggingBuilder()
{
    var configuration = new ConfigurationBuilder().Build();
    var serviceProvider = new ServiceCollection()
        .AddLogging(loggingBuilder => {
            loggingBuilder.AddXunit(_testOutputHelper);
        })
        .AddEcho(configuration)
        .BuildServiceProvider();
    var sut = serviceProvider.GetRequiredService<IEchoService>();
    var testInput = "Scenario: custom logging builder";
    var testResult = await sut.Echo(testInput).ConfigureAwait(false);
    testResult.Should().Be(testInput, "the input should have been returned");
}
```

![VS Code - Dotnet Debugger - Xunit ILogger registered](/assets/images/posts/20211114/capture-logs-in-unit-tests/005_XunitLogger.png){width=1564 height=814 }

The first time I ran this test, I was baffled. I could only see the console output from ConsoleLogger test we did previously. A quick google search brought me to the [solution](https://github.com/xunit/xunit/issues/1141#issuecomment-555717377). We need to tell the dotnet test runner to display it with `dotnet test --logger:"console;verbosity=detailed"`. Telling an entire team they can no longer simply run `dotnet test` was not a real solution; luckily, we can simplify things with `dotnet test --settings runsettings.xml`.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<RunSettings>
    <LoggerRunSettings>
        <Loggers>
            <Logger friendlyName="console" enabled="True">
                <Configuration>
                    <Verbosity>detailed</Verbosity>
                </Configuration>
            </Logger>
        </Loggers>
    </LoggerRunSettings>
</RunSettings>
```

However, explicitly passing `--settings` every time does not solve anything. On the [Microsoft Docs](https://docs.microsoft.com/en-us/visualstudio/test/configure-unit-tests-by-using-a-dot-runsettings-file?view=vs-2022) I found the solution. We can tell MSBuild to use `RunSettingsFilePath`, which takes care of it for us. If we now run `dotnet test` we get proper output. For example, you can add a `Directory.Build.props` to the root of your project.

```xml
<Project>
  <PropertyGroup>
    <RunSettingsFilePath>$(MSBuildThisFileDirectory)runsettings.xml</RunSettingsFilePath>
  </PropertyGroup>
</Project>
```

## Closing Thoughts
I know I am not the first to write about this topic, but I hope to provide fresh insight into the subject matter. The different techniques all have their merit. I have used all three on other occasions and remind you that the NullLogger is a viable option in many cases. Nine times out of 10, you probably only care about the business logic to test. For the final remaining time, I can only say the well-known programming wisdom: "It depends".

As always, if you have any questions, feel free to reach out. Do you have suggestions or alternatives? I would love to hear about them.

The corresponding source code for this article is on [GitHub](https://github.com/kaylumah/CaptureLogsInUnitTests).

See you next time, stay healthy and happy coding to all 🧸!