// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Text.Json;
using Ssg.Extensions.Metadata.Abstractions;
using Xunit;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
namespace Test.Unit
{
    public abstract class StronglyTypedIdTests<TStrongTypedId, TPrimitive>
        where TStrongTypedId : struct
    {
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

        [Fact]
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

        [Fact]
        public void SystemTextJson_Should_SerializeAndDeserialize()
        {
            string originalValueAsString = SampleValue?.ToString() ?? string.Empty;

            TStrongTypedId id = ConvertFromPrimitive(SampleValue);

            string json = JsonSerializer.Serialize(id);
            Assert.Contains(originalValueAsString, json);

            TStrongTypedId deserialized = JsonSerializer.Deserialize<TStrongTypedId>(json);
            Assert.Equal(id, deserialized);
        }
        
        [Fact]
        public void SystemTextJson_Should_SerializeAndDeserializeDictionary()
        {
            string originalValueAsString = SampleValue?.ToString() ?? string.Empty;
            TStrongTypedId id = ConvertFromPrimitive(SampleValue);
            Dictionary<TStrongTypedId, string> data = new();
            data[id] = "one";

            #pragma warning disable
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                Converters = { new StringValueRecordStructConverter<TStrongTypedId>(
                    value => ConvertToPrimitive(value), id => ConvertFromPrimitive(id)) }
            };
            
            string json = JsonSerializer.Serialize(data, options);
            Assert.Contains(originalValueAsString, json);

            // TStrongTypedId deserialized = JsonSerializer.Deserialize<TStrongTypedId>(json);
            // Assert.Equal(id, deserialized);
        }

        [Fact]
        public void YamlDotNet_Should_SerializeAndDeserialize()
        {
            string originalValueAsString = SampleValue?.ToString() ?? string.Empty;
            TStrongTypedId id = ConvertFromPrimitive(SampleValue);

            ISerializer serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            IDeserializer deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            string yaml = serializer.Serialize(id);
            Assert.Contains(originalValueAsString, yaml);

            TStrongTypedId deserialized = deserializer.Deserialize<TStrongTypedId>(yaml);
            Assert.Equal(id, deserialized);
        }

        [Fact]
        public void DataContractSerializer_Should_SerializeAndDeserialize()
        {
            string originalValueAsString = SampleValue?.ToString() ?? string.Empty;
            TStrongTypedId id = ConvertFromPrimitive(SampleValue);

            DataContractSerializer serializer = new DataContractSerializer(typeof(TStrongTypedId));

            using MemoryStream memoryStream = new MemoryStream();
            serializer.WriteObject(memoryStream, id);
            memoryStream.Position = 0;

            string xml = new StreamReader(memoryStream).ReadToEnd();
            Assert.Contains(originalValueAsString, xml);

            memoryStream.Position = 0;
            TStrongTypedId? deserialized = (TStrongTypedId?)serializer.ReadObject(memoryStream);
            Assert.NotNull(deserialized);
            Assert.Equal(id, deserialized);
        }
    }

    public abstract class StronglyTypedStringIdTests<TId> : StronglyTypedIdTests<TId, string> where TId : struct
    {
        protected override string SampleValue => "12345";

        protected override string EmptyValue => string.Empty;
    }

    public class AuthorIdTests : StronglyTypedStringIdTests<AuthorId>
    {
        protected override AuthorId ConvertFromPrimitive(string value) => value;

        protected override string ConvertToPrimitive(AuthorId id) => id;
    }

    public class OrganizationIdTests : StronglyTypedStringIdTests<OrganizationId>
    {
        protected override OrganizationId ConvertFromPrimitive(string value) => value;

        protected override string ConvertToPrimitive(OrganizationId id) => id;
    }

    public class TagIdTests : StronglyTypedStringIdTests<TagId>
    {
        protected override TagId ConvertFromPrimitive(string value) => value;

        protected override string ConvertToPrimitive(TagId id) => id;
    }

    public class SiteIdTests : StronglyTypedStringIdTests<SiteId>
    {
        protected override SiteId ConvertFromPrimitive(string value) => value;

        protected override string ConvertToPrimitive(SiteId id) => id;
    }

    public class PageIdTests : StronglyTypedStringIdTests<PageId>
    {
        protected override PageId ConvertFromPrimitive(string value) => value;

        protected override string ConvertToPrimitive(PageId id) => id;
    }
}