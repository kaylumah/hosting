// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
using Microsoft.Extensions.Logging;
using Moq;

namespace Test.Unit.Mocks
{
    public class LoggerMock<T> : Mock<ILogger<T>>
    {

    }
}