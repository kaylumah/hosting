// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Xml;
using Kaylumah.Ssg.Manager.Site.Service;
using Xunit;
using Xunit.Abstractions;
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
            public TStrongTypedId Id
            { get; set; }

            // Add List of ComplexDto?
        }

        protected const string Json = "SystemTextJson";
        const string Yaml = "YamlDotNet";
        const string Xml = "DataContract";
        readonly ITestOutputHelper _TestOutputHelper;

        protected StronglyTypedIdTests(ITestOutputHelper testOutputHelper)
        {
            _TestOutputHelper = testOutputHelper;
        }

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

        [Theory]
        [InlineData(Json)]
        [InlineData(Yaml)]
        [InlineData(Xml)]
        public void Serializer_Should_SerializeAndDeserialize_SingleValue(string serializer)
        {
            string originalValueAsString = SampleValue?.ToString() ?? throw new InvalidOperationException("Sample Value must be set");
            TStrongTypedId strongTypedId = ConvertFromPrimitive(SampleValue);

            string serialized = Serialize(strongTypedId, serializer);
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
            string originalValueAsString = SampleValue?.ToString() ?? throw new InvalidOperationException("Sample Value must be set");
            TStrongTypedId strongTypedId = ConvertFromPrimitive(SampleValue);
            List<TStrongTypedId> list = [strongTypedId];

            string serialized = Serialize(list, serializer);
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
            string originalValueAsString = SampleValue?.ToString() ?? throw new InvalidOperationException("Sample Value must be set");
            TStrongTypedId strongTypedId = ConvertFromPrimitive(SampleValue);
            TStrongTypedId[] array = [strongTypedId];

            string serialized = Serialize(array, serializer);
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
            string originalValueAsString = SampleValue?.ToString() ?? throw new InvalidOperationException("Sample Value must be set");
            TStrongTypedId strongTypedId = ConvertFromPrimitive(SampleValue);
            Dictionary<TStrongTypedId, string> dictionary = new Dictionary<TStrongTypedId, string>
            {
                { strongTypedId, "abc" }
            };

            string serialized = Serialize(dictionary, serializer);
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
            string originalValueAsString = SampleValue?.ToString() ?? throw new InvalidOperationException("Sample Value must be set");
            TStrongTypedId strongTypedId = ConvertFromPrimitive(SampleValue);
            ComplexDto dto = new ComplexDto();
            dto.Id = strongTypedId;

            string serialized = Serialize(dto, serializer);
            Assert.Contains(originalValueAsString, serialized, StringComparison.OrdinalIgnoreCase);

            ComplexDto deserializedDto = Deserialize<ComplexDto>(serialized, serializer);
            TStrongTypedId? deserialized = deserializedDto.Id;
            Assert.Equal(strongTypedId, deserialized);
        }

        protected string Serialize<T>(T value, string format)
        {
            string result = format switch
            {
                Json => SerializeJson(value),
                Yaml => SerializeYaml(value),
                Xml => SerializeXml(value),
                _ => throw new ArgumentException("Invalid format", nameof(format))
            };

            bool isEmptyString = string.IsNullOrWhiteSpace(result);
            Assert.False(isEmptyString, "Serialized string should not be empty");

            _TestOutputHelper.WriteLine(result);
            return result;
        }

        protected T Deserialize<T>(string serialized, string format) => format switch
        {
            Json => DeserializeJson<T>(serialized),
            Yaml => DeserializeYaml<T>(serialized),
            Xml => DeserializeXml<T>(serialized),
            _ => throw new ArgumentException("Invalid format", nameof(format))
        };

        string SerializeXml<T>(T obj)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(T));
            UTF8Encoding encoding = new UTF8Encoding(false);
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.Indent = true;
            xmlWriterSettings.Encoding = encoding;

            using MemoryStream memoryStream = new MemoryStream();
            using XmlWriter writer = XmlWriter.Create(memoryStream, xmlWriterSettings);

            serializer.WriteObject(writer, obj);
            writer.Flush();
            writer.Close();

            byte[] bytes = memoryStream.ToArray();
            string result = encoding.GetString(bytes);
            return result;
        }

        T DeserializeXml<T>(string xml)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(T));
            UTF8Encoding encoding = new UTF8Encoding(false);
            byte[] byteArray = encoding.GetBytes(xml);

            using MemoryStream memoryStream = new MemoryStream(byteArray);
            using XmlReader reader = XmlReader.Create(memoryStream);

            return (T)serializer.ReadObject(reader)!;
        }

        string SerializeJson<T>(T obj)
        {
#pragma warning disable CA1869
            JsonSerializerOptions jsonOptions = new();
            jsonOptions.WriteIndented = true;
            jsonOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            jsonOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            jsonOptions.PropertyNameCaseInsensitive = true;
            StronglyTypedIdJsonConverter<TStrongTypedId> converter = new StronglyTypedIdJsonConverter<TStrongTypedId>();
            jsonOptions.Converters.Add(converter);
#pragma warning restore CA1869

            using MemoryStream memoryStream = new MemoryStream();
            UTF8Encoding encoding = new UTF8Encoding(false); // Prevent BOM
            using StreamWriter writer = new StreamWriter(memoryStream, encoding);

            string json = JsonSerializer.Serialize(obj, jsonOptions);
            writer.Write(json);
            writer.Flush();

            byte[] bytes = memoryStream.ToArray();
            string result = encoding.GetString(bytes);
            return result;
        }

        T DeserializeJson<T>(string json)
        {
#pragma warning disable CA1869
            JsonSerializerOptions jsonOptions = new();
            jsonOptions.WriteIndented = true;
            jsonOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            jsonOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            jsonOptions.PropertyNameCaseInsensitive = true;
            StronglyTypedIdJsonConverter<TStrongTypedId> converter = new StronglyTypedIdJsonConverter<TStrongTypedId>();
            jsonOptions.Converters.Add(converter);
#pragma warning restore CA1869

            byte[] byteArray = new UTF8Encoding(false).GetBytes(json);
            using MemoryStream memoryStream = new MemoryStream(byteArray);
            using StreamReader reader = new StreamReader(memoryStream, Encoding.UTF8);

            string jsonString = reader.ReadToEnd();
            return JsonSerializer.Deserialize<T>(jsonString, jsonOptions)!;
        }

        string SerializeYaml<T>(T obj)
        {
            StronglyTypedIdYamlConverter<TStrongTypedId> converter = new StronglyTypedIdYamlConverter<TStrongTypedId>();
            ISerializer serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithTypeConverter(converter)
                .Build();

            using MemoryStream memoryStream = new MemoryStream();
            UTF8Encoding encoding = new UTF8Encoding(false);
            using StreamWriter writer = new StreamWriter(memoryStream, encoding);

            string yaml = serializer.Serialize(obj);
            writer.Write(yaml);
            writer.Flush();

            byte[] bytes = memoryStream.ToArray();
            string result = encoding.GetString(bytes);
            return result;
        }

        T DeserializeYaml<T>(string yaml)
        {
            StronglyTypedIdYamlConverter<TStrongTypedId> converter = new StronglyTypedIdYamlConverter<TStrongTypedId>();
            IDeserializer deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithTypeConverter(converter)
                .Build();

            byte[] byteArray = new UTF8Encoding(false).GetBytes(yaml);
            using MemoryStream memoryStream = new MemoryStream(byteArray);
            using StreamReader reader = new StreamReader(memoryStream, Encoding.UTF8);

            string yamlString = reader.ReadToEnd();
            T result = deserializer.Deserialize<T>(yamlString);
            return result;
        }
    }
}