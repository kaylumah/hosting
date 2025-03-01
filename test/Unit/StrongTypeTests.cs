// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Xml;
using Ssg.Extensions.Metadata.Abstractions;
using Xunit;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Test.Unit
{
    public abstract class StronglyTypedIdTests<TStrongTypedId, TPrimitive>
        where TStrongTypedId : struct
    {
        // ReSharper disable once MemberCanBePrivate.Global
        // Must be public or get system runtime attributes
        public class ComplexDto
        {
            public TStrongTypedId? Id
            { get; set; }

            // Add List of ComplexDto?
        }

        protected const string Json = "json"; //"SystemTextJson";
        protected const string Yaml = "yaml"; //"YamlDotNet";
        protected const string Xml = "xml"; //"DataContract";

        protected abstract TPrimitive SampleValue
        { get; }

        protected abstract TPrimitive EmptyValue
        { get; }

        protected abstract TStrongTypedId ConvertFromPrimitive(TPrimitive value);

        protected abstract TPrimitive ConvertToPrimitive(TStrongTypedId id);

        [Fact]
        public void ImplicitConversion_Should_ConvertToPrimitive()
        {
            TStrongTypedId id = ConvertFromPrimitive(SampleValue);
            TPrimitive primitive = ConvertToPrimitive(id);
            Assert.Equal(SampleValue, primitive);
        }

        [Fact]
        public void ImplicitConversion_Should_ConvertFromPrimitive()
        {
            TStrongTypedId id = ConvertFromPrimitive(SampleValue);
            TPrimitive primitive = ConvertToPrimitive(id);
            Assert.Equal(SampleValue, primitive);
        }

        [Fact]
        public void Equality_Should_BeTrue_ForSameValue()
        {
            TStrongTypedId id1 = ConvertFromPrimitive(SampleValue);
            TStrongTypedId id2 = ConvertFromPrimitive(SampleValue);
            Assert.Equal(id1, id2);
        }

        [Fact]
        public void Equality_Should_BeFalse_ForDifferentValues()
        {
            TStrongTypedId id1 = ConvertFromPrimitive(SampleValue);
            TStrongTypedId id2 = ConvertFromPrimitive(EmptyValue);
            Assert.NotEqual(id1, id2);
        }

        [Fact]
        public void HashCode_Should_BeConsistent()
        {
            TStrongTypedId id1 = ConvertFromPrimitive(SampleValue);
            TStrongTypedId id2 = ConvertFromPrimitive(SampleValue);
            int id1HashCode = id1.GetHashCode();
            int id2HashCode = id2.GetHashCode();
            Assert.Equal(id1HashCode, id2HashCode);
        }

        [Fact]
        public void HashCode_Should_Differ_ForDifferentValues()
        {
            TStrongTypedId id1 = ConvertFromPrimitive(SampleValue);
            TStrongTypedId id2 = ConvertFromPrimitive(EmptyValue);
            int id1HashCode = id1.GetHashCode();
            int id2HashCode = id2.GetHashCode();
            Assert.NotEqual(id1HashCode, id2HashCode);
        }

        /*
        public void DefaultValue_Should_BeHandledCorrectly()
        {
            // NULL value vs string.Empty
            TStrongTypedId defaultId = default;
            TStrongTypedId emptyId = ConvertFromPrimitive(EmptyValue);
            Assert.Equal(emptyId, defaultId);
        }
        */

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

        [Fact(Skip = "Not sure if relevant")]
        public void SystemTextJson_Should_Throw_When_DataIsMalformed()
        {
            string invalidJson = "{ \"Value\": 12345 }"; // Expecting a string but got an integer
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<TStrongTypedId>(invalidJson));
        }

        [Fact(Skip = "not ready")]
        public void SystemTextJson_Should_SerializeAndDeserializeDictionary()
        {
            string originalValueAsString = SampleValue?.ToString() ?? string.Empty;
            TStrongTypedId id = ConvertFromPrimitive(SampleValue);
            Dictionary<TStrongTypedId, string> data = new();
            data[id] = "one";

#pragma warning disable
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                Converters = { new StringValueRecordStructConverter<AuthorId>(
                    value => value, id2 => id2) }
            };

            string json = JsonSerializer.Serialize(data, options);
            Assert.Contains(originalValueAsString, json);

            // TStrongTypedId deserialized = JsonSerializer.Deserialize<TStrongTypedId>(json);
            // Assert.Equal(id, deserialized);
        }

        static IEnumerable<object[]> SingleValueTestData()
        {
            string[] formatters = new string[1];

            foreach (string formatter in formatters)
            {
                // Nested foreach
                yield return new object[] { };
            }
        }

        [Theory]
        [InlineData(Json)]
        [InlineData(Yaml)]
        [InlineData(Xml)]
        public void Serializer_Should_SerializeAndDeserialize_SingleValue(string serializer)
        {
            string originalValueAsString = SampleValue.ToString();
            TStrongTypedId strongTypedId = ConvertFromPrimitive(SampleValue);

            string serialized = Serialize(strongTypedId, serializer);
            Assert.False(string.IsNullOrWhiteSpace(serialized), "Serialized string should not be empty");
            Assert.Contains(originalValueAsString, serialized, StringComparison.OrdinalIgnoreCase);

            TStrongTypedId deserialized = Deserialize<TStrongTypedId>(serialized, serializer);
            Assert.Equal(strongTypedId, deserialized);
        }

        [Theory]
        [InlineData(Json)]
        [InlineData(Yaml)]
        [InlineData(Xml)]
        public void Serializer_Should_SerializeAndDeserialize_List(string serializer)
        {
            string originalValueAsString = SampleValue.ToString();
            TStrongTypedId strongTypedId = ConvertFromPrimitive(SampleValue);
            List<TStrongTypedId> list = [strongTypedId];

            string serialized = Serialize(list, serializer);
            Assert.False(string.IsNullOrWhiteSpace(serialized), "Serialized string should not be empty");
            Assert.Contains(originalValueAsString, serialized, StringComparison.OrdinalIgnoreCase);

            List<TStrongTypedId> deserializedList = Deserialize<List<TStrongTypedId>>(serialized, serializer);
            TStrongTypedId deserialized = deserializedList.ElementAt(0);
            Assert.Equal(strongTypedId, deserialized);
        }

        [Theory]
        [InlineData(Json)]
        [InlineData(Yaml)]
        [InlineData(Xml)]
        public void Serializer_Should_SerializeAndDeserialize_Array(string serializer)
        {
            string originalValueAsString = SampleValue.ToString();
            TStrongTypedId strongTypedId = ConvertFromPrimitive(SampleValue);
            TStrongTypedId[] array = [strongTypedId];

            string serialized = Serialize(array, serializer);
            Assert.False(string.IsNullOrWhiteSpace(serialized), "Serialized string should not be empty");
            Assert.Contains(originalValueAsString, serialized, StringComparison.OrdinalIgnoreCase);

            TStrongTypedId[] deserializedArray = Deserialize<TStrongTypedId[]>(serialized, serializer);
            TStrongTypedId deserialized = deserializedArray[0];
            Assert.Equal(strongTypedId, deserialized);
        }

        [Theory]
        [InlineData(Json)]
        [InlineData(Yaml)]
        [InlineData(Xml)]
        public void Serializer_Should_SerializeAndDeserialize_Dictionary(string serializer)
        {
            string originalValueAsString = SampleValue.ToString();
            TStrongTypedId strongTypedId = ConvertFromPrimitive(SampleValue);
            Dictionary<TStrongTypedId, string> dictionary = new Dictionary<TStrongTypedId, string>
            {
                { strongTypedId, "abc" }
            };

            string serialized = Serialize(dictionary, serializer);
            Assert.False(string.IsNullOrWhiteSpace(serialized), "Serialized string should not be empty");
            Assert.Contains(originalValueAsString, serialized, StringComparison.OrdinalIgnoreCase);

            Dictionary<TStrongTypedId, string> deserializedDictionary = Deserialize<Dictionary<TStrongTypedId, string>>(serialized, serializer);
            TStrongTypedId? deserialized = deserializedDictionary.Keys.FirstOrDefault();
            Assert.Equal(strongTypedId, deserialized);
        }

        [Theory]
        [InlineData(Json)]
        [InlineData(Yaml)]
        [InlineData(Xml)]
        public void Serializer_Should_SerializeAndDeserialize_Dto(string serializer)
        {
            string originalValueAsString = SampleValue.ToString();
            TStrongTypedId strongTypedId = ConvertFromPrimitive(SampleValue);
            ComplexDto dto = new ComplexDto
            {
                Id = strongTypedId
            };

            string serialized = Serialize(dto, serializer);
            Assert.False(string.IsNullOrWhiteSpace(serialized), "Serialized string should not be empty");
            Assert.Contains(originalValueAsString, serialized, StringComparison.OrdinalIgnoreCase);

            ComplexDto deserializedDto = Deserialize<ComplexDto>(serialized, serializer);
            TStrongTypedId? deserialized = deserializedDto.Id;
            Assert.Equal(strongTypedId, deserialized);
        }

        string Serialize<T>(T value, string format) => format switch
        {
            Json => SerializeJson(value),
            Yaml => SerializeYaml(value),
            Xml => SerializeXml(value),
            _ => throw new ArgumentException("Invalid format", nameof(format))
        };

        T Deserialize<T>(string serialized, string format) => format switch
        {
            Json => DeserializeJson<T>(serialized),
            Yaml => DeserializeYaml<T>(serialized),
            Xml => DeserializeXml<T>(serialized),
            _ => throw new ArgumentException("Invalid format", nameof(format))
        };

        static string SerializeXml<T>(T obj)
        {
            DataContractSerializer serializer = CreateSerializer<T>();

            using MemoryStream memoryStream = new MemoryStream();
            UTF8Encoding encoding = new UTF8Encoding(false);
            using XmlWriter writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true, Encoding = encoding });

            serializer.WriteObject(writer, obj);
            writer.Flush();
            writer.Close();

            return encoding.GetString(memoryStream.ToArray());
        }

        static T DeserializeXml<T>(string xml)
        {
            DataContractSerializer serializer = CreateSerializer<T>();
            UTF8Encoding encoding = new UTF8Encoding(false);
            byte[] byteArray = encoding.GetBytes(xml);

            using MemoryStream memoryStream = new MemoryStream(byteArray);
            using XmlReader reader = XmlReader.Create(memoryStream);

            return (T)serializer.ReadObject(reader)!;
        }

        static DataContractSerializer CreateSerializer<T>()
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(T));
            return serializer;
        }

        static readonly JsonSerializerOptions _JsonOptions = new()
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        static string SerializeJson<T>(T obj)
        {
            using MemoryStream memoryStream = new MemoryStream();
            UTF8Encoding encoding = new UTF8Encoding(false); // Prevent BOM
            using StreamWriter writer = new StreamWriter(memoryStream, encoding);

            string json = JsonSerializer.Serialize(obj, _JsonOptions);
            writer.Write(json);
            writer.Flush();

            return encoding.GetString(memoryStream.ToArray());
        }

        static T DeserializeJson<T>(string json)
        {
            byte[] byteArray = new UTF8Encoding(false).GetBytes(json);
            using MemoryStream memoryStream = new MemoryStream(byteArray);
            using StreamReader reader = new StreamReader(memoryStream, Encoding.UTF8);

            string jsonString = reader.ReadToEnd();
            return JsonSerializer.Deserialize<T>(jsonString, _JsonOptions)!;
        }

        static string SerializeYaml<T>(T obj)
        {
            ISerializer serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            using MemoryStream memoryStream = new MemoryStream();
            UTF8Encoding encoding = new UTF8Encoding(false);
            using StreamWriter writer = new StreamWriter(memoryStream, encoding);

            string yaml = serializer.Serialize(obj);
            writer.Write(yaml);
            writer.Flush();

            return encoding.GetString(memoryStream.ToArray());
        }

        static T DeserializeYaml<T>(string yaml)
        {
            IDeserializer deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            byte[] byteArray = new UTF8Encoding(false).GetBytes(yaml);
            using MemoryStream memoryStream = new MemoryStream(byteArray);
            using StreamReader reader = new StreamReader(memoryStream, Encoding.UTF8);

            string yamlString = reader.ReadToEnd();
            return deserializer.Deserialize<T>(yamlString);
        }
    }

    public abstract class StronglyTypedStringIdTests<TStrongTypedId> : StronglyTypedIdTests<TStrongTypedId, string> where TStrongTypedId : struct
    {
        protected override string SampleValue => "12345";

        protected override string EmptyValue => string.Empty;
    }

    public readonly record struct TestId(string Value)
    {
        public static implicit operator string(TestId authorId) => authorId.Value;
        public static implicit operator TestId(string value) => new(value);
    }

    /// <summary>
    /// Represent the data structure of AuthorId, OrganizationId, TagId, SiteId, PageId
    /// </summary>
    public class TestIdTests : StronglyTypedStringIdTests<TestId>
    {
        protected override TestId ConvertFromPrimitive(string value) => value;

        protected override string ConvertToPrimitive(TestId id) => id;
    }
}