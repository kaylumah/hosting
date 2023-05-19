// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using Ssg.Extensions.Data.Yaml;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    public static class YamlParserExtensions
    {
        public static T Parse<T>(this IYamlParser yamlParser, System.IO.Abstractions.IFileSystemInfo file)
        {
            var raw = file.ReadFile();
            return yamlParser.Parse<T>(raw);
        }
    }
}
