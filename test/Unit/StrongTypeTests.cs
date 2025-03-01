// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
    /*
     * [Fact]
       public void SystemTextJson_Should_SerializeAndDeserializeList()
       {
           var list = new List<TStrongTypedId> { ConvertFromPrimitive(SampleValue) };

           string json = JsonSerializer.Serialize(list);
           List<TStrongTypedId> deserialized = JsonSerializer.Deserialize<List<TStrongTypedId>>(json)!;

           Assert.Equal(list, deserialized);
       }

       [Fact]
       public void SystemTextJson_Should_SerializeAndDeserializeArray()
       {
           var array = new TStrongTypedId[] { ConvertFromPrimitive(SampleValue) };

           string json = JsonSerializer.Serialize(array);
           TStrongTypedId[] deserialized = JsonSerializer.Deserialize<TStrongTypedId[]>(json)!;

           Assert.Equal(array, deserialized);
       }

       [Fact]
       public void SystemTextJson_Should_SerializeAndDeserializeNestedType()
       {
           var obj = new ComplexType<TStrongTypedId>
           {
               Id = ConvertFromPrimitive(SampleValue),
               Name = "Test"
           };

           string json = JsonSerializer.Serialize(obj);
           ComplexType<TStrongTypedId> deserialized = JsonSerializer.Deserialize<ComplexType<TStrongTypedId>>(json)!;

           Assert.Equal(obj.Id, deserialized.Id);
           Assert.Equal(obj.Name, deserialized.Name);
       }

       [Fact]
       public void SystemTextJson_Should_SerializeAndDeserializeDeeplyNestedDictionary()
       {
           var dict = new Dictionary<TStrongTypedId, ComplexType<TStrongTypedId>>
           {
               { ConvertFromPrimitive(SampleValue), new ComplexType<TStrongTypedId> { Id = ConvertFromPrimitive(SampleValue), Name = "Nested Test" } }
           };

           var options = new JsonSerializerOptions();
           StronglyTypedIdConverterRegistrar.RegisterAllIdConverters(options);

           string json = JsonSerializer.Serialize(dict, options);
           Dictionary<TStrongTypedId, ComplexType<TStrongTypedId>> deserialized = JsonSerializer.Deserialize<Dictionary<TStrongTypedId, ComplexType<TStrongTypedId>>>(json, options)!;

           Assert.Equal(dict.Keys, deserialized.Keys);
           Assert.Equal(dict.Values.First().Id, deserialized.Values.First().Id);
           Assert.Equal(dict.Values.First().Name, deserialized.Values.First().Name);
       }

       [Fact]
       public void YamlDotNet_Should_SerializeAndDeserializeList()
       {
           var list = new List<TStrongTypedId> { ConvertFromPrimitive(SampleValue) };
           var serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
           var deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();

           string yaml = serializer.Serialize(list);
           List<TStrongTypedId> deserialized = deserializer.Deserialize<List<TStrongTypedId>>(yaml)!;

           Assert.Equal(list, deserialized);
       }

       [Fact]
       public void YamlDotNet_Should_SerializeAndDeserializeNestedType()
       {
           var obj = new ComplexType<TStrongTypedId> { Id = ConvertFromPrimitive(SampleValue), Name = "Test" };
           var serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
           var deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();

           string yaml = serializer.Serialize(obj);
           ComplexType<TStrongTypedId> deserialized = deserializer.Deserialize<ComplexType<TStrongTypedId>>(yaml)!;

           Assert.Equal(obj.Id, deserialized.Id);
           Assert.Equal(obj.Name, deserialized.Name);
       }

       [Fact]
       public void DataContractSerializer_Should_SerializeAndDeserializeList()
       {
           var list = new List<TStrongTypedId> { ConvertFromPrimitive(SampleValue) };
           var serializer = new DataContractSerializer(typeof(List<TStrongTypedId>));

           using MemoryStream memoryStream = new MemoryStream();
           serializer.WriteObject(memoryStream, list);
           memoryStream.Position = 0;

           List<TStrongTypedId> deserialized = (List<TStrongTypedId>)serializer.ReadObject(memoryStream)!;
           Assert.Equal(list, deserialized);
       }

       [Fact]
       public void DataContractSerializer_Should_SerializeAndDeserializeNestedType()
       {
           var obj = new ComplexType<TStrongTypedId> { Id = ConvertFromPrimitive(SampleValue), Name = "Test" };
           var serializer = new DataContractSerializer(typeof(ComplexType<TStrongTypedId>));

           using MemoryStream memoryStream = new MemoryStream();
           serializer.WriteObject(memoryStream, obj);
           memoryStream.Position = 0;

           ComplexType<TStrongTypedId> deserialized = (ComplexType<TStrongTypedId>)serializer.ReadObject(memoryStream)!;
           Assert.Equal(obj.Id, deserialized.Id);
           Assert.Equal(obj.Name, deserialized.Name);
       }
       
       [DataContract]
       public class ComplexType<TStrongTypedId>
       {
           [DataMember]
           public TStrongTypedId Id { get; set; }
       
           [DataMember]
           public string Name { get; set; }
       }
       
       [DataContract]
       public class StronglyTypedDto<TStrongTypedId>
       {
           [DataMember]
           public TStrongTypedId Id { get; set; }
       
           [DataMember]
           public string Name { get; set; }
       
           [DataMember]
           public DateTime CreatedAt { get; set; }
       
           public override bool Equals(object? obj)
           {
               return obj is StronglyTypedDto<TStrongTypedId> dto &&
                      EqualityComparer<TStrongTypedId>.Default.Equals(Id, dto.Id) &&
                      Name == dto.Name &&
                      CreatedAt == dto.CreatedAt;
           }
       
           public override int GetHashCode()
           {
               return HashCode.Combine(Id, Name, CreatedAt);
           }
       }
       
       [Fact]
       public void SystemTextJson_Should_SerializeAndDeserialize_Dto()
       {
           var dto = new StronglyTypedDto<TStrongTypedId>
           {
               Id = ConvertFromPrimitive(SampleValue),
               Name = "Test DTO",
               CreatedAt = DateTime.UtcNow
           };
       
           var options = new JsonSerializerOptions();
           StronglyTypedIdConverterRegistrar.RegisterAllIdConverters(options);
       
           string json = JsonSerializer.Serialize(dto, options);
           StronglyTypedDto<TStrongTypedId> deserialized = JsonSerializer.Deserialize<StronglyTypedDto<TStrongTypedId>>(json, options)!;
       
           Assert.Equal(dto, deserialized);
       }
       
       [Fact]
       public void YamlDotNet_Should_SerializeAndDeserialize_Dto()
       {
           var dto = new StronglyTypedDto<TStrongTypedId>
           {
               Id = ConvertFromPrimitive(SampleValue),
               Name = "Test DTO",
               CreatedAt = DateTime.UtcNow
           };
       
           var serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
           var deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
       
           string yaml = serializer.Serialize(dto);
           StronglyTypedDto<TStrongTypedId> deserialized = deserializer.Deserialize<StronglyTypedDto<TStrongTypedId>>(yaml)!;
       
           Assert.Equal(dto, deserialized);
       }
       
       [Fact]
       public void DataContractSerializer_Should_SerializeAndDeserialize_Dto()
       {
           var dto = new StronglyTypedDto<TStrongTypedId>
           {
               Id = ConvertFromPrimitive(SampleValue),
               Name = "Test DTO",
               CreatedAt = DateTime.UtcNow
           };
       
           var serializer = new DataContractSerializer(typeof(StronglyTypedDto<TStrongTypedId>));
       
           using MemoryStream memoryStream = new MemoryStream();
           serializer.WriteObject(memoryStream, dto);
           memoryStream.Position = 0;
       
           StronglyTypedDto<TStrongTypedId> deserialized = (StronglyTypedDto<TStrongTypedId>)serializer.ReadObject(memoryStream)!;
       
           Assert.Equal(dto, deserialized);
       }
       
       [DataContract]
       public class NestedDto<TStrongTypedId>
       {
           [DataMember]
           public StronglyTypedDto<TStrongTypedId> Parent { get; set; }
       
           [DataMember]
           public List<StronglyTypedDto<TStrongTypedId>> Children { get; set; } = new();
       }
       
       [Fact]
       public void SystemTextJson_Should_SerializeAndDeserialize_NestedDto()
       {
           var parent = new StronglyTypedDto<TStrongTypedId>
           {
               Id = ConvertFromPrimitive(SampleValue),
               Name = "Parent",
               CreatedAt = DateTime.UtcNow
           };
       
           var child = new StronglyTypedDto<TStrongTypedId>
           {
               Id = ConvertFromPrimitive(EmptyValue),
               Name = "Child",
               CreatedAt = DateTime.UtcNow
           };
       
           var nestedDto = new NestedDto<TStrongTypedId> { Parent = parent, Children = new List<StronglyTypedDto<TStrongTypedId>> { child } };
       
           var options = new JsonSerializerOptions();
           StronglyTypedIdConverterRegistrar.RegisterAllIdConverters(options);
       
           string json = JsonSerializer.Serialize(nestedDto, options);
           NestedDto<TStrongTypedId> deserialized = JsonSerializer.Deserialize<NestedDto<TStrongTypedId>>(json, options)!;
       
           Assert.Equal(nestedDto.Parent, deserialized.Parent);
           Assert.Equal(nestedDto.Children, deserialized.Children);
       }
       
       [DataContract]
       public class StronglyTypedDto<TStrongTypedId>
       {
           [DataMember]
           public TStrongTypedId Id { get; set; }
       
           [DataMember]
           public string Name { get; set; }
       
           [DataMember]
           public DateTime CreatedAt { get; set; }
       
           [DataMember]
           public StronglyTypedDto<TStrongTypedId>? Parent { get; set; }  // Nullable Parent
       
           [DataMember]
           public List<StronglyTypedDto<TStrongTypedId>>? Children { get; set; } // Nullable Children
       
           public override bool Equals(object? obj)
           {
               return obj is StronglyTypedDto<TStrongTypedId> dto &&
                      EqualityComparer<TStrongTypedId>.Default.Equals(Id, dto.Id) &&
                      Name == dto.Name &&
                      CreatedAt == dto.CreatedAt &&
                      EqualityComparer<StronglyTypedDto<TStrongTypedId>?>.Default.Equals(Parent, dto.Parent) &&
                      EqualityComparer<List<StronglyTypedDto<TStrongTypedId>>?>.Default.Equals(Children, dto.Children);
           }
       
           public override int GetHashCode()
           {
               return HashCode.Combine(Id, Name, CreatedAt, Parent, Children);
           }
       }
       
       [Fact]
       public void SystemTextJson_Should_SerializeAndDeserialize_Dto()
       {
           var dto = new StronglyTypedDto<TStrongTypedId>
           {
               Id = ConvertFromPrimitive(SampleValue),
               Name = "Test DTO",
               CreatedAt = DateTime.UtcNow
           };
       
           var options = new JsonSerializerOptions();
           StronglyTypedIdConverterRegistrar.RegisterAllIdConverters(options);
       
           string json = JsonSerializer.Serialize(dto, options);
           StronglyTypedDto<TStrongTypedId> deserialized = JsonSerializer.Deserialize<StronglyTypedDto<TStrongTypedId>>(json, options)!;
       
           Assert.Equal(dto, deserialized);
       }
       
       [Fact]
       public void SystemTextJson_Should_SerializeAndDeserialize_NestedDto()
       {
           var parent = new StronglyTypedDto<TStrongTypedId>
           {
               Id = ConvertFromPrimitive(SampleValue),
               Name = "Parent",
               CreatedAt = DateTime.UtcNow
           };
       
           var child = new StronglyTypedDto<TStrongTypedId>
           {
               Id = ConvertFromPrimitive(EmptyValue),
               Name = "Child",
               CreatedAt = DateTime.UtcNow,
               Parent = parent
           };
       
           parent.Children = new List<StronglyTypedDto<TStrongTypedId>> { child };
       
           var options = new JsonSerializerOptions();
           StronglyTypedIdConverterRegistrar.RegisterAllIdConverters(options);
       
           string json = JsonSerializer.Serialize(parent, options);
           StronglyTypedDto<TStrongTypedId> deserialized = JsonSerializer.Deserialize<StronglyTypedDto<TStrongTypedId>>(json, options)!;
       
           Assert.Equal(parent.Id, deserialized.Id);
           Assert.Equal(parent.Name, deserialized.Name);
           Assert.Equal(parent.CreatedAt, deserialized.CreatedAt);
           Assert.Equal(parent.Children?.First().Id, deserialized.Children?.First().Id);
           Assert.Equal(parent.Children?.First().Parent?.Id, deserialized.Children?.First().Parent?.Id);
       }
     */

    public abstract class StronglyTypedIdTests<TStrongTypedId, TPrimitive>
        where TStrongTypedId : struct
    {
        protected const string Json = "json"; //"SystemTextJson";
        
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

        protected string Serialize<T>(T value, string format) => format switch
        {
            "json" => SerializeJson(value),
            "yaml" => SerializeYaml(value),
            "xml" => SerializeXml(value),
            _ => throw new ArgumentException("Invalid format", nameof(format))
        };
        
        protected T Deserialize<T>(string serialized, string format) => format switch
        {
            "json" => DeserializeJson<T>(serialized),
            "yaml" => DeserializeYaml<T>(serialized),
            "xml" => DeserializeXml<T>(serialized),
            _ => throw new ArgumentException("Invalid format", nameof(format))
        };

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
        // [MemberData(nameof(SingleValueTestData))]
        [InlineData(Json)]
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
        
        [Fact]
        public void SystemTextJson_Should_SerializeAndDeserialize_SingleValue()
        {
            string originalValueAsString = SampleValue?.ToString() ?? string.Empty;
            TStrongTypedId id = ConvertFromPrimitive(SampleValue);

            string json = SerializeJson(id);
            Assert.Contains(originalValueAsString, json);
            TStrongTypedId deserialized = DeserializeJson<TStrongTypedId>(json);
            Assert.NotNull(deserialized);
            Assert.Equal(id, deserialized);
        }

        [Fact]
        public void YamlDotNet_Should_SerializeAndDeserialize_SingleValue()
        {
            string originalValueAsString = SampleValue?.ToString() ?? string.Empty;
            TStrongTypedId id = ConvertFromPrimitive(SampleValue);

            string yaml = SerializeYaml(id);
            Assert.Contains(originalValueAsString, yaml);
            TStrongTypedId deserialized = DeserializeYaml<TStrongTypedId>(yaml);
            Assert.NotNull(deserialized);
            Assert.Equal(id, deserialized);
        }

        [Fact]
        public void DataContractSerializer_Should_SerializeAndDeserialize_SingleValue()
        {
            string originalValueAsString = SampleValue?.ToString() ?? string.Empty;
            TStrongTypedId id = ConvertFromPrimitive(SampleValue);

            string xml = SerializeXml(id);
            Assert.Contains(originalValueAsString, xml);
            TStrongTypedId deserialized = DeserializeXml<TStrongTypedId>(xml);
            Assert.NotNull(deserialized);
            Assert.Equal(id, deserialized);
        }

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
        
        
        [Theory(Skip = "high?")]
        // [MemberData(nameof(SingleValueTestData))]
        [InlineData(Json)]
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