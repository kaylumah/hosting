// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.IO.Abstractions.TestingHelpers;
using Ssg.Extensions.Metadata.Abstractions;
using TechTalk.SpecFlow;
using Test.Specflow.Entities;

namespace Test.Specflow.Steps.Collections;

[Binding]
public class AuthorCollectionStepDefinitions
{
    private readonly AuthorCollection _authorCollection;
    private readonly MockFileSystem _fileSystem;

    public AuthorCollectionStepDefinitions(MockFileSystem fileSystem, AuthorCollection authorCollection)
    {
        _fileSystem = fileSystem;
        _authorCollection = authorCollection;
    }

    [Given("the following authors:")]
    public void GivenTheFollowingAuthors(AuthorCollection authorCollection)
    {
        _authorCollection.AddRange(authorCollection);
        var authorMetaDataCollection = new AuthorMetaDataCollection();
        authorMetaDataCollection.AddRange(_authorCollection.ToAuthorMetadata());
        _fileSystem.AddYamlDataFile(Constants.Files.Authors, authorMetaDataCollection);
    }
}
