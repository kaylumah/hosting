// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using Ssg.Extensions.Data.Yaml;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    public static class YamlParserExtensions
    {
        public static T Parse<T>(this IYamlParser yamlParser, System.IO.Abstractions.IFileSystemInfo file)
        {
            try
            {
                string raw = file.ReadFile();
                T result = yamlParser.Parse<T>(raw);
                return result;
            }
            catch (Exception ex)
            {
                Debug.Assert(ex != null);
                throw;
            }
        }
    }
}
