// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.IO.Abstractions.TestingHelpers;
using System.Text;

namespace Test.Specflow.Utilities;
#pragma warning disable CS3002
public class MockFileDataFactory
{
    private Encoding _encoding;
    public MockFileDataFactory WithUtf8Encoding() => WithEncoding(new System.Text.UTF8Encoding(false));
    private MockFileDataFactory WithEncoding(Encoding encoding)
    {
        _encoding = encoding;
        return this;
    }

    public MockFileData Create()
    {
        var data = string.Empty;
        var bytes = _encoding.GetBytes(data);
        return new MockFileData(bytes);
    }
    
    public static MockFileData EmptyFile()
    {
        return new MockFileDataFactory()
            .WithUtf8Encoding()
            .Create();
    }
}
#pragma warning restore CS3002
