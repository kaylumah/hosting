// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using FluentAssertions;
using TechTalk.SpecFlow;

namespace Test.Specflow.Steps;

[Binding]
public class ValidationSteps
{
    private readonly ValidationContext _validationContext;

    public ValidationSteps(ValidationContext validationContext)
    {
        _validationContext = validationContext;
    }

    [Then("the scenario executed successfully:")]
    public void ThenTheScenarioExecutedSuccessfully()
    {
        _validationContext.TestServiceException.Should().BeNull();
    }
}
