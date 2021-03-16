// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
using FluentAssertions;
using Kaylumah.Ssg.Utilities.Web;
using Xunit;

namespace Test.Unit
{
    public class WebUtilTests
    {
        [Fact]
        public void Test1()
        {
            var sut = new Class1();
            var input = "using-c#-in-your-git-hooks.html";
            
            var encodedInput = sut.Encode(input);
            var decoded = sut.Decode(encodedInput);
            decoded.Should().Be(input);
        }
    }
}