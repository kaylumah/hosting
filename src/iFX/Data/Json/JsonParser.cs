// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Text.Json;
using Kaylumah.Ssg.iFX.Data.Abstractions;

namespace Kaylumah.Ssg.iFX.Data.Json
{
    public class JsonParser : IJsonParser
    {
        T IParser.Parse<T>(string raw)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(raw);
            // JsonNode? result = JsonSerializer.Deserialize<JsonNode>(raw);
            T? result = JsonSerializer.Deserialize<T>(raw);
            Debug.Assert(result != null);
            return result;
        }
    }
}