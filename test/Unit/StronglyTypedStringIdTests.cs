// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

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

        // Chinese: 你好世界
        // Emoji 💾📚
        // Escape \u0000
        // Int32.MinValue, Int32.MaxValue, Guid.Empty,  "   " 
        // Bool instead of string, number instead of guid
        // "\t\n\r" (Whitespace, escape characters)
        // NULL value
        // 	List with 100,000+ entries.
        // •	Large Dictionary<ChapterId, string> (100,000+ keys).
        /*
         Fuzz testing (or fuzzing) is a type of automated testing that provides random, unexpected, or malformed inputs to a system to uncover potential bugs, crashes, or security vulnerabilities.
         * var faker = new Faker();
           var randomJson = $"{{ \"Author\": \"{faker.Random.AlphaNumeric(50)}\" }}";
         */

        /*
         * Fact]
           public void SystemTextJson_Should_Handle_Fuzzed_String_Data_Gracefully()
           {
               var faker = new Faker();

               for (int i = 0; i < 100; i++)
               {
                   string randomString = faker.Random.String2(10, 200); // Random string between 10-200 chars
                   string json = $"{{ \"Author\": \"{randomString}\" }}";

                   try
                   {
                       var deserialized = JsonSerializer.Deserialize<Library>(json, jsonOptions);
                       Assert.NotNull(deserialized.Author);
                   }
                   catch (Exception ex)
                   {
                       Assert.True(false, $"Fuzzing failed for input: {randomString}. Exception: {ex.Message}");
                   }
               }
           }
           
           [Fact]
           public void SystemTextJson_Should_Handle_Fuzzed_Guids_Gracefully()
           {
               var faker = new Faker();

               for (int i = 0; i < 100; i++)
               {
                   string randomGuid = faker.Random.Guid().ToString();
                   string json = $"{{ \"BookId\": \"{randomGuid}\" }}";

                   try
                   {
                       var deserialized = JsonSerializer.Deserialize<Library>(json, jsonOptions);
                       Assert.NotNull(deserialized.BookId);
                   }
                   catch (Exception ex)
                   {
                       Assert.True(false, $"Fuzzing failed for input: {randomGuid}. Exception: {ex.Message}");
                   }
               }
           }

           [Fact]
           public void SystemTextJson_Should_Handle_Fuzzed_Numeric_Data_Gracefully()
           {
               var faker = new Faker();

               for (int i = 0; i < 100; i++)
               {
                   int randomInt = faker.Random.Int(int.MinValue, int.MaxValue);
                   string json = $"{{ \"ChapterId\": \"{randomInt}\" }}";

                   try
                   {
                       var deserialized = JsonSerializer.Deserialize<Library>(json, jsonOptions);
                       Assert.NotNull(deserialized.ChapterId);
                   }
                   catch (Exception ex)
                   {
                       Assert.True(false, $"Fuzzing failed for input: {randomInt}. Exception: {ex.Message}");
                   }
               }
           }
           
           [Fact]
           public void SystemTextJson_Should_Handle_Fuzzed_Malformed_Data()
           {
               var faker = new Faker();

               for (int i = 0; i < 50; i++)
               {
                   string json = faker.Lorem.Sentence(); // Generates completely random, nonsense JSON

                   try
                   {
                       JsonSerializer.Deserialize<Library>(json, jsonOptions);
                   }
                   catch (JsonException)
                   {
                       // Expected behavior: Should throw a JsonException for invalid format.
                   }
                   catch (Exception ex)
                   {
                       Assert.True(false, $"Unexpected exception for fuzzed input: {json}. Exception: {ex.Message}");
                   }
               }
           }
           
           [Fact]
           public void SystemTextJson_Should_Handle_Fuzzed_Data()
           {
               var faker = new Faker();

               for (int i = 0; i < 100; i++)
               {
                   string randomString = faker.Random.String2(10, 200);
                   string randomGuid = faker.Random.Guid().ToString();
                   int randomInt = faker.Random.Int(int.MinValue, int.MaxValue);

                   string json = $@"
                   {{
                       ""Author"": ""{randomString}"",
                       ""BookId"": ""{randomGuid}"",
                       ""ChapterId"": ""{randomInt}""
                   }}";

                   try
                   {
                       var deserialized = JsonSerializer.Deserialize<Library>(json, jsonOptions);
                       Assert.NotNull(deserialized);
                   }
                   catch (Exception ex)
                   {
                       Assert.True(false, $"Fuzzing failed for input: {json}. Exception: {ex.Message}");
                   }
               }
           }
         */
    }
    
    public readonly record struct TestStringId(string Value)
    {
        public static implicit operator string(TestStringId authorStringId) => authorStringId.Value;
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