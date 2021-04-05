# Post Ideas

## Aspect Oriented Programming

- https://maartenderaedemaeker.be/2017/07/23/using-type-interceptors-with-autofac/
- https://docs.microsoft.com/en-us/archive/msdn-magazine/2014/february/aspect-oriented-programming-aspect-oriented-programming-with-the-realproxy-class

## Assembly Metadata (net style / .NET5 SDK

https://dev.to/stuartblang/adding-assemblymetadataattribute-using-new-sdk-project-with-msbuild-2akl

## Decorator in DI

https://andrewlock.net/using-scrutor-to-automatically-register-your-services-with-the-asp-net-core-di-container/

https://andrewlock.net/adding-decorated-classes-to-the-asp.net-core-di-container-using-scrutor/

## DI Console

https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection-usage

https://andrewlock.net/using-dependency-injection-in-a-net-core-console-application/

https://pradeeploganathan.com/dotnet/configuration-in-a-net-core-console-application/

https://blog.hildenco.com/2020/05/configuration-in-net-core-console.html

https://docs.microsoft.com/en-us/dotnet/core/extensions/configuration

https://garywoodfine.com/configuration-api-net-core-console-application/

https://keestalkstech.com/2018/04/dependency-injection-with-ioptions-in-console-apps-in-net-core-2/

https://referbruv.com/blog/posts/dependency-injection-in-a-net-core-console-application

## IOptions

https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-5.0

https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.options.iconfigureoptions-1?view=dotnet-plat-ext-5.0

https://stevetalkscode.co.uk/using-iconfigureoptions

https://andrewlock.net/simplifying-dependency-injection-for-iconfigureoptions-with-the-configureoptions-helper/

## Mocks

https://dev.azure.com/graphene-apps/IDesign%20Method/_git/VisioEx?path=%2Fsrc%2FTest%2FUnit%2FSpecFlow%2FMocks%2FShapesAccessMock.cs&version=GB133-proper-callout-support&_a=contents

        [Fact]
        public async Task TestLoggerExtensions()
        {
            var loggerMock = new Mock<ILogger<SiteManager>>();
            loggerMock.Setup(mock => 
                mock.IsEnabled(It.IsAny<LogLevel>())
            ).Returns(true);

            ISiteManager sut = new SiteManager(loggerMock.Object);
            await sut.GenerateSite(null);

            loggerMock.Verify(mock => mock.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Directory not found")),
                It.IsAny<Exception>(), It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);


            // loggerMock.Verify(
            //     m => m.Log(LogLevel.Information,
            //     It.IsAny<EventId>(),
            //     It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("_data")),
            //     null,
            //     It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            //     Times.Once
            // );
        }


        namespace Microsoft.Extensions.Logging
{
    public static class LoggerExtensions
    {
        private static readonly Action<ILogger, string, Exception> _directoryNotFound;

        static LoggerExtensions()
        {
            _directoryNotFound = LoggerMessage.Define<string>(
                LogLevel.Information,
                new EventId(1, nameof(DirectoryNotFound)),
                "Directory not found (Directory = '{Directory}')"
            );
        }

        public static void DirectoryNotFound(this ILogger logger, string directory)
        {
            _directoryNotFound(logger, directory, null);
        }
    }
}

## Testing ILogger

- https://stackoverflow.com/questions/63957361/how-to-mock-a-fileprovider-in-c
https://www.javaer101.com/en/article/6947747.html

https://github.com/martincostello/xunit-logging

https://adamstorr.azurewebsites.net/blog/mocking-ilogger-with-moq

https://codeburst.io/unit-testing-with-net-core-ilogger-t-e8c16c503a80

https://www.meziantou.net/how-to-get-asp-net-core-logs-in-the-output-of-xunit-tests.htm

https://chrissainty.com/unit-testing-ilogger-in-aspnet-core/

https://anthonygiretti.com/2020/02/05/tips-tricks-for-unit-testing-in-net-core-3-checking-matching-and-non-matching-arguments-on-ilogger/

https://alessio.franceschelli.me/posts/dotnet/how-to-test-logging-when-using-microsoft-extensions-logging/

https://www.thecodebuzz.com/unit-test-and-mock-logger-methods-in-net-core/

https://whuysentruit.medium.com/unit-testing-ilogger-in-asp-net-core-9a2d066d0fb8

https://christianfindlay.com/2020/07/03/ilogger/

https://stackoverflow.com/questions/43424095/how-to-unit-test-with-ilogger-in-asp-net-core

https://gist.github.com/cknaap/6919df54820853f7a3ef054303cebfc3

https://alastaircrabtree.com/using-logging-in-unit-tests-in-net-core/

---

https://dev.to/sparky/sparkytesthelpers-moq-307h

https://dev.to/chaitanyasuvarna/writing-unit-tests-for-httpclient-using-nunit-and-moq-in-c-37jh

https://dev.to/kritner/getting-started-with-unit-testing-and-moq---part-2-406

https://dev.to/rajmondburgaj/how-to-setup-methods-which-return-a-built-int-type-and-implement-ienumerable-interface-in-moq--20of

https://dev.to/dotnet/how-you-can-learn-mock-testing-in-net-core-and-c-with-moq-4ikd

https://dev.to/canro91/how-to-create-fakes-with-moq-and-what-i-don-t-like-about-it-45m4

https://dev.to/matheusrodrigues/mock-multiple-calls-to-the-same-method-with-fakeiteasy-moq-and-nsubstitute-45lp

https://dev.to/seankilleen/quick-tip-when-testing-with-moq-try-lambdas-for-more-flexible-tests-1k9o



TODO https://adamstorr.azurewebsites.net/tags/C%23