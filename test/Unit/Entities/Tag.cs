// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using Test.Unit.Helpers;

namespace Test.Unit.Entities
{
    public class Tag
    {
        [GherkinTableHeader(0)]
        public string Id { get; set; }
    }

    public class TagCollection : MockEntityCollection<string, Tag>
    {
        public override string BuildKey(Tag item)
        {
            return item.Id;
        }
    }
}
