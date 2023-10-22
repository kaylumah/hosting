// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Ssg.Extensions.Data.Yaml
{
    public class YamlParser : IYamlParser
    {
        readonly IDeserializer _deserializer;

        public YamlParser()
        {
            _deserializer = new DeserializerBuilder()
                .IgnoreUnmatchedProperties()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
               .Build();
        }

        public T Parse<T>(string raw)
        {
            T result = _deserializer.Deserialize<T>(raw);
            return result;
        }
    }
}
