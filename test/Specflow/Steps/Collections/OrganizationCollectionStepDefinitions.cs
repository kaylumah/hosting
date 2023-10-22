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
    public class OrganizationCollectionStepDefinitions
    {
        readonly OrganizationCollection _OrganizationCollection;
        readonly MockFileSystem _FileSystem;

        public OrganizationCollectionStepDefinitions(MockFileSystem fileSystem, OrganizationCollection organizationCollection)
        {
            _FileSystem = fileSystem;
            _OrganizationCollection = organizationCollection;
        }

        [Given("the following organizations:")]
        public void GivenTheFollowingOrganizations(OrganizationCollection organizationCollection)
        {
            _OrganizationCollection.AddRange(organizationCollection);
            OrganizationMetaDataCollection organizationMetaDataCollection = new OrganizationMetaDataCollection();
            organizationMetaDataCollection.AddRange(_OrganizationCollection.ToOrganizationMetadata());
            _FileSystem.AddYamlDataFile(Constants.Files.Organizations, organizationMetaDataCollection);
        }
    }
}
