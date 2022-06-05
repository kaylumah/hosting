﻿// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Kaylumah.Ssg.Utilities.Time;

public interface ISystemClock
{
    DateTimeOffset LocalNow { get; }
    DateTimeOffset UtcNow { get; }
    long UtcNowTicks { get; }
}

public class SystemClock : ISystemClock
{
    // https://github.com/dotnet/aspnetcore/blob/a450cb69b5e4549f5515cdb057a68771f56cefd7/src/Servers/Kestrel/Core/src/Internal/Infrastructure/SystemClock.cs#L11
    // https://github.com/dotnet/aspnetcore/blob/a450cb69b5e4549f5515cdb057a68771f56cefd7/src/Servers/Kestrel/shared/test/MockSystemClock.cs#L10
    // https://github.com/dotnet/aspnetcore/blob/a450cb69b5e4549f5515cdb057a68771f56cefd7/src/Servers/Kestrel/Core/src/Internal/Infrastructure/ISystemClock.cs#L11
    // https://github.com/dotnet/aspnetcore/issues/16844
    // https://github.com/dotnet/runtime/issues/36617

    private readonly TimeZoneInfo _timeZone;

    public SystemClock()
    {
        _timeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Amsterdam");
    }

    public DateTimeOffset LocalNow => TimeZoneInfo.ConvertTime(UtcNow, _timeZone);
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    public long UtcNowTicks => UtcNow.Ticks;
}
