// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using Ssg.Extensions.Data.Abstractions;

namespace Ssg.Extensions.Data.Json
{
    public class JsonParser : IJsonParser
    {
        T IParser.Parse<T>(string raw)
        {
            throw new System.NotImplementedException();
        }
    }
}