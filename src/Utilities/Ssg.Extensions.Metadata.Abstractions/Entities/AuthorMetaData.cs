// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ssg.Extensions.Metadata.Abstractions
{
    #pragma warning disable
    public class AuthorIdConverter : JsonConverter<AuthorId>
    {
        public override AuthorId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new AuthorId(reader.GetString()!);
        }

        public override void Write(Utf8JsonWriter writer, AuthorId value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Value);
        }

        public override AuthorId ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new AuthorId(reader.GetString()!);
        }

        public override void WriteAsPropertyName(Utf8JsonWriter writer, AuthorId value, JsonSerializerOptions options)
        {
            writer.WritePropertyName(value.Value);
        }
    }
    
    public class StringValueRecordStructConverter<T> : JsonConverter<T> where T : struct
    {
        private readonly Func<string, T> _fromString;
        private readonly Func<T, string> _toString;

        public StringValueRecordStructConverter(Func<string, T> fromString, Func<T, string> toString)
        {
            _fromString = fromString;
            _toString = toString;
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return _fromString(reader.GetString()!);
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(_toString(value));
        }

        public override T ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return _fromString(reader.GetString()!);
        }

        public override void WriteAsPropertyName(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WritePropertyName(_toString(value));
        }
    }

    public class IdRecordStructConverter<T> : JsonConverter<T> where T : struct
    {
        private readonly Func<object, T> _fromObject;
        private readonly Func<T, object> _toObject;
        private readonly Type _underlyingType;

        public IdRecordStructConverter()
        {
            var type = typeof(T);
            _underlyingType = GetUnderlyingType(type);

            // Find implicit conversion operators
            var fromMethod = type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(m =>
                    m.IsSpecialName && m.Name == "op_Implicit" &&
                    m.ReturnType == type &&
                    m.GetParameters().Length == 1 &&
                    m.GetParameters()[0].ParameterType == _underlyingType);

            var toMethod = type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(m =>
                    m.IsSpecialName && m.Name == "op_Implicit" &&
                    m.ReturnType == _underlyingType &&
                    m.GetParameters().Length == 1 &&
                    m.GetParameters()[0].ParameterType == type);

            if (fromMethod == null || toMethod == null)
            {
                throw new InvalidOperationException($"Type {type.Name} must have implicit conversions to and from {_underlyingType}.");
            }

            _fromObject = (object value) => (T)fromMethod.Invoke(null, new object[] { value })!;
            _toObject = (T value) => toMethod.Invoke(null, new object[] { value })!;
        }

        private static Type GetUnderlyingType(Type type)
        {
            // Check the constructor's parameter type to determine what type is stored
            var constructor = type.GetConstructors().FirstOrDefault();
            if (constructor != null)
            {
                var param = constructor.GetParameters().FirstOrDefault();
                if (param != null)
                {
                    return param.ParameterType; // string, int, or Guid
                }
            }

            throw new InvalidOperationException($"Could not determine the underlying type for {type.Name}.");
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            object value = _underlyingType switch
            {
                Type t when t == typeof(string) => reader.GetString()!,
                Type t when t == typeof(Guid) => reader.GetGuid(),
                Type t when t == typeof(int) => reader.GetInt32(),
                _ => throw new JsonException($"Unsupported ID type {_underlyingType}.")
            };

            return _fromObject(value);
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            object objValue = _toObject(value);

            if (objValue is string strVal)
                writer.WriteStringValue(strVal);
            else if (objValue is Guid guidVal)
                writer.WriteStringValue(guidVal);
            else if (objValue is int intVal)
                writer.WriteNumberValue(intVal);
            else
                throw new JsonException($"Unsupported ID type {_underlyingType}.");
        }

        public override T ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return _fromObject(reader.GetString()!);
        }

        public override void WriteAsPropertyName(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WritePropertyName(_toObject(value).ToString()!);
        }
    }


    public readonly record struct AuthorId(string Value)
    {
        public static implicit operator string(AuthorId authorId) => authorId.Value;
        public static implicit operator AuthorId(string value) => new(value);
    }

    [DebuggerDisplay("AuthorMetaData '{FullName}'")]
    public class AuthorMetaData
    {
        public AuthorId Id
        { get; set; } = null!;
        public string FullName
        { get; set; } = null!;
        public string Email
        { get; set; } = null!;
        public string Uri
        { get; set; } = null!;
        public string Picture
        { get; set; } = null!;
        public Links Links
        { get; set; } = new();
    }

    public class AuthorMetaDataCollection : KeyedCollection<AuthorId, AuthorMetaData>
    {
        protected override AuthorId GetKeyForItem(AuthorMetaData item)
        {
            return item.Id;
        }

        public new IDictionary<AuthorId, AuthorMetaData> Dictionary => base.Dictionary!;

        public IEnumerable<AuthorId> Keys => base.Dictionary?.Keys ?? Enumerable.Empty<AuthorId>();

    }
}