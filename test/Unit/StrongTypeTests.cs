// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;
using Xunit;
using Xunit.Abstractions;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
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

        const string Json = "json"; //"SystemTextJson";
        const string Yaml = "yaml"; //"YamlDotNet";
        const string Xml = "xml"; //"DataContract";
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

        string Serialize<T>(T value, string format)
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
        
        T Deserialize<T>(string serialized, string format) => format switch
        {
            Json => DeserializeJson<T>(serialized),
            Yaml => DeserializeYaml<T>(serialized),
            Xml => DeserializeXml<T>(serialized),
            _ => throw new ArgumentException("Invalid format", nameof(format))
        };

        static string SerializeXml<T>(T obj)
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

        static T DeserializeXml<T>(string xml)
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
            TypedIdRecordStructJsonConverter<TStrongTypedId> converter = new TypedIdRecordStructJsonConverter<TStrongTypedId>();
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
            TypedIdRecordStructJsonConverter<TStrongTypedId> converter = new TypedIdRecordStructJsonConverter<TStrongTypedId>();
            jsonOptions.Converters.Add(converter);
#pragma warning restore CA1869
            
            byte[] byteArray = new UTF8Encoding(false).GetBytes(json);
            using MemoryStream memoryStream = new MemoryStream(byteArray);
            using StreamReader reader = new StreamReader(memoryStream, Encoding.UTF8);

            string jsonString = reader.ReadToEnd();
            return JsonSerializer.Deserialize<T>(jsonString, jsonOptions)!;
        }

        static string SerializeYaml<T>(T obj)
        {
            StronglyTypedIdYamlConverter<TestStringId> converter = new StronglyTypedIdYamlConverter<TestStringId>();
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

        static T DeserializeYaml<T>(string yaml)
        {
            StronglyTypedIdYamlConverter<TestStringId> converter = new StronglyTypedIdYamlConverter<TestStringId>();
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

    public class TypedIdRecordStruct<T> where T : struct
    {
        public Func<object, T> FromObject
        { get; }

        public Func<T, object> ToObject
        { get; }
        
        public Type UnderlyingType
        { get; }
        
        public TypedIdRecordStruct()
        {
            Type strongIdType = typeof(T);
            UnderlyingType = GetUnderlyingType(strongIdType);

            MethodInfo[] methods = strongIdType.GetMethods(BindingFlags.Public | BindingFlags.Static);
            IEnumerable<MethodInfo> implicitOperatorMethods = methods.Where(IsImplicitOperator).ToList();

            MethodInfo? fromMethod = implicitOperatorMethods
                .SingleOrDefault(method => IsSpecificOperator(method, strongIdType, UnderlyingType));
            MethodInfo? toMethod = implicitOperatorMethods
                .SingleOrDefault(method => IsSpecificOperator(method, UnderlyingType, strongIdType));

            if (fromMethod == null || toMethod == null)
            {
                throw new InvalidOperationException($"Type {strongIdType.Name} must have implicit conversions to and from {UnderlyingType}.");
            }

            FromObject = (object value) =>
            {
                object[] arguments = new object[] { value };
                T result = (T)fromMethod.Invoke(null, arguments)!;
                return result;
            };
            
            ToObject = (T value) =>
            {
                object[] arguments = new object[] { value };
                object result = toMethod.Invoke(null, arguments)!;
                return result;
            };
        }
        
        static bool IsImplicitOperator(MethodInfo methodInfo)
        {
            bool result = methodInfo is { IsSpecialName: true, Name: "op_Implicit" };
            return result;
        }

        static bool IsSpecificOperator(MethodInfo methodInfo, Type returnType, Type parameterType)
        {
            bool returnTypeMatches = methodInfo.ReturnType == returnType;

            ParameterInfo[] parameterInfos = methodInfo.GetParameters();
            ParameterInfo parameterInfo = parameterInfos.Single();
            bool parameterMatches = parameterInfo.ParameterType == parameterType;

            bool result = returnTypeMatches && parameterMatches;
            return result;
        }
        
        static Type GetUnderlyingType(Type type)
        {
            ConstructorInfo[] constructors = type.GetConstructors();
            ConstructorInfo constructor = constructors.Single();

            ParameterInfo[] parameters = constructor.GetParameters();
            ParameterInfo parameterInfo = parameters.Single();

            Type result = parameterInfo.ParameterType;
            return result;
        }
    }
    
#pragma warning disable
    public class TypedIdRecordStructJsonConverter<T> : JsonConverter<T> where T : struct
    {
        readonly TypedIdRecordStruct<T> _Id;
        
        public TypedIdRecordStructJsonConverter()
        {
            _Id = new TypedIdRecordStruct<T>();
        }
        
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Type targetType = _Id.UnderlyingType;
            object value = targetType switch
            {
                { } t when t == typeof(string) => reader.GetString(),
                { } t when t == typeof(Guid) => reader.GetGuid(),
                { } t when t == typeof(int) => reader.GetInt32(),
                _ => throw new JsonException($"Unsupported ID type {targetType}.")
            };

            return _Id.FromObject(value);
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            Type targetType = _Id.UnderlyingType;
            object objValue = _Id.ToObject(value);

            switch (objValue)
            {
                case string strVal:
                writer.WriteStringValue(strVal);
                break;
                case Guid guidVal:
                writer.WriteStringValue(guidVal);
                break;
                case int intVal:
                writer.WriteNumberValue(intVal);
                break;
                default:
                throw new JsonException($"Unsupported ID type {targetType}.");
            }
        }

        public override T ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return _Id.FromObject(reader.GetString()!);
        }

        public override void WriteAsPropertyName(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WritePropertyName(_Id.ToObject(value).ToString()!);
        }
    }
    
    public class StronglyTypedIdYamlConverter<T> : IYamlTypeConverter where T : struct
    {
        readonly TypedIdRecordStruct<T> _Id;
        
        public StronglyTypedIdYamlConverter()
        {
            _Id = new TypedIdRecordStruct<T>();
        }
        
        bool IYamlTypeConverter.Accepts(Type type)
        {
            bool result = type == typeof(T);
            return result;
        }

        object? IYamlTypeConverter.ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
        {
            Scalar? scalar = (YamlDotNet.Core.Events.Scalar)parser.Current;
            parser.MoveNext();
            return _Id.FromObject(scalar.Value);
        }

        void IYamlTypeConverter.WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
        {
            emitter.Emit(new YamlDotNet.Core.Events.Scalar(_Id.ToObject((T)value).ToString()));
        }
    }
}