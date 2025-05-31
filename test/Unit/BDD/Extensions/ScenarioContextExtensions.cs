﻿// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using Reqnroll;
using Reqnroll.Tracing;

namespace Test.Unit.BDD.Extensions
{
    public static class ScenarioContextExtensions
    {
        public static string ToVerifyMethodName(this ScenarioContext scenarioContext, string artifact)
        {
            ScenarioInfo info = scenarioContext.ScenarioInfo;
            string testName = info.Title.ToIdentifier();
            return $"{testName}-{artifact}";
        }
    }
}
