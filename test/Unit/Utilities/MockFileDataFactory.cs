﻿// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using Test.Unit.Helpers;

namespace Test.Unit.Utilities
{
    public class MockFileDataFactory
    {
        Encoding? _Encoding;
        string? _FrontMatter;
        string? _Contents;

        static UTF8Encoding UTF8Encoding;

        static MockFileDataFactory()
        {
            UTF8Encoding = new UTF8Encoding(false);
        }

        public MockFileDataFactory WithContents(string contents)
        {
            _Contents = contents;
            return this;
        }

        public MockFileDataFactory WithYamlFrontMatter(Dictionary<string, object?>? data = null)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("---");
            if (data != null && data.Count != 0)
            {
                YamlDotNet.Serialization.ISerializer serializer = YamlSerializer.Create();
                string raw = serializer.Serialize(data);
                stringBuilder.Append(raw);
            }

            stringBuilder.AppendLine("---");
            _FrontMatter = stringBuilder.ToString();
            return this;
        }

        public MockFileDataFactory WithUtf8Encoding() => WithEncoding(UTF8Encoding);
        MockFileDataFactory WithEncoding(Encoding encoding)
        {
            _Encoding = encoding;
            return this;
        }

        public MockFileData Create()
        {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(_FrontMatter))
            {
                sb.Append(_FrontMatter);
            }

            if (!string.IsNullOrEmpty(_Contents))
            {
                sb.Append(_Contents);
            }

            string data = sb.ToString();
            byte[]? bytes = _Encoding?.GetBytes(data);
            MockFileData result = new MockFileData(bytes);
            return result;
        }

        public static MockFileData PlainFile(string contents)
        {
            MockFileData result = new MockFileDataFactory()
                .WithUtf8Encoding()
                .WithContents(contents)
                .Create();
            return result;
        }

        public static MockFileData EnrichedFile(string contents, Dictionary<string, object?>? data = null)
        {
            MockFileData result = new MockFileDataFactory()
                .WithUtf8Encoding()
                .WithContents(contents)
                .WithYamlFrontMatter(data)
                .Create();
            return result;
        }

        public static MockFileData EmptyFile()
        {
            MockFileData result = 
             new MockFileDataFactory()
                .WithUtf8Encoding()
                .Create();
            return result;
        }
    }
}
