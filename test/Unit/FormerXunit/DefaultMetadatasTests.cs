// Copyright (c) Kaylumah, 2025. All rights reserved.
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
            DefaultMetadata itemWithoutScope = new DefaultMetadata();
            itemWithoutScope.Path = "";
            itemWithoutScope.Extensions = [".html"];
            DefaultMetadata itemWithScope = new DefaultMetadata();
            itemWithScope.Path = "";
            itemWithScope.Scope = "";
            itemWithScope.Extensions = [".html"];
            DefaultMetadata itemWithNamedScope = new DefaultMetadata();
            itemWithNamedScope.Path = "";
            itemWithNamedScope.Scope = "posts";
            itemWithNamedScope.Extensions = [".html"];
            DefaultMetadata itemPathWithNameScope = new DefaultMetadata();
            itemPathWithNameScope.Path = "2019";
            itemPathWithNameScope.Scope = "posts";
            itemPathWithNameScope.Extensions = [".html"];

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
