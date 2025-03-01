// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using Xunit.Abstractions;

namespace Test.Unit
{
    public abstract class StronglyTypedIntIdTests<TStrongTypedId> : StronglyTypedIdTests<TStrongTypedId, int> where TStrongTypedId : struct
    {
        protected StronglyTypedIntIdTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        protected override int SampleValue => 3;

        protected override int EmptyValue => default;
    }
    
    public readonly record struct TestIntId(int Value)
    {
        public static implicit operator int(TestIntId intId) => intId.Value;
        public static implicit operator TestIntId(int value) => new(value);
    }
    
    public class TestIntIdTests : StronglyTypedIntIdTests<TestIntId>
    {
        public TestIntIdTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        protected override TestIntId ConvertFromPrimitive(int value) => value;

        protected override int ConvertToPrimitive(TestIntId intId) => intId;
    }
}