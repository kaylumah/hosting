// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Bogus;
using Xunit;
using Xunit.Abstractions;

namespace Test.Unit
{
    public class StronglyTypedIntIdTests : StronglyTypedIdTests<TestIntId, int>
    {
        readonly ITestOutputHelper _TestOutputHelper;

        public StronglyTypedIntIdTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _TestOutputHelper = testOutputHelper;
        }

        protected override int SampleValue => 3;

        protected override int EmptyValue => 0;

        protected override TestIntId ConvertFromPrimitive(int value) => value;

        protected override int ConvertToPrimitive(TestIntId intId) => intId;

        public static IEnumerable<object[]> IntTestData()
        {
            Faker faker = new Faker();
            string[] serializers = new[] { Json, Yaml, Xml };

            int[] edgeCases = new int[]
            { 
            // long.MinValue, 
            // long.MaxValue, 
               int.MinValue,
               -1,
               0,
               1,
               int.MaxValue
            };
            int numberOfRandomCases = 100 - edgeCases.Length;
            IEnumerable<int> randomCases = Enumerable.Range(0, numberOfRandomCases).Select(_ => faker.Random.Int(int.MinValue, int.MaxValue));
            int[] testCases = edgeCases.Concat(randomCases).ToArray();

            foreach (string serializer in serializers)
            {
                foreach (int value in testCases)
                {
                    object[] arguments = new object[] { serializer, value };
                    yield return arguments;
                }
            }
        }

        [Theory]
        [MemberData(nameof(IntTestData))]
        public void Serializer_Should_SerializeAndDeserialize_FuzzedIntValue(string serializer, int input)
        {
            try
            {
                string inputAsString = input.ToString(CultureInfo.InvariantCulture);
                TestIntId strongTypedId = ConvertFromPrimitive(input);
                string serialized = Serialize(strongTypedId, serializer);
                Assert.Contains(inputAsString, serialized, StringComparison.OrdinalIgnoreCase);
                TestIntId deserialized = Deserialize<TestIntId>(serialized, serializer);
            }
            catch (Exception ex)
            {
                string message = $"Fuzzing failed for input INT: '{input}'. Exception: {ex}";
                _TestOutputHelper.WriteLine(message);
                Assert.Fail(message);
            }
        }
    }

    public readonly record struct TestIntId(int Value)
    {
        public static implicit operator int(TestIntId intId) => intId.Value;
        public static implicit operator TestIntId(int value) => new(value);
    }
}