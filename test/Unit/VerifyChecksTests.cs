// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace Test.Unit
{
    public class VerifyChecksTests
    {
        [Fact(Skip = "Broken")]
        public Task Run() =>
            VerifyChecks.Run();
    }
}