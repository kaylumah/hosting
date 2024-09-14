// Copyright (c) Kaylumah, 2024. All rights reserved.
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
    public sealed class OrganizationIdJsonConverter : JsonConverter<OrganizationId>
    {
        public override OrganizationId Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            string? value = reader.GetString()!;
            OrganizationId result = new OrganizationId(value);
            return result;
        }

        public override void Write(Utf8JsonWriter writer, OrganizationId value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }

        public override void WriteAsPropertyName(
            Utf8JsonWriter writer,
            OrganizationId value,
            JsonSerializerOptions options)
        {
            writer.WritePropertyName(value);
        }

        public override OrganizationId ReadAsPropertyName(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            OrganizationId value = Read(ref reader, typeToConvert, options);
            return value;
        }
    }

    public readonly struct OrganizationId
    {
        readonly string _OrganizationId;

        public OrganizationId(string organizationId)
        {
            _OrganizationId = organizationId;
        }

        public static implicit operator string(OrganizationId organization) => organization._OrganizationId;
        public static implicit operator OrganizationId(string value) => new OrganizationId(value);
    }

    [DebuggerDisplay("OrganizationMetaData '{FullName}'")]
    public class OrganizationMetaData
    {
        public OrganizationId Id
        { get; set; } = null!;
        public string FullName
        { get; set; } = null!;
        public string Linkedin
        { get; set; } = null!;
        public string Twitter
        { get; set; } = null!;
        public string Logo
        { get; set; } = null!;
        public DateTimeOffset Founded
        { get; set; }
    }

    public class OrganizationMetaDataCollection : KeyedCollection<OrganizationId, OrganizationMetaData>
    {
        protected override OrganizationId GetKeyForItem(OrganizationMetaData item)
        {
            return item.Id;
        }

        public new IDictionary<OrganizationId, OrganizationMetaData> Dictionary => base.Dictionary!;

        public IEnumerable<OrganizationId> Keys => base.Dictionary?.Keys ?? Enumerable.Empty<OrganizationId>();

    }
}