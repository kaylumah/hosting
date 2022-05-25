// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Ssg.Utilities;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    public static partial class FileSystemInfoExtensions
    {
        public static string ReadFile (this System.IO.Abstractions.IFileSystemInfo file)
        {
            var stream = file.CreateReadStream();
            using var reader = new StreamReader(stream);
            var raw = reader.ReadToEnd();
            return raw;
        }
    }
}
