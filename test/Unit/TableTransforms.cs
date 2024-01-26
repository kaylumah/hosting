// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using Test.Unit.Entities;

namespace Test.Unit
{
    [Binding]
    public class TableTransforms
    {
        [StepArgumentTransformation]
        public static AuthorCollection ToAuthors(Table table)
        {
            ArgumentNullException.ThrowIfNull(table);
            table.ValidateIfMappedCorrectlyTo<Author>();
            AuthorCollection authorCollection = new AuthorCollection();
            authorCollection.AddRange(table.CreateSet<Author>());
            return authorCollection;
        }

        [StepArgumentTransformation]
        public static OrganizationCollection ToOrganizations(Table table)
        {
            ArgumentNullException.ThrowIfNull(table);
            table.ValidateIfMappedCorrectlyTo<Organization>();
            OrganizationCollection organizationCollection = new OrganizationCollection();
            organizationCollection.AddRange(table.CreateSet<Organization>());
            return organizationCollection;
        }

        [StepArgumentTransformation]
        public static TagCollection ToTags(Table table)
        {
            ArgumentNullException.ThrowIfNull(table);
            table.ValidateIfMappedCorrectlyTo<Tag>();
            TagCollection tagCollection = new TagCollection();
            tagCollection.AddRange(table.CreateSet<Tag>());
            return tagCollection;
        }
    }
}
