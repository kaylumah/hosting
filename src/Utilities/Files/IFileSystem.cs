// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;

namespace Kaylumah.Ssg.Utilities
{
    public interface IFileSystem
    {
        IEnumerable<IFileInfo> GetFiles(string path, bool recursive = false);
        IFileInfo GetFile(string path);
        byte[] GetFileBytes(string path);
        Task<File<TData>> GetFile<TData>(string path);
        IDirectoryContents GetDirectoryContents(string path);
        void CreateDirectory(string path);
        Task WriteAllBytesAsync(string path, byte[] bytes);
    }
}