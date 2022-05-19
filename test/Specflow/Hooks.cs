// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace Test.Specflow
{
    [Binding]
    internal static class Hooks
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
        public List<string> TransformToListOfString(string commaSeparatedList)
        {
            return commaSeparatedList.Split(",").ToList();
        }
    }

    public class Page
    {
        public string Uri { get; set; }
    }
}
