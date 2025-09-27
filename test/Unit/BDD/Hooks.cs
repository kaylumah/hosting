// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using Reqnroll;
using Reqnroll.Assist;
using Reqnroll.Assist.ValueRetrievers;

namespace Test.Unit.BDD
{
    [Binding]
    public class Hooks
    {
        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            // https://docs.specflow.org/projects/specflow/en/latest/Bindings/SpecFlow-Assist-Helpers.html
            NullValueRetriever nullValueRetriever = new NullValueRetriever(Constants.NullIndicator);
            Service.Instance.ValueRetrievers.Register(nullValueRetriever);
        }
    }
}
