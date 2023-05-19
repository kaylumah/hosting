// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using Test.Specflow.Utilities;

namespace Test.Specflow.Steps;

[Binding]
public class SystemClockStepDefinitions
{
    private readonly SystemClockMock _systemClockMock;

    public SystemClockStepDefinitions(SystemClockMock systemClockMock)
    {
        _systemClockMock = systemClockMock;
    }

    [Given(@"the current date is '(.*)':")]
    public void GivenTheCurrentDateIs(DateTimeOffset systemDateTime)
    {
        _systemClockMock.SetupSystemTime(systemDateTime);
    }
}
