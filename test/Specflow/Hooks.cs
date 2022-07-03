// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace Test.Specflow;

[Binding]
public class Hooks
{
    [BeforeTestRun]
    public static void BeforeTestRun()
    {
        // https://docs.specflow.org/projects/specflow/en/latest/Bindings/SpecFlow-Assist-Helpers.html
        Service.Instance.ValueRetrievers.Register(new NullValueRetriever("<null>"));
    }

    [StepArgumentTransformation]
    public List<String> TransformToListOfString(string commaSeparatedList)
    {
        return commaSeparatedList.Split(",").ToList();
    }
}
