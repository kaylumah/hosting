// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using Test.Specflow.Entities;

namespace Test.Specflow;

[Binding]
public class TableTransforms
{
    [StepArgumentTransformation]
    public static AuthorCollection ToAuthors(Table table)
    {
        ArgumentNullException.ThrowIfNull(table);
        table.ValidateIfMappedCorrectlyTo<Author>();
        var authorCollection = new AuthorCollection();
        authorCollection.AddRange(table.CreateSet<Author>());
        return authorCollection;
    }
    
    [StepArgumentTransformation]
    public static OrganizationCollection ToOrganizations(Table table)
    {
        ArgumentNullException.ThrowIfNull(table);
        table.ValidateIfMappedCorrectlyTo<Organization>();
        var organizationCollection = new OrganizationCollection();
        organizationCollection.AddRange(table.CreateSet<Organization>());
        return organizationCollection;
    }
}
