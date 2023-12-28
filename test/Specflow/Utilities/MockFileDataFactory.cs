// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using Test.Specflow.Helpers;

namespace Test.Specflow.Utilities
{
    public class MockFileDataFactory
    {
        Encoding _Encoding;
        string _FrontMatter;
        string _Contents;

        public MockFileDataFactory WithContents(string contents)
        {
            _Contents = contents;
            return this;
        }

        public MockFileDataFactory WithYamlFrontMatter(Dictionary<string, object> data = null)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("---");
            if (data != null && data.Any())
            {
                YamlDotNet.Serialization.ISerializer serializer = YamlSerializer.Create();
                string raw = serializer.Serialize(data);
                stringBuilder.Append(raw);
            }

            stringBuilder.AppendLine("---");
            _FrontMatter = stringBuilder.ToString();
            return this;
        }

        public MockFileDataFactory WithUtf8Encoding() => WithEncoding(new UTF8Encoding(false));
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
            byte[] bytes = _Encoding.GetBytes(data);
            return new MockFileData(bytes);
        }

        public static MockFileData PlainFile(string contents)
        {
            return new MockFileDataFactory()
                .WithUtf8Encoding()
                .WithContents(contents)
                .Create();
        }

        public static MockFileData EnrichedFile(string contents, Dictionary<string, object> data = null)
        {
            return new MockFileDataFactory()
                .WithUtf8Encoding()
                .WithContents(contents)
                .WithYamlFrontMatter(data)
                .Create();
        }

        public static MockFileData EmptyFile()
        {
            return new MockFileDataFactory()
                .WithUtf8Encoding()
                .Create();
        }
    }
}
