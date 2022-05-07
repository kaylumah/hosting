// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.IO.Abstractions;
using Microsoft.Extensions.FileProviders;

namespace Kaylumah.Ssg.Utilities;

public interface IFileSystem
{
    IEnumerable<IFileSystemInfo> GetFiles(string path, bool recursive = false);
    System.IO.Abstractions.IFileInfo GetFile(string path);
    byte[] GetFileBytes(string path);
    void CreateDirectory(string path);
    Task WriteAllBytesAsync(string path, byte[] bytes);
}
