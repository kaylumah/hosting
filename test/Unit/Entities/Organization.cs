﻿// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using Test.Unit.BDD;
using Test.Unit.Helpers;

namespace Test.Unit.Entities
{
    public class Organization
    {
        [GherkinTableHeader(0)]
        public string? Id
        { get; set; }

        [GherkinTableHeader(1)]
        public string? Name
        { get; set; }
    }

    public class OrganizationCollection : MockEntityCollection<string, Organization>
    {
        public override string BuildKey(Organization item)
        {
            return item.Id ?? string.Empty;
        }
    }
}
