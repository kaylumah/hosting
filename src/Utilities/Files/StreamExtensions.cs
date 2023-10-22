// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace System.IO
{
    public static class StreamExtensions
    {
        public static byte[] ToByteArray(this Stream input)
        {
            using MemoryStream ms = new MemoryStream();
            input.CopyTo(ms);
            byte[] result = ms.ToArray();
            return result;
        }
    }
}
