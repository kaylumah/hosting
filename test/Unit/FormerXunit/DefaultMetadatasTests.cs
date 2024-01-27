// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

// using FluentAssertions;
// using Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;
// using Xunit;

// namespace Test.Unit.FormerXunit
// {
//     public class DefaultMetadatasTests
//     {
//         [Fact]
//         public void TestKey()
//         {
//             DefaultMetadata itemWithoutScope = new DefaultMetadata() { Path = "" };
//             DefaultMetadata itemWithScope = new DefaultMetadata() { Path = "", Scope = "" };
//             DefaultMetadata itemWithNamedScope = new DefaultMetadata() { Path = "", Scope = "posts" };
//             DefaultMetadata itemPathWithNameScope = new DefaultMetadata() { Path = "2019", Scope = "posts" };

//             DefaultMetadatas data = new DefaultMetadatas
//             {
//                 itemWithoutScope,
//                 itemWithScope,
//                 itemWithNamedScope,
//                 itemPathWithNameScope
//             };

//             data[""].Should().NotBeNull();
//             data["."].Should().NotBeNull();
//             data[".posts"].Should().NotBeNull();
//             data["2019.posts"].Should().NotBeNull();
//         }
//     }
// }
