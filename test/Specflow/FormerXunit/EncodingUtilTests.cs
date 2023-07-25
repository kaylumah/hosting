// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO;
using System.Text;
using FluentAssertions;
using Kaylumah.Ssg.Utilities;
using Xunit;

namespace Test.Unit;

public class EncodingUtilTests
{

    [Theory]
#pragma warning disable CS3016 // Arrays as attribute arguments is not CLS-compliant
    [MemberData(nameof(EncodingTestData))]
#pragma warning restore CS3016 // Arrays as attribute arguments is not CLS-compliant
    public void Test_EncodingUtil_DetermineEncoding_ShouldReturnCorrectEncoding(Stream stream, string expectedEncoding)
    {
        var result = stream.DetermineEncoding();
        result.Should().NotBeNull();
        var encoding = result.EncodingName;
        encoding.Should().Be(expectedEncoding);
    }

    public static IEnumerable<object[]> EncodingTestData()
    {
        yield return new object[] {
                new MemoryStream(Encoding.ASCII.GetBytes("Hello World!")),
                Encoding.UTF8.EncodingName
            };

        yield return new object[] {
                new MemoryStream(Encoding.UTF8.GetBytes("Hello World!")),
                Encoding.UTF8.EncodingName
            };
    }
}
