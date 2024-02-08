// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using Reqnroll;
using Reqnroll.Assist;
using Reqnroll.Assist.ValueRetrievers;

namespace Test.Unit
{
    [Binding]
    public class Hooks
    {
        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            // https://docs.specflow.org/projects/specflow/en/latest/Bindings/SpecFlow-Assist-Helpers.html
            Service.Instance.ValueRetrievers.Register(new NullValueRetriever(Constants.NullIndicator));
        }
    }
}
