﻿// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Test.Specflow
{
    [Binding]
    public class Mapping
    {
        [Given("the following metadata:")]
        public void Given(Table table)
        {
            var set = table.CreateSet<MetaItem>();

            var defaultMetaDatas = new DefaultMetadatas();

            var groupedByScope = set.GroupBy(x => x.scope);

            foreach(var scopeGroup in groupedByScope)
            {
                var scope = scopeGroup.Key;
                var groupedByPath = scopeGroup.GroupBy(x => x.path);

                foreach(var pathGroup in groupedByPath)
                {
                    var path = pathGroup.Key;

                    defaultMetaDatas.Add(new DefaultMetadata
                    {
                        Path = path,
                        Scope = scope,
                        Values = new FileMetaData()
                    });
                }
            }

        }
    }

    public readonly record struct MetaItem(string scope, string path);
}
