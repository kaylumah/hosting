// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Test.Unit
{
    public class Revisit
    {
        /*
                public void DefaultValue_Should_BeHandledCorrectly()
                {
                    // NULL value vs string.Empty
                    TStrongTypedId defaultId = default;
                    TStrongTypedId emptyId = ConvertFromPrimitive(EmptyValue);
                    Assert.Equal(emptyId, defaultId);
                }
                */
        
        /*
        [Fact(Skip = "Not sure if relevant")]
                public void SystemTextJson_Serialization_Should_BeFast()
                {
                    TStrongTypedId id = ConvertFromPrimitive(SampleValue);
                    Stopwatch stopwatch = Stopwatch.StartNew();
        
                    for (int i = 0; i < 100000; i++)
                    {
                        string json = JsonSerializer.Serialize(id);
                        _ = JsonSerializer.Deserialize<TStrongTypedId>(json);
                    }
        
                    stopwatch.Stop();
                    Assert.True(stopwatch.ElapsedMilliseconds < 2000, "Serialization too slow.");
                }
                */
        
        /*
[Fact(Skip = "Not sure if relevant")]
        public void SystemTextJson_Should_Throw_When_DataIsMalformed()
        {
            string invalidJson = "{ \"Value\": 12345 }"; // Expecting a string but got an integer
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<TStrongTypedId>(invalidJson));
        }
        */
        
        // Int32.MinValue, Int32.MaxValue, Guid.Empty,  "   " 
                // Bool instead of string, number instead of guid
                // NULL value
                // 	List with 100,000+ entries.
                // •	Large Dictionary<ChapterId, string> (100,000+ keys).
                /*
                 
                 * var faker = new Faker();
                   var randomJson = $"{{ \"Author\": \"{faker.Random.AlphaNumeric(50)}\" }}";
                 */
        
                /*
                 * 
                   
                   [Fact]
                   public void SystemTextJson_Should_Handle_Fuzzed_Malformed_Data()
                   {
                       var faker = new Faker();
        
                       for (int i = 0; i < 50; i++)
                       {
                           string json = faker.Lorem.Sentence(); // Generates completely random, nonsense JSON
                       }
                   }
                   
                   
                 */
    }
}