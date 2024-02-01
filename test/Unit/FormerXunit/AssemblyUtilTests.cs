// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Reflection;
using FluentAssertions;
using Kaylumah.Ssg.Utilities;
using Xunit;

namespace Test.Unit.FormerXunit
{
    public class AssemblyUtilTests
    {
        [Fact(Skip = "Unstable")]
        public void Test_AssemblyData()
        {
            AssemblyInfo result = Assembly.GetExecutingAssembly().RetrieveAssemblyInfo();
            result.Should().NotBeNull();
            result.Copyright.Should().NotBeNull();
            result.Version.Should().NotBeNull();
            result.Metadata.Count.Should().

            Be(8);
        }
    }
}
