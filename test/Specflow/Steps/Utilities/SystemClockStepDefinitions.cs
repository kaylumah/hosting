
// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using TechTalk.SpecFlow;
using Test.Specflow.Utilities;

namespace Test.Specflow.Steps.Utilities
{
    [Binding]
    public class SystemClockStepDefinitions
    {
        readonly SystemClockMock _SystemClockMock;

        public SystemClockStepDefinitions(SystemClockMock systemClockMock)
        {
            _SystemClockMock = systemClockMock;
        }

        [Given(@"the current date is '(.*)':")]
        public void GivenTheCurrentDateIs(DateTimeOffset systemDateTime)
        {
            _SystemClockMock.SetupSystemTime(systemDateTime);
        }
    }
}
