// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.IO.Abstractions.TestingHelpers;
using Ssg.Extensions.Metadata.Abstractions;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Test.Unit.Helpers
{
    public static class YamlSerializer
    {
        public class CustomDateTimeYamlTypeConverter : IYamlTypeConverter
        {
            public bool Accepts(Type type) => type == typeof(DateTimeOffset);

            public object ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
            {
                throw new NotImplementedException();
            }

            public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
            {
                DateTimeOffset? dateTime = (DateTimeOffset?)value;
                string? str = dateTime?.ToString("o", CultureInfo.InvariantCulture);
                emitter.Emit((ParsingEvent)new Scalar(AnchorName.Empty, TagName.Empty, str!, ScalarStyle.Any, true, false));
            }
        }
        internal sealed class AuthorIdYamlTypeConverter : IYamlTypeConverter
        {
            static readonly Type _ContentCategoryNodeType;

            static AuthorIdYamlTypeConverter()
            {
                _ContentCategoryNodeType = typeof(AuthorId);
            }

            public bool Accepts(Type type)
            {
                return type == _ContentCategoryNodeType;
            }

            public object ReadYaml(IParser parser, Type type, ObjectDeserializer serializer)
            {
                throw new NotImplementedException();
            }

            public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
            {
                AuthorId? node = (AuthorId?)value;
                string? str = node;
                Scalar? scalar = new Scalar(AnchorName.Empty, TagName.Empty, str!, ScalarStyle.Any, true, false);
                emitter.Emit(scalar);
            }
        }
        internal sealed class OrganizationIdYamlTypeConverter : IYamlTypeConverter
        {
            static readonly Type _ContentCategoryNodeType;

            static OrganizationIdYamlTypeConverter()
            {
                _ContentCategoryNodeType = typeof(OrganizationId);
            }

            public bool Accepts(Type type)
            {
                return type == _ContentCategoryNodeType;
            }

            public object ReadYaml(IParser parser, Type type, ObjectDeserializer serializer)
            {
                throw new NotImplementedException();
            }

            public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
            {
                OrganizationId? node = (OrganizationId?)value;
                string? str = node;
                Scalar scalar = new Scalar(AnchorName.Empty, TagName.Empty, str!, ScalarStyle.Any, true, false);
                emitter.Emit(scalar);
            }
        }
        public static ISerializer Create()
        {
            CustomDateTimeYamlTypeConverter timeConverter = new CustomDateTimeYamlTypeConverter();
            AuthorIdYamlTypeConverter authorIdYamlTypeConverter = new AuthorIdYamlTypeConverter();
            OrganizationIdYamlTypeConverter organizationIdYamlTypeConverter = new OrganizationIdYamlTypeConverter();
            ISerializer serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull | DefaultValuesHandling.OmitDefaults)
                .WithTypeConverter(timeConverter)
                .WithTypeConverter(authorIdYamlTypeConverter)
                .WithTypeConverter(organizationIdYamlTypeConverter)
                .Build();
            return serializer;
        }
    }
}
