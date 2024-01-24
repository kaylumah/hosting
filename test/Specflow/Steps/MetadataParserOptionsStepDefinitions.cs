// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Test.Unit.Steps
{
    [Binding]
    public class MetadataParserOptionsStepDefinitions
    {
        readonly MetadataParserOptions _MetadataParserOptions;

        public MetadataParserOptionsStepDefinitions(MetadataParserOptions metadataParserOptions)
        {
            _MetadataParserOptions = metadataParserOptions;
        }

        [Given("the following defaults:")]
        public void GivenTheFollowingDefaults(DefaultMetadatas metadatas)
        {
            _MetadataParserOptions.Defaults = metadatas;
        }

        [Given("the following extension mapping:")]
        public void GivenTheFollowingExtensionMapping(Table table)
        {
            IEnumerable<(string key, string value)> set = table.CreateSet<(string key, string value)>();
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            foreach ((string key, string value) in set)
            {
                dictionary.Add(key, value);
            }

            _MetadataParserOptions.ExtensionMapping = dictionary;
        }
    }
}
