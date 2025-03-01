// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    public class StronglyTypedIdJsonConverter<T> : JsonConverter<T> where T : struct
    {
        readonly StronglyTypedIdHelper<T> _StronglyTypedIdHelper;

        public StronglyTypedIdJsonConverter()
        {
            _StronglyTypedIdHelper = new StronglyTypedIdHelper<T>();
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Type targetType = _StronglyTypedIdHelper.UnderlyingType;
            object value = targetType switch
            {
                _ when targetType == typeof(string) => reader.GetString() ?? string.Empty,
                _ when targetType == typeof(Guid) => reader.GetGuid(),
                _ when targetType == typeof(int) => reader.GetInt32(),
                _ => throw new JsonException($"Unsupported ID type {targetType}.")
            };

            T result = _StronglyTypedIdHelper.FromObject(value);
            return result;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            Type targetType = _StronglyTypedIdHelper.UnderlyingType;
            object objValue = _StronglyTypedIdHelper.ToObject(value);

            if (objValue is null)
            {
                throw new ArgumentNullException(nameof(value), "Cannot serialize a null strongly-typed ID.");
            }

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
            string? input = reader.GetString();
            if (string.IsNullOrEmpty(input))
            {
                throw new JsonException("Property name cannot be null or empty.");
            }

            Type targetType = _StronglyTypedIdHelper.UnderlyingType;
            object? converted = input.ConvertValue(targetType);
            Debug.Assert(converted != null);

            T result = _StronglyTypedIdHelper.FromObject(converted);
            return result;
        }

        public override void WriteAsPropertyName(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            object x = _StronglyTypedIdHelper.ToObject(value);
            string xAsString = x.ToString()!;
            writer.WritePropertyName(xAsString);
        }
    }
}