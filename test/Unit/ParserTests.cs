// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Ssg.Extensions.Data.Abstractions;
using Kaylumah.Ssg.Extensions.Data.Csv;
using Xunit;

namespace Test.Unit
{
    public class ParserTests
    {
        [Fact]
        public void Test1()
        {
            ICollectionParser csvParser = new CsvParser();
        }
    }
}