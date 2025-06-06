﻿// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.IO.Abstractions.TestingHelpers;
using Kaylumah.Ssg.Extensions.Metadata.Abstractions;
using Reqnroll;
using Test.Unit.Entities;

namespace Test.Unit.Steps.Collections
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
            System.Collections.Generic.IEnumerable<OrganizationMetaData> organizations = _OrganizationCollection.ToOrganizationMetadata();
            organizationMetaDataCollection.AddRange(organizations);
            _FileSystem.AddYamlDataFile(Kaylumah.Ssg.Manager.Site.Service.Constants.KnownFiles.Organizations, organizationMetaDataCollection);
        }
    }
}
