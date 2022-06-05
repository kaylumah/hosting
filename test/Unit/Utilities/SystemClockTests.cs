// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Test.Unit;

using System;
using Kaylumah.Ssg.Utilities.Time;
using Xunit;

public class SystemClockTests
{
    [Fact]
    public void Test1()
    {
        ISystemClock clock = new SystemClock();

        // TimeZone Stuff
        var localZone = TimeZoneInfo.Local;
        var namedZone1 = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");
        var namedZone2 = TimeZoneInfo.FindSystemTimeZoneById("Europe/Amsterdam");

        var now = clock.UtcNow;
        var converted = TimeZoneInfo.ConvertTime(now, localZone);
    }
}
