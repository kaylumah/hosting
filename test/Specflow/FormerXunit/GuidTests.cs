// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Ssg.Utilities;
using Xunit;

namespace Test.Unit.FormerXunit
{
    public class GuidTests
    {
        // Original source: https://github.com/Faithlife/FaithlifeUtility/blob/master/tests/Faithlife.Utility.Tests/GuidUtilityTests.cs

        [Fact]
        public void Test1()
        {
            // Guid nsDNS = new Guid("6ba7b810-9dad-11d1-80b4-00c04fd430c8")
            // Guid nsRoot = Guid.Create(nsDNS, "myapp.example.com", 5)
            // Guid nsFoo = Guid.Create(nsRoot, "Foo", 5)
            System.Guid nsRoot1 = GuidUtility.Create(GuidUtility.DnsNamespace, "kaylumah.nl", 5);
            System.Guid nsRoot2 = GuidUtility.Create(GuidUtility.DnsNamespace, "app.kaylumah.nl", 5);
            System.Guid nsRoot3 = GuidUtility.Create(GuidUtility.DnsNamespace, "kaylumah.nl/other", 5);

            string title = "2020/08/01/kaylumah-the-new-home-for-blogs-written-by-max-hamulyak.html";
            System.Guid id1a = GuidUtility.Create(nsRoot1, title, 5);
            System.Guid id1b = GuidUtility.Create(nsRoot1, title, 5);
            System.Guid id2 = GuidUtility.Create(nsRoot2, title, 5);
            System.Guid id3 = GuidUtility.Create(nsRoot3, title, 5);
        }
    }
}
