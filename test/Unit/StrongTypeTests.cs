// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

#pragma warning disable
using System.IO;
using System.Runtime.Serialization;
using System.Text.Json;
using Ssg.Extensions.Metadata.Abstractions;
using Xunit;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Test.Unit
{
    public abstract class StronglyTypedIdTests<TId, TPrimitive> 
        where TId : struct
    {
        protected abstract TPrimitive SampleValue
        { get; }

        protected abstract TPrimitive EmptyValue
        { get; }

        protected abstract TId ConvertFromPrimitive(TPrimitive? value);
        protected abstract TPrimitive ConvertToPrimitive(TId id);

        [Fact]
        public void ImplicitConversion_Should_ConvertToPrimitive()
        {
            TId id = ConvertFromPrimitive(SampleValue);
            TPrimitive primitive = ConvertToPrimitive(id);
            Assert.Equal(SampleValue, primitive);
        }

        [Fact]
        public void ImplicitConversion_Should_ConvertFromPrimitive()
        {
            TId id = ConvertFromPrimitive(SampleValue);
            TPrimitive primitive = ConvertToPrimitive(id);
            Assert.Equal(SampleValue, primitive);
        }

        [Fact]
        public void Equality_Should_BeTrue_ForSameValue()
        {
            TId id1 = ConvertFromPrimitive(SampleValue);
            TId id2 = ConvertFromPrimitive(SampleValue);
            Assert.Equal(id1, id2);
        }

        [Fact]
        public void Equality_Should_BeFalse_ForDifferentValues()
        {
            TId id1 = ConvertFromPrimitive(SampleValue);
            TId id2 = ConvertFromPrimitive(EmptyValue);
            Assert.NotEqual(id1, id2);
        }

        [Fact]
        public void DefaultValue_Should_BeHandledCorrectly()
        {
            TId defaultId = default;
            Assert.Equal(ConvertFromPrimitive(EmptyValue), defaultId);
        }
        
        /*
        [Fact]
        public void NullString_Should_BeHandledGracefully()
        {
            TId id = ConvertFromPrimitive(default);
            Assert.Equal(ConvertFromPrimitive(null), id);
        }
        */
        
        [Fact]
        public void HashCode_Should_BeConsistent()
        {
            TId id1 = ConvertFromPrimitive(SampleValue);
            TId id2 = ConvertFromPrimitive(SampleValue);
            Assert.Equal(id1.GetHashCode(), id2.GetHashCode());
        }

        [Fact]
        public void HashCode_Should_Differ_ForDifferentValues()
        {
            TId id1 = ConvertFromPrimitive(SampleValue);
            TId id2 = ConvertFromPrimitive(EmptyValue);
            Assert.NotEqual(id1.GetHashCode(), id2.GetHashCode());
        }
        
        /*
        [Fact]
        public void CaseSensitivity_Should_BeRespected()
        {
            if (SampleValue is string strValue)
            {
                TId lower = ConvertFromPrimitive(strValue.ToLower());
                TId upper = ConvertFromPrimitive(strValue.ToUpper());
                Assert.NotEqual(lower, upper);
            }
        }
        */
        
        [Fact]
        public void SystemTextJson_Should_Throw_When_DataIsMalformed()
        {
            string invalidJson = "{ \"Value\": 12345 }"; // Expecting a string but got an integer
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<TId>(invalidJson));
        }
        
        [Fact]
        public void Serialization_Should_BeFast()
        {
            TId id = ConvertFromPrimitive(SampleValue);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            for (int i = 0; i < 100000; i++)
            {
                string json = JsonSerializer.Serialize(id);
                _ = JsonSerializer.Deserialize<TId>(json);
            }

            stopwatch.Stop();
            Assert.True(stopwatch.ElapsedMilliseconds < 2000, "Serialization too slow.");
        }
        
        
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
}