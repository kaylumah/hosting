// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using FluentAssertions;
using Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;
using Xunit;

namespace Test.Unit.FormerXunit
{
    public class DefaultMetadatasTests
    {
        [Fact]
        public void TestKey()
        {
            DefaultMetadata itemWithoutScope = new DefaultMetadata() { Path = "", Extensions = [".html"] };
            DefaultMetadata itemWithScope = new DefaultMetadata() { Path = "", Scope = "", Extensions = [".html"] };
            DefaultMetadata itemWithNamedScope = new DefaultMetadata() { Path = "", Scope = "posts", Extensions = [".html"] };
            DefaultMetadata itemPathWithNameScope = new DefaultMetadata() { Path = "2019", Scope = "posts", Extensions = [".html"] };

            DefaultMetadatas data = new DefaultMetadatas
            {
                itemWithoutScope,
                itemWithScope,
                itemWithNamedScope,
                itemPathWithNameScope
            };

            data[".html"].Should().NotBeNull();
            data["..html"].Should().NotBeNull();
            data[".posts.html"].Should().NotBeNull();
            data["2019.posts.html"].Should().NotBeNull();
        }
    }
}
