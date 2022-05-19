// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.IO.Abstractions.TestingHelpers;
using System.Text;

namespace Test.Specflow;

internal class MockFileDataFactory
{
    private string _frontMatter;
    private string _content;

    public static MockFileData DefaultFile(string content, Dictionary<string, object> data = null)
    {
        return new MockFileDataFactory()
            //.WithYamlFrontMatter(data)
            .WithContent(content)
            .Create();
    }

    public static MockFileData DefaultFile(Dictionary<string, object> data = null)
    {
        return new MockFileDataFactory()
            .WithYamlFrontMatter(data)
            .WithContent(string.Empty)
            .Create();
    }

    public MockFileDataFactory WithYamlFrontMatter(Dictionary<string, object> data = null)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("---");
        if (data != null && data.Any())
        {
            var raw = new YamlDotNet.Serialization.Serializer().Serialize(data);
            stringBuilder.Append(raw);
        }
        stringBuilder.AppendLine("---");
        _frontMatter = stringBuilder.ToString();
        return this;
    }

    public MockFileDataFactory WithContent(string content)
    {
        _content = content;
        return this;
    }

    public MockFileData Create()
    {
        var stringBuilder = new StringBuilder();
        if (!string.IsNullOrEmpty(_frontMatter))
        {
            stringBuilder.Append(_frontMatter);
        }
        if (!string.IsNullOrEmpty(_content))
        {
            stringBuilder.Append(_content);
        }
        var fileData = stringBuilder.ToString();
        var bytes = Encoding.UTF8.GetBytes(fileData);
        return new MockFileData(bytes);
    }
}
