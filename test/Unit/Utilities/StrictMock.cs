// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using Moq;

namespace Test.Unit.Utilities
{
    public class StrictMock<T> : Mock<T> where T : class
    {
        protected StrictMock() : base(MockBehavior.Strict) { }
    }
}
