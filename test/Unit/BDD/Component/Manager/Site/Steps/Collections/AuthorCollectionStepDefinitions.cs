// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.IO.Abstractions.TestingHelpers;
using Kaylumah.Ssg.Extensions.Metadata.Abstractions;
using Reqnroll;
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
            System.Collections.Generic.IEnumerable<AuthorMetaData> authors = _AuthorCollection.ToAuthorMetadata();
            authorMetaDataCollection.AddRange(authors);
            _FileSystem.AddYamlDataFile(Kaylumah.Ssg.Manager.Site.Service.Constants.KnownFiles.Authors, authorMetaDataCollection);
        }
    }
}
