﻿// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using FluentAssertions;
using Reqnroll;

namespace Test.Unit.Steps
{
    [Binding]
    public class ValidationSteps
    {
        readonly ValidationContext _ValidationContext;

        public ValidationSteps(ValidationContext validationContext)
        {
            _ValidationContext = validationContext;
        }

        [Then("the scenario executed successfully:")]
        public void ThenTheScenarioExecutedSuccessfully()
        {
            _ValidationContext.TestServiceException.Should().BeNull();
        }
    }
}
