// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using Kaylumah.Ssg.Extensions.Data.Abstractions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Kaylumah.Ssg.Extensions.Data.Yaml
{
    public class YamlParser : IYamlParser
    {
        readonly IDeserializer _Deserializer;

        public YamlParser()
        {
            _Deserializer = new DeserializerBuilder()
                .IgnoreUnmatchedProperties()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
               .Build();
        }

        T IParser.Parse<T>(string raw)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(raw);
            T result = _Deserializer.Deserialize<T>(raw);
            return result;
        }
    }
}
