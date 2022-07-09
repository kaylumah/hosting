// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.IO.Abstractions;

namespace Kaylumah.Ssg.Utilities;

public interface IFileSystem
{
    IEnumerable<IFileSystemInfo> GetFiles(string path, bool recursive = false);
    IFileInfo GetFile(string path);
    byte[] GetFileBytes(string path);
    Task WriteAllBytesAsync(string path, byte[] bytes);
}
