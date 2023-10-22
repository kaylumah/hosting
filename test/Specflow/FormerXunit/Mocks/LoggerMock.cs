// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.Logging;
using Moq;

namespace Test.Specflow.FormerXunit.Mocks
{
    public class LoggerMock<T> : Mock<ILogger<T>>
    {
        public LoggerMock()
        {
            // https://ardalis.com/testing-logging-in-aspnet-core/
            // https://stackoverflow.com/questions/39604198/how-to-test-asp-net-core-built-in-ilogger
            Setup(x =>
                    x.Log(
                        LogLevel.Information,
                        It.IsAny<EventId>(),
                        It.IsAny<It.IsAnyType>(),
                        It.IsAny<Exception>(),
                        (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()
                    )
                )
                .Callback<LogLevel, EventId, object, Exception, Delegate>(
                    (level, eventid, state, ex, func) =>
                    {
                        string result = state.ToString();
                        //this.Out.WriteLine(state.ToString());
                    }
                );
            // Setup(logger => logger.Log(It.IsAny<LogLevel>(), It.IsAny<string>()))
            //     .Callback((LogLevel level, string message) => {

            //     });
            // Setup(logger => logger.LogInformation(It.IsAny<string>()))
            //     .Callback<string>(message => {});

            // Setup(x => x.Log(
            //     It.IsAny<LogLevel>(),
            //     It.IsAny<EventId>(),
            //     It.IsAny<It.IsAnyType>(),
            //     It.IsAny<Exception>(),
            //     (Func<It.IsAnyType, Exception, string>)It.IsAny<object>())
            // )
            // .Callback((LogLevel level,
            //     EventId eventId,
            //     Object object,
            //     Exception exception,
            //     string message) => {

            // });
            // .Callback(new InvocationAction(invocation => {

            // }));

        }
    }
}
