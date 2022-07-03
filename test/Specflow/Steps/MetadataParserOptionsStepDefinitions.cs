// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;

namespace Test.Specflow.Steps;

#pragma warning disable CS3001
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
        var set = table.CreateSet<(string key, string value)>();
        var dictionary = new Dictionary<string, string>();
        foreach (var (key, value) in set)
        {
            dictionary.Add(key, value);
        }
        _metadataParserOptions.ExtensionMapping = dictionary;
    }
}
