// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
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