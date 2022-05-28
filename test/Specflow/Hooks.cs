// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace Test.Specflow
{

    [Binding]
    internal class Hooks
    {
        
        [BeforeTestRun]
        internal static void BeforeTestRun()
        {
            // https://docs.specflow.org/projects/specflow/en/latest/Bindings/SpecFlow-Assist-Helpers.html
           Service
                .Instance
                .ValueRetrievers
                .Register(new NullValueRetriever("<null>"));
           
        }

        [StepArgumentTransformation]
        public static List<string> TransformToListOfString(string commaSeparatedList)
        {
            return commaSeparatedList.Split(",").ToList();
        }

        [StepArgumentTransformation]
        public static List<Page> TransformPages(Table table)
        {
            var pages = table.CreateSet<Page>();
            return pages.ToList();
        }
    }
}
