// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Bogus;
using Xunit;
using Xunit.Abstractions;

namespace Test.Unit
{
    public abstract class StronglyTypedStringIdTests<TStrongTypedId> : StronglyTypedIdTests<TStrongTypedId, string> where TStrongTypedId : struct
    {
        protected StronglyTypedStringIdTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        protected override string SampleValue => "12345";

        protected override string EmptyValue => string.Empty;

        #pragma warning disable CA1000 // static members
        public static IEnumerable<object[]> StringTestData()
        {
            // Kaylumah in Japanese Katakana
            // ケイルマ (Keiruma)
            // カ (Ka) → Closest match to “Kay”, but “ケイ (Kei)” is more accurate for the long ‘ay’ sound.
            // ル (Ru) → Represents the “lu” sound.
            // マ (Ma) → Represents the “mah” sound.
            string japaneseKatakana = "ケイルマ";
            
            // Καϊλουμά (Kaïloumá)
            // Κ (Ka) → Closest match to “K” in Greek.
            // α (a) → Represents the short "a" sound.
            // ϊ (ï) → The diaeresis (¨) ensures "i" is pronounced separately, making it "Ka-ï" instead of "Kai" as one syllable.
            // λ (l) → Represents the "l" sound.
            // ο (o) → Represents the "o" sound, similar to "u" in "lumah".
            // υ (u) → Represents the "u" sound, which is often pronounced like **"i"** in modern Greek but retains an "u" value in some transliterations.
            // μ (m) → Represents the "m" sound.
            // ά (á) → The **accented α** ensures the stress is on the last syllable ("-má").
            string greek = "Καϊλουμά";

            string emoji = "🔥";

            string whiteSpace = "   ";

            string escape = "\u0000";
            
            // "\t\n\r" (Whitespace, escape characters)
            string[] serializers = new[] { Json, Yaml, Xml };
            string[] input = new[] { japaneseKatakana, greek, emoji, whiteSpace, escape };

            foreach (string serializer in serializers)
            {
                foreach (string value in input)
                {
                    object[] arguments = new object[] { serializer, value };
                    yield return arguments;
                }
            }
        }
        #pragma warning restore CA1000
        
        [Theory(Skip = "Contains Failures")]
        [MemberData(nameof(StringTestData))]
        public void Serializer_Should_SerializeAndDeserialize_SpecialStringValue(string serializer, string input)
        {
            TStrongTypedId strongTypedId = ConvertFromPrimitive(input);

            string serialized = Serialize(strongTypedId, serializer);
            Assert.Contains(input, serialized, StringComparison.OrdinalIgnoreCase);

            TStrongTypedId deserialized = Deserialize<TStrongTypedId>(serialized, serializer);
            Assert.Equal(strongTypedId, deserialized);
        }
        
        [Theory]
        [InlineData(Json)]
        [InlineData(Yaml)]
        [InlineData(Xml)]
        public void Serializer_Should_SerializeAndDeserialize_FuzzedStringValue(string serializer)
        {
            Faker faker = new Faker();
            
            for (int i = 0; i < 100; i++)
            {
                string randomString = faker.Random.String2(10, 200);
                TStrongTypedId strongTypedId = ConvertFromPrimitive(randomString);
                string serialized = Serialize(strongTypedId, serializer);
                Assert.Contains(randomString, serialized, StringComparison.OrdinalIgnoreCase);
                TStrongTypedId deserialized = Deserialize<TStrongTypedId>(serialized, serializer);

                /*
                try
                {
                    var deserialized = JsonSerializer.Deserialize<Library>(json, jsonOptions);
                    Assert.NotNull(deserialized.Author);
                }
                catch (Exception ex)
                {
                    Assert.True(false, $"Fuzzing failed for input: {randomString}. Exception: {ex.Message}");
                }
                */
            }
        }
    }
    
    public readonly record struct TestStringId(string Value)
    {
        public static implicit operator string(TestStringId stringId) => stringId.Value;
        public static implicit operator TestStringId(string value) => new(value);
    }

    public class TestStringIdTests : StronglyTypedStringIdTests<TestStringId>
    {
        public TestStringIdTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        protected override TestStringId ConvertFromPrimitive(string value) => value;

        protected override string ConvertToPrimitive(TestStringId stringId) => stringId;
    }
}