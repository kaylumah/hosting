﻿// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using Kaylumah.Ssg.iFX.Data.Csv;
using Kaylumah.Ssg.iFX.Data.Json;
using Kaylumah.Ssg.iFX.Data.Yaml;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    public static class ParserExtensions
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

        public static T Parse<T>(this IJsonParser jsonParser, System.IO.Abstractions.IFileSystemInfo file)
        {
            try
            {
                string raw = file.ReadFile();
                T result = jsonParser.Parse<T>(raw);
                return result;
            }
            catch (Exception ex)
            {
                Debug.Assert(ex != null);
                throw;
            }
        }

        public static T[] Parse<T>(this ICsvParser csvParser, System.IO.Abstractions.IFileSystemInfo file)
        {
            try
            {
                string raw = file.ReadFile();
                T[] result = csvParser.Parse<T>(raw);
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
