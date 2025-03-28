﻿// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO;
using System.Text;
using FluentAssertions;
using Kaylumah.Ssg.Utilities;
using Xunit;

namespace Test.Unit.FormerXunit
{
    public class EncodingTests
    {

        [Theory]
        [MemberData(nameof(EncodingTestData))]
        public void Test_EncodingUtil_DetermineEncoding_ShouldReturnCorrectEncoding(Stream stream, string expectedEncoding)
        {
            Encoding result = stream.DetermineEncoding();
            result.Should().NotBeNull();
            string encoding = result.EncodingName;
            encoding.Should().Be(expectedEncoding);
        }

        public static IEnumerable<object[]> EncodingTestData()
        {
            byte[] ascii = Encoding.ASCII.GetBytes("Hello World!");
            yield return new object[] {
                new MemoryStream(ascii),
                Encoding.UTF8.EncodingName
            };

            byte[] utf8 = Encoding.UTF8.GetBytes("Hello World!");
            yield return new object[] {
                new MemoryStream(utf8),
                Encoding.UTF8.EncodingName
            };
        }
    }
}
