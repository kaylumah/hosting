// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Reflection;
using FluentAssertions;
using Kaylumah.Ssg.Utilities;
using Xunit;

namespace Test.Unit;

public class AssemblyUtilTests
{
    [Fact]
    public void Test_AssemblyData()
    {
        var result = Assembly.GetExecutingAssembly().RetrieveAssemblyInfo();
        result.Should().NotBeNull();
        result.Copyright.Should().NotBeNull();
        result.Version.Should().NotBeNull();
        result.Metadata.Count.Should().Be(6);
    }
}
