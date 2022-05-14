// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Test.Specflow;

[Binding]
internal class FeatureSteps
{
    [Given("scope '(.*)' has the following metadata:")]
    public void GivenTheFollowingData(string scope, Table table)
    {
        var metaData = table.ToDictionary();
    }
}
