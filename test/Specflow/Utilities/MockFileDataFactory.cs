// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.IO.Abstractions.TestingHelpers;
using System.Text;
using Test.Specflow.Helpers;

namespace Test.Specflow.Utilities;
public class MockFileDataFactory
{
    private Encoding _encoding;
    private string _frontMatter;
    private string _contents;

    public MockFileDataFactory WithContents(string contents)
    {
        _contents = contents;
        return this;
    }

    public MockFileDataFactory WithYamlFrontMatter(Dictionary<string, object> data = null)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("---");
        if (data != null && data.Any())
        {
            var serializer = YamlSerializer.Create();
            var raw = serializer.Serialize(data);
            stringBuilder.Append(raw);
        }
        stringBuilder.AppendLine("---");
        _frontMatter = stringBuilder.ToString();
        return this;
    }

    public MockFileDataFactory WithUtf8Encoding() => WithEncoding(new UTF8Encoding(false));
    private MockFileDataFactory WithEncoding(Encoding encoding)
    {
        _encoding = encoding;
        return this;
    }

    public MockFileData Create()
    {
        var sb = new StringBuilder();
        if (!string.IsNullOrEmpty(_frontMatter))
        {
            sb.Append(_frontMatter);
        }
        if (!string.IsNullOrEmpty(_contents))
        {
            sb.Append(_contents);
        }
        var data = sb.ToString();
        var bytes = _encoding.GetBytes(data);
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
