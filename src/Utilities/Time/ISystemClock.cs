// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Kaylumah.Ssg.Utilities.Time;

public interface ISystemClock
{
    DateTimeOffset UtcNow { get; }
    // TimeZoneInfo LocalZone { get; }
    // DateTimeOffset LocalNow => TimeZoneInfo.ConvertTime(UtcNow, LocalZone);
}

public class SystemClock : ISystemClock
{
    // https://github.com/dotnet/aspnetcore/issues/16844
    // https://github.com/dotnet/runtime/issues/36617
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
