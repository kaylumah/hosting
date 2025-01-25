// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text.Json;
using Ssg.Extensions.Metadata.Abstractions;
using Xunit;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Test.Unit
{
    public abstract class StronglyTypedIdTests<TId, TPrimitive> where TId : struct
    {
        protected abstract TPrimitive SampleValue
        { get; }
        protected abstract TId ConvertFromPrimitive(TPrimitive value);
        protected abstract TPrimitive ConvertToPrimitive(TId id);

        [Fact]
        public void SystemTextJson_Should_SerializeAndDeserialize()
        {
            TPrimitive originalValue = SampleValue;
            string originalValueAsString = originalValue?.ToString() ?? string.Empty;

            TId id = ConvertFromPrimitive(originalValue);

            string json = JsonSerializer.Serialize(id);
            Assert.Contains(originalValueAsString, json);

            TId deserialized = JsonSerializer.Deserialize<TId>(json);
            Assert.Equal(id, deserialized);
        }
        
        [Fact]
        public void YamlDotNet_Should_SerializeAndDeserialize()
        {
            TPrimitive originalValue = SampleValue;
            string originalValueAsString = originalValue?.ToString() ?? string.Empty;
            TId id = ConvertFromPrimitive(originalValue);

            ISerializer serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            IDeserializer deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            string yaml = serializer.Serialize(id);
            Assert.Contains(originalValueAsString, yaml);

            TId deserialized = deserializer.Deserialize<TId>(yaml);
            Assert.Equal(id, deserialized);
        }
        
        [Fact]
        public void DataContractSerializer_Should_SerializeAndDeserialize()
        {
            TPrimitive originalValue = SampleValue;
            string originalValueAsString = originalValue?.ToString() ?? string.Empty;
            TId id = ConvertFromPrimitive(originalValue);
            
            DataContractSerializer serializer = new DataContractSerializer(typeof(TId));

            using MemoryStream memoryStream = new MemoryStream();
            serializer.WriteObject(memoryStream, id);
            memoryStream.Position = 0;

            string xml = new StreamReader(memoryStream).ReadToEnd();
            Assert.Contains(originalValueAsString, xml);

            memoryStream.Position = 0;
            TId? deserialized = (TId?)serializer.ReadObject(memoryStream);
            Assert.NotNull(deserialized);
            Assert.Equal(id, deserialized);
        }
    }

    public class AuthorIdTests : StronglyTypedIdTests<AuthorId, string>
    {
        protected override string SampleValue => "12345";

        protected override AuthorId ConvertFromPrimitive(string value) => value;

        protected override string ConvertToPrimitive(AuthorId id) => id;
    }

#pragma warning disable

    public class AuthorIdSerializationTests
    {
        private const string TestValue = "12345";

        // 1. Test System.Text.Json serialization and deserialization
        [Fact]
        public void SystemTextJson_Should_SerializeAndDeserialize_AuthorId()
        {
            var authorId = new AuthorId(TestValue);

            // Serialize
            string json = JsonSerializer.Serialize(authorId);
            Console.WriteLine("System.Text.Json Serialized:\n" + json);

            // Deserialize
            var deserialized = JsonSerializer.Deserialize<AuthorId>(json);

            Assert.Equal(authorId, deserialized);
            Assert.Contains(TestValue, json);
        }

        // 2. Test DataContractSerializer serialization and deserialization
        [Fact]
        public void DataContractSerializer_Should_SerializeAndDeserialize_AuthorId()
        {
            var authorId = new AuthorId(TestValue);

            var serializer = new DataContractSerializer(typeof(AuthorId));

            using var memoryStream = new MemoryStream();
            serializer.WriteObject(memoryStream, authorId);

            // Convert memory stream to string to inspect XML output
            memoryStream.Position = 0;
            string xml = new StreamReader(memoryStream).ReadToEnd();
            Console.WriteLine("DataContractSerializer Serialized:\n" + xml);

            // Deserialize
            memoryStream.Position = 0;
            var deserialized = (AuthorId)serializer.ReadObject(memoryStream);

            Assert.Equal(authorId, deserialized);
            Assert.Contains("<Value>12345</Value>", xml);
        }

        // 3. Test YamlDotNet serialization and deserialization
        [Fact]
        public void YamlDotNet_Should_SerializeAndDeserialize_AuthorId()
        {
            var authorId = new AuthorId(TestValue);

            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            // Serialize to YAML
            string yaml = serializer.Serialize(authorId);
            Console.WriteLine("YamlDotNet Serialized:\n" + yaml);

            // Deserialize from YAML
            var deserialized = deserializer.Deserialize<AuthorId>(yaml);

            Assert.Equal(authorId, deserialized);
            Assert.Contains("value: 12345", yaml.ToLower());
        }

        // 4. Implicit conversion test: AuthorId -> string
        [Fact]
        public void ImplicitConversion_Should_Convert_AuthorId_To_String()
        {
            AuthorId authorId = new AuthorId(TestValue);
            string stringValue = authorId; // Implicit conversion
            Assert.Equal(TestValue, stringValue);
        }

        // 5. Implicit conversion test: string -> AuthorId
        [Fact]
        public void ImplicitConversion_Should_Convert_String_To_AuthorId()
        {
            AuthorId authorId = TestValue; // Implicit conversion
            Assert.Equal(TestValue, authorId.Value);
        }

        // 6. Null string to AuthorId conversion
        [Fact]
        public void ImplicitConversion_Should_Handle_NullString_To_AuthorId()
        {
            AuthorId authorId = (string)null!;
            Assert.Equal(null, authorId.Value);
        }

        // 7. Empty string to AuthorId conversion
        [Fact]
        public void ImplicitConversion_Should_Handle_EmptyString_To_AuthorId()
        {
            AuthorId authorId = "";
            Assert.Equal(string.Empty, authorId.Value);
        }

        // 8. Test value equality for same AuthorId values
        [Fact]
        public void AuthorId_Should_BeEqual_When_ValuesAreSame()
        {
            AuthorId id1 = "123";
            AuthorId id2 = "123";
            Assert.Equal(id1, id2);
        }

        // 9. Test value inequality for different AuthorId values
        [Fact]
        public void AuthorId_Should_NotBeEqual_When_ValuesAreDifferent()
        {
            AuthorId id1 = "123";
            AuthorId id2 = "456";
            Assert.NotEqual(id1, id2);
        }

        // 10. Case sensitivity in equality checks
        [Fact]
        public void AuthorId_Should_BeCaseSensitive()
        {
            AuthorId id1 = "abc";
            AuthorId id2 = "ABC";
            Assert.NotEqual(id1, id2);
        }

        // 11. Check serialization/deserialization of an object containing AuthorId
        public class Author
        {
            public AuthorId Id { get; set; }
        }

        [Fact]
        public void SystemTextJson_Should_SerializeAndDeserialize_AuthorClass()
        {
            var author = new Author { Id = new AuthorId(TestValue) };

            string json = JsonSerializer.Serialize(author);
            Console.WriteLine("Serialized Author:\n" + json);

            var deserialized = JsonSerializer.Deserialize<Author>(json);
            Assert.Equal(author.Id, deserialized!.Id);
        }

        // 12. Hash code consistency test
        [Fact]
        public void AuthorId_Should_Have_Consistent_HashCode()
        {
            AuthorId id1 = "12345";
            AuthorId id2 = "12345";
            Assert.Equal(id1.GetHashCode(), id2.GetHashCode());
        }

        // 13. Test default value handling
        [Fact]
        public void Default_AuthorId_Should_BeEmpty()
        {
            AuthorId defaultId = default;
            Assert.Equal(null, defaultId.Value);
        }

        // 14. Test deserialization of malformed JSON
        [Fact]
        public void SystemTextJson_Should_Throw_When_DataIsMalformed()
        {
            string invalidJson = "{ \"Value\": 12345 }"; // Invalid JSON for string
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<AuthorId>(invalidJson));
        }
    }
}