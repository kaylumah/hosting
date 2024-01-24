// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.IO.Abstractions.TestingHelpers;
using Ssg.Extensions.Metadata.Abstractions;
using TechTalk.SpecFlow;
using Test.Unit.Entities;

namespace Test.Unit.Steps.Collections
{
    [Binding]
    public class AuthorCollectionStepDefinitions
    {
        readonly AuthorCollection _AuthorCollection;
        readonly MockFileSystem _FileSystem;

        public AuthorCollectionStepDefinitions(MockFileSystem fileSystem, AuthorCollection authorCollection)
        {
            _FileSystem = fileSystem;
            _AuthorCollection = authorCollection;
        }

        [Given("the following authors:")]
        public void GivenTheFollowingAuthors(AuthorCollection authorCollection)
        {
            _AuthorCollection.AddRange(authorCollection);
            AuthorMetaDataCollection authorMetaDataCollection = new AuthorMetaDataCollection();
            authorMetaDataCollection.AddRange(_AuthorCollection.ToAuthorMetadata());
            _FileSystem.AddYamlDataFile(Constants.Files.Authors, authorMetaDataCollection);
        }
    }
}
