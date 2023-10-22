// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Tracing;

namespace Test.Specflow.Extensions;

public static class ScenarioContextExtensions
{
    public static string ToVerifyMethodName(this ScenarioContext scenarioContext, string artifact)
    {
        ScenarioInfo info = scenarioContext.ScenarioInfo;
        string testName = info.Title.ToIdentifier();
        return $"{testName}-{artifact}";
    }
}
