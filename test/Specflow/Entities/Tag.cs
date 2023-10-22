// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using Test.Specflow.Helpers;

namespace Test.Specflow.Entities
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
