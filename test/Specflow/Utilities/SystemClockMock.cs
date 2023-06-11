// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using Kaylumah.Ssg.Utilities.Time;

namespace Test.Specflow.Utilities;

public class SystemClockMock : StrictMock<ISystemClock>
{
    public void SetupSystemTime(DateTimeOffset systemTime)
    {
        Setup(x => x.LocalNow).Returns(systemTime);
    }
}
