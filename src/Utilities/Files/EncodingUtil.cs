// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
using System.Text;

namespace Kaylumah.Ssg.Utilities;

public class EncodingUtil
{
    public Encoding DetermineEncoding(Stream stream)
    {
        using var reader = new StreamReader(stream, Encoding.Default, detectEncodingFromByteOrderMarks: true);
        if (reader.Peek() >= 0) // you need this!
            reader.Read();

        return reader.CurrentEncoding;
    }
}