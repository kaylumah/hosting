// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Ssg.Extensions.Metadata.Abstractions;
using VerifyXunit;
using Xunit;

namespace Test.Unit
{
    public class SnapshotTests
    {
        [Fact]
        public async Task Test1()
        {
            BuildData buildData = (BuildData) RuntimeHelpers.GetUninitializedObject(typeof(BuildData));
            
            SiteMetaData siteMetaData = new SiteMetaData("", "", "", "", "", "", new(), buildData, new());
            await Verifier.Verify(siteMetaData);
        }
    }
}