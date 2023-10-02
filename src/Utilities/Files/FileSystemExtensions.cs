// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

namespace Kaylumah.Ssg.Utilities;

public static class FileSystemExtensions
{
    public static Stream CreateReadStream(this IFileSystemInfo fileInfo)
    {
        var fileSystem = fileInfo.FileSystem;
        var result = fileSystem.FileStream.New(fileInfo.FullName, FileMode.Open);
        return result;
    }

    public static bool IsDirectory(this IFileSystemInfo fileSystemInfo)
    {
        var result = fileSystemInfo.GetType().IsAssignableTo(typeof(IDirectoryInfo));
        return result;
    }

    public static byte[] GetFileBytes(this IFileSystem fileSystem, string path)
    {
        var fileInfo = fileSystem.GetFile(path);
        using var fileStream = fileInfo.CreateReadStream();
        var result = fileStream.ToByteArray();
        return result;
    }
    public static IEnumerable<IFileSystemInfo> GetFiles(this IFileSystem fileSystem, string path,
        bool recursive = false)
    {
        // TODO: better solution
        var workingDirectory = string.IsNullOrEmpty(path) ? fileSystem.Directory.GetCurrentDirectory() : path;
        var result = new List<IFileSystemInfo>();
        var scanDirectory = fileSystem.DirectoryInfo.New(workingDirectory);
        if (!scanDirectory.Exists)
        {
            return result;
        }

        var scanResult = scanDirectory.GetFileSystemInfos();
        result.AddRange(scanResult);

        if (recursive)
        {
            var directories = scanResult.Where(x => x.IsDirectory());
            foreach (var directory in directories)
            {
                var files = fileSystem.GetFiles(directory.FullName, recursive);
                result.AddRange(files);
            }
        }

        return result;
    }

    public static IFileInfo GetFile(this IFileSystem fileSystem, string path)
    {
        var result = fileSystem.FileInfo.New(path);
        return result;
    }
}
