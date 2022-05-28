// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using Test.Specflow.Entities;
using Test.Specflow.Entities.Extensions;

namespace Test.Specflow
{
    [Binding]
    public class MetadataParserOptionsSteps
    {
        private readonly MetadataParserOptions _metadataParserOptions;

        public MetadataParserOptionsSteps(MetadataParserOptions metadataParserOptions)
        {
            _metadataParserOptions = metadataParserOptions;
        }

        [Given("the following default metadata:")]
        public void GivenTheFollowingDefaultMetadata(Table table)
        {
            var set = table.CreateSet<MetaItem>();
            _metadataParserOptions.Defaults = set.ToDefaultMetadatas();
        }

        [Given("the following extension mapping:")]
        public void GivenTheFollowingExtensionMapping(Table table)
        {
            var set = table.CreateSet<(string key, string value)>();
            var dictionary = new Dictionary<string, string>();
            foreach(var (key, value) in set)
            {
                dictionary.Add(key, value);
            }
            _metadataParserOptions.ExtensionMapping = dictionary;
        }
    }
}
