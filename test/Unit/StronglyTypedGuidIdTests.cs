// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using Xunit.Abstractions;

namespace Test.Unit
{
    public abstract class StronglyTypedGuidIdTests<TStrongTypedId> : StronglyTypedIdTests<TStrongTypedId, Guid> where TStrongTypedId : struct
    {
        protected StronglyTypedGuidIdTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }
        
        protected override Guid SampleValue => Guid.Parse("77928f1f-318f-45ca-8bb2-fae02ed62e9f");

        protected override Guid EmptyValue => Guid.Empty;
    }
    
    public readonly record struct TestGuidId(Guid Value)
    {
        public static implicit operator Guid(TestGuidId guidId) => guidId.Value;
        public static implicit operator TestGuidId(Guid value) => new(value);
    }
    
    public class TestGuidIdTests : StronglyTypedGuidIdTests<TestGuidId>
    {
        public TestGuidIdTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        protected override TestGuidId ConvertFromPrimitive(Guid value) => value;

        protected override Guid ConvertToPrimitive(TestGuidId guidId) => guidId;
    }
}