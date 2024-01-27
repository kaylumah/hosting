// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.Time.Testing;
using TechTalk.SpecFlow;

namespace Test.Unit.Steps.Utilities
{
    [Binding]
    public class FakeTimeProviderStepDefinitions
    {
        readonly FakeTimeProvider _FakeTimeProvider;

        public FakeTimeProviderStepDefinitions(FakeTimeProvider fakeTimeProvider)
        {
            _FakeTimeProvider = fakeTimeProvider;
        }

        [Given(@"the current date is '(.*)':")]
        public void GivenTheCurrentDateIs(DateTimeOffset systemDateTime)
        {
            _FakeTimeProvider.SetUtcNow(systemDateTime);
        }
    }
}
