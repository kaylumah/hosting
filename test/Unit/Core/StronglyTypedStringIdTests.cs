// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Bogus;
using Xunit;
using Xunit.Abstractions;

namespace Test.Unit.Core
{
    public class StronglyTypedStringIdTests : StronglyTypedIdTests<TestStringId, string>
    {
        readonly ITestOutputHelper _TestOutputHelper;

        public StronglyTypedStringIdTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _TestOutputHelper = testOutputHelper;
        }

        protected override string SampleValue => "12345";

        protected override string EmptyValue => string.Empty;

        protected override TestStringId ConvertFromPrimitive(string value) => value;

        protected override string ConvertToPrimitive(TestStringId stringId) => stringId;

        public static IEnumerable<object[]> StringTestData()
        {
            // Kaylumah in Japanese Katakana
            // ケイルマ (Keiruma)
            // カ (Ka) → Closest match to “Kay”, but “ケイ (Kei)” is more accurate for the long ‘ay’ sound.
            // ル (Ru) → Represents the “lu” sound.
            // マ (Ma) → Represents the “mah” sound.
            // string japaneseKatakana = "ケイルマ";

            // Καϊλουμά (Kaïloumá)
            // Κ (Ka) → Closest match to “K” in Greek.
            // α (a) → Represents the short "a" sound.
            // ϊ (ï) → The diaeresis (¨) ensures "i" is pronounced separately, making it "Ka-ï" instead of "Kai" as one syllable.
            // λ (l) → Represents the "l" sound.
            // ο (o) → Represents the "o" sound, similar to "u" in "lumah".
            // υ (u) → Represents the "u" sound, which is often pronounced like **"i"** in modern Greek but retains an "u" value in some transliterations.
            // μ (m) → Represents the "m" sound.
            // ά (á) → The **accented α** ensures the stress is on the last syllable ("-má").
            // string greek = "Καϊλουμά";

            // string emoji = "🔥";
            // string escape = "\u0000";

            Faker faker = new Faker();
            string[] serializers = new[] { Json, Yaml, Xml };

            string[] edgeCases = new string[]
            {
                string.Empty, // Completely empty string
                "   ",        // Whitespace-only string
                // "\t\n\r",       // Escape characters (tab, newline, carriage return) (fails in all three)
                // "<script>alert('XSS')</script>", // Potential serialization issue (fails in DataContract)
                "{}",          // JSON-like structure
                "[ ]",         // Array-like structure
            };
            int numberOfRandomCases = 100 - edgeCases.Length;
            IEnumerable<string> randomCases = Enumerable.Range(0, numberOfRandomCases).Select(_ => faker.Random.String2(10, 200));
            string[] testCases = edgeCases.Concat(randomCases).ToArray();

            foreach (string serializer in serializers)
            {
                foreach (string value in testCases)
                {
                    object[] arguments = new object[] { serializer, value };
                    yield return arguments;
                }
            }
        }

        [Theory]
        [MemberData(nameof(StringTestData))]
        public void Serializer_Should_SerializeAndDeserialize_FuzzedStringValue(string serializer, string input)
        {
            try
            {
                TestStringId strongTypedId = ConvertFromPrimitive(input);
                string serialized = Serialize(strongTypedId, serializer);
                Assert.Contains(input, serialized, StringComparison.OrdinalIgnoreCase);
                TestStringId deserialized = Deserialize<TestStringId>(serialized, serializer);
            }
            catch (Exception ex)
            {
                string message = $"Fuzzing failed for input STRING: '{input}'. Exception: {ex}";
                _TestOutputHelper.WriteLine(message);
                Assert.Fail(message);
            }
        }
    }

    public readonly record struct TestStringId(string Value)
    {
        public static implicit operator string(TestStringId stringId) => stringId.Value;
        public static implicit operator TestStringId(string value) => new(value);
    }
}