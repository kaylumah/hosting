// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.IO.Abstractions.TestingHelpers;
using Kaylumah.Ssg.Engine.Transformation.Interface;
using TechTalk.SpecFlow;
using Test.Specflow.Entities;

namespace Test.Specflow.Steps.Collections;

[Binding]
public class TagCollectionStepDefinitions
{
    private readonly TagCollection _tagCollection;
    private readonly MockFileSystem _fileSystem;

    public TagCollectionStepDefinitions(MockFileSystem fileSystem, TagCollection tagCollection)
    {
        _fileSystem = fileSystem;
        _tagCollection = tagCollection;
    }

    [Given("the following tags:")]
    public void GivenTheFollowingTags(TagCollection tagCollection)
    {
        _tagCollection.AddRange(tagCollection);
        var tagMetaDataCollection = new TagMetaDataCollection();
        tagMetaDataCollection.AddRange(_tagCollection.ToTagMetadata());
        _fileSystem.AddYamlDataFile(Constants.Files.Tags, tagMetaDataCollection);
    }
}
