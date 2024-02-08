// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.IO.Abstractions.TestingHelpers;
using Ssg.Extensions.Metadata.Abstractions;
using Reqnroll;
using Test.Unit.Entities;

namespace Test.Unit.Steps.Collections
{
    [Binding]
    public class TagCollectionStepDefinitions
    {
        readonly TagCollection _TagCollection;
        readonly MockFileSystem _FileSystem;

        public TagCollectionStepDefinitions(MockFileSystem fileSystem, TagCollection tagCollection)
        {
            _FileSystem = fileSystem;
            _TagCollection = tagCollection;
        }

        [Given("the following tags:")]
        public void GivenTheFollowingTags(TagCollection tagCollection)
        {
            _TagCollection.AddRange(tagCollection);
            TagMetaDataCollection tagMetaDataCollection = new TagMetaDataCollection();
            tagMetaDataCollection.AddRange(_TagCollection.ToTagMetadata());
            _FileSystem.AddYamlDataFile(Constants.Files.Tags, tagMetaDataCollection);
        }
    }
}
