// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.IO;
using Kaylumah.Ssg.Utilities;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    public static partial class FileSystemInfoExtensions
    {
        public static string ReadFile(this System.IO.Abstractions.IFileSystemInfo file)
        {
            Stream stream = file.CreateReadStream();
            using StreamReader reader = new StreamReader(stream);
            string raw = reader.ReadToEnd();
            return raw;
        }
    }
}
