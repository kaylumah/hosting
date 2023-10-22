// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.IO.Abstractions.TestingHelpers;
using Ssg.Extensions.Metadata.Abstractions;
using TechTalk.SpecFlow;
using Test.Specflow.Entities;

namespace Test.Specflow.Steps.Collections
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
