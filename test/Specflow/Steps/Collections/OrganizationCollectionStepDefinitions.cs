// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.IO.Abstractions.TestingHelpers;
using Kaylumah.Ssg.Engine.Transformation.Interface;
using Ssg.Extensions.Metadata.Abstractions;
using TechTalk.SpecFlow;
using Test.Specflow.Entities;

namespace Test.Specflow.Steps.Collections;

[Binding]
public class OrganizationCollectionStepDefinitions
{
    private readonly OrganizationCollection _organizationCollection;
    private readonly MockFileSystem _fileSystem;

    public OrganizationCollectionStepDefinitions(MockFileSystem fileSystem, OrganizationCollection organizationCollection)
    {
        _fileSystem = fileSystem;
        _organizationCollection = organizationCollection;
    }

    [Given("the following organizations:")]
    public void GivenTheFollowingOrganizations(OrganizationCollection organizationCollection)
    {
        _organizationCollection.AddRange(organizationCollection);
        var organizationMetaDataCollection = new OrganizationMetaDataCollection();
        organizationMetaDataCollection.AddRange(_organizationCollection.ToOrganizationMetadata());
        _fileSystem.AddYamlDataFile(Constants.Files.Organizations, organizationMetaDataCollection);
    }
}
