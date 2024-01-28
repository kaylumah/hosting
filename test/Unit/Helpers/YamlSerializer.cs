// Copyright (c) Kaylumah, 2024. All rights reserved.
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

            public object ReadYaml(IParser parser, Type type)
            {
                throw new NotImplementedException();
            }

            public void WriteYaml(IEmitter emitter, object value, Type type)
            {
                DateTimeOffset dateTime = (DateTimeOffset)value;
                string str = dateTime.ToString("o", CultureInfo.InvariantCulture);
                emitter.Emit((ParsingEvent)new Scalar(AnchorName.Empty, TagName.Empty, str, ScalarStyle.Any, true, false));
            }
        }
        internal sealed class AuthorIdYamlTypeConverter : IYamlTypeConverter
        {
            static readonly Type _ContentCategoryNodeType = typeof(AuthorId);

            public bool Accepts(Type type)
            {
                return type == _ContentCategoryNodeType;
            }

            public object ReadYaml(IParser parser, Type type)
            {
                throw new NotImplementedException();
            }

            public void WriteYaml(IEmitter emitter, object value, Type type)
            {
                AuthorId node = (AuthorId)value;
                string str = node;
                emitter.Emit(new Scalar(AnchorName.Empty, TagName.Empty, str, ScalarStyle.Any, true, false));
            }
        }
        internal sealed class OrganizationIdYamlTypeConverter : IYamlTypeConverter
        {
            static readonly Type _ContentCategoryNodeType = typeof(OrganizationId);

            public bool Accepts(Type type)
            {
                return type == _ContentCategoryNodeType;
            }

            public object ReadYaml(IParser parser, Type type)
            {
                throw new NotImplementedException();
            }

            public void WriteYaml(IEmitter emitter, object value, Type type)
            {
                OrganizationId node = (OrganizationId)value;
                string str = node;
                emitter.Emit(new Scalar(AnchorName.Empty, TagName.Empty, str, ScalarStyle.Any, true, false));
            }
        }
        public static ISerializer Create()
        {
            ISerializer serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull | DefaultValuesHandling.OmitDefaults)
                .WithTypeConverter(new CustomDateTimeYamlTypeConverter())
                .WithTypeConverter(new AuthorIdYamlTypeConverter())
                .WithTypeConverter(new OrganizationIdYamlTypeConverter())
                .Build();
            return serializer;
        }
    }
}
