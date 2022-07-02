// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using Test.Specflow.Entities;

namespace Test.Specflow;
#pragma warning disable CS3001

[Binding]
public class UnusedSteps
{
    [Given("the following blog posts:")]
    [Given("the following blog articles:")]
    public void Given(Table table)
    {
        var posts = table.CreateSet<BlogPost>();
    }
    
    [Then("'(.*)' are valid")]
    public void Then(List<string> values)
    {
    }
}
#pragma warning restore CS3001
