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