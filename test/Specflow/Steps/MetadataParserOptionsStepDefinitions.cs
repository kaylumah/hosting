// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Test.Specflow.Steps;

[Binding]
public class MetadataParserOptionsStepDefinitions
{
    private readonly MetadataParserOptions _metadataParserOptions;

    public MetadataParserOptionsStepDefinitions(MetadataParserOptions metadataParserOptions)
    {
        _metadataParserOptions = metadataParserOptions;
    }

    [Given("the following defaults:")]
    public void GivenTheFollowingDefaults(DefaultMetadatas metadatas)
    {
        _metadataParserOptions.Defaults = metadatas;
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
        _metadataParserOptions.ExtensionMapping = dictionary;
    }
}
