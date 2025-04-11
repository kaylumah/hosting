// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using Xunit;
using Xunit.Abstractions;

namespace Test.Unit.Core
{
    public class StronglyTypedGuidIdTests : StronglyTypedIdTests<TestGuidId, Guid>
    {
        readonly ITestOutputHelper _TestOutputHelper;

        public StronglyTypedGuidIdTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _TestOutputHelper = testOutputHelper;
        }

        protected override Guid SampleValue => Guid.Parse("77928f1f-318f-45ca-8bb2-fae02ed62e9f");

        protected override Guid EmptyValue => Guid.Empty;

        protected override TestGuidId ConvertFromPrimitive(Guid value) => value;

        protected override Guid ConvertToPrimitive(TestGuidId guidId) => guidId;

        public static IEnumerable<object[]> GuidTestData()
        {
            Faker faker = new Faker();
            string[] serializers = new[] { Json, Yaml, Xml };

            Guid[] edgeCases = new Guid[]
            {
                Guid.Empty
            };
            int numberOfRandomCases = 100 - edgeCases.Length;
            IEnumerable<Guid> randomCases = Enumerable.Range(0, numberOfRandomCases).Select(_ => faker.Random.Guid());
            Guid[] testCases = edgeCases.Concat(randomCases).ToArray();

            foreach (string serializer in serializers)
            {
                foreach (Guid value in testCases)
                {
                    object[] arguments = new object[] { serializer, value };
                    yield return arguments;
                }
            }
        }

        [Theory]
        [MemberData(nameof(GuidTestData))]
        public void Serializer_Should_SerializeAndDeserialize_FuzzedGuidValue(string serializer, Guid input)
        {
            try
            {
                string inputAsString = input.ToString();
                TestGuidId strongTypedId = ConvertFromPrimitive(input);
                string serialized = Serialize(strongTypedId, serializer);
                Assert.Contains(inputAsString, serialized, StringComparison.OrdinalIgnoreCase);
                TestGuidId deserialized = Deserialize<TestGuidId>(serialized, serializer);
            }
            catch (Exception ex)
            {
                string message = $"Fuzzing failed for input GUID: '{input}'. Exception: {ex}";
                _TestOutputHelper.WriteLine(message);
                Assert.Fail(message);
            }
        }
    }

    public readonly record struct TestGuidId(Guid Value)
    {
        public static implicit operator Guid(TestGuidId guidId) => guidId.Value;
        public static implicit operator TestGuidId(Guid value) => new(value);
    }
}