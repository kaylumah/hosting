// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.IO;
using System.Text;

namespace Kaylumah.Ssg.Utilities
{
    public static partial class EncodingExtensions
    {
        public static Encoding DetermineEncoding(this Stream stream)
        {
            using StreamReader reader = new StreamReader(stream, Encoding.Default, detectEncodingFromByteOrderMarks: true);
            // if (reader.Peek() >= 0) // you need this! (swapped for IDESIGN105)
            if (0 <= reader.Peek())
            {
                reader.Read();
            }

            return reader.CurrentEncoding;
        }
    }
}
