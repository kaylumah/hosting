// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Test.Specflow.Entities;

public class Organization
{
    public string Id { get; set; }
}

public class OrganizationCollection : MockEntityCollection<string, Organization>
{
    public override string BuildKey(Organization item)
    {
        return item.Id;
    }
}
