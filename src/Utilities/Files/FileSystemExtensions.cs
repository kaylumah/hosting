// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.IO.Abstractions;

namespace Kaylumah.Ssg.Utilities;

public static class FileSystemExtensions
{
    public static Stream CreateReadStream(this IFileSystemInfo fileInfo)
    {
        var fileSystem = fileInfo.FileSystem;
        return fileSystem.FileStream.New(fileInfo.FullName, FileMode.Open);
    }

    public static bool IsDirectory(this IFileSystemInfo fileSystemInfo)
    {
        return fileSystemInfo.GetType().IsAssignableTo(typeof(IDirectoryInfo));
    }

    public static byte[] GetFileBytes(this IFileSystem fileSystem, string path)
    {
        var fileInfo = fileSystem.GetFile(path);
        using var fileStream = fileInfo.CreateReadStream();
        return fileStream.ToByteArray();
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
                result.AddRange(fileSystem.GetFiles(directory.FullName, recursive));
            }
        }

        return result;
    }

    public static IFileInfo GetFile(this IFileSystem fileSystem, string path)
    {
        return fileSystem.FileInfo.New(path);
    }
}
