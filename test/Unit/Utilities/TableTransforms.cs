// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using Reqnroll;
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
            System.Collections.Generic.IEnumerable<Author> authors = table.CreateSet<Author>();
            authorCollection.AddRange(authors);
            return authorCollection;
        }

        [StepArgumentTransformation]
        public static OrganizationCollection ToOrganizations(Table table)
        {
            ArgumentNullException.ThrowIfNull(table);
            table.ValidateIfMappedCorrectlyTo<Organization>();
            OrganizationCollection organizationCollection = new OrganizationCollection();
            System.Collections.Generic.IEnumerable<Organization> organizations = table.CreateSet<Organization>();
            organizationCollection.AddRange(organizations);
            return organizationCollection;
        }

        [StepArgumentTransformation]
        public static TagCollection ToTags(Table table)
        {
            ArgumentNullException.ThrowIfNull(table);
            table.ValidateIfMappedCorrectlyTo<Tag>();
            TagCollection tagCollection = new TagCollection();
            System.Collections.Generic.IEnumerable<Tag> tags = table.CreateSet<Tag>();
            tagCollection.AddRange(tags);
            return tagCollection;
        }
    }
}
