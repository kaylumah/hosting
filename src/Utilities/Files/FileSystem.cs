// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.IO.Abstractions;

namespace Kaylumah.Ssg.Utilities;

public static class FileSystemExtensions
{
    public static Stream CreateReadStream(this IFileSystemInfo fileInfo)
    {
        var fileSystem = fileInfo.FileSystem;
        return fileSystem.FileStream.Create(fileInfo.FullName, FileMode.Open);
    }

    public static bool IsDirectory(this IFileSystemInfo fileSystemInfo)
    {
        return fileSystemInfo.GetType().IsAssignableTo(typeof(IDirectoryInfo));
    }
}

public class FileSystem : IFileSystem
{
    private readonly System.IO.Abstractions.IFileSystem _fileSystem;

    public FileSystem(System.IO.Abstractions.IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    public void CreateDirectory(string path)
    {
        Directory.CreateDirectory(path);
    }

    public IFileInfo GetFile(string path)
    {
        return _fileSystem.FileInfo.FromFileName(path);
    }

    public byte[] GetFileBytes(string path)
    {
        var fileInfo = GetFile(path);
        using var fileStream = fileInfo.CreateReadStream();
        return fileStream.ToByteArray();
    }

    public IEnumerable<IFileSystemInfo> GetFiles(string path, bool recursive = false)
    {
        // TODO: better solution
        var workingDirectory = string.IsNullOrEmpty(path) ? _fileSystem.Directory.GetCurrentDirectory() : path;
        var result = new List<IFileSystemInfo>();
        var scanDirectory = _fileSystem.DirectoryInfo.FromDirectoryName(workingDirectory);
        var scanResult = scanDirectory.GetFileSystemInfos();
        result.AddRange(scanResult);

        if (recursive)
        {
            var directories = scanResult.Where(x => x.IsDirectory());
            foreach (var directory in directories)
            {
                result.AddRange(GetFiles(directory.FullName, recursive));
            }
        }

        return result;
    }

    public async Task WriteAllBytesAsync(string path, byte[] bytes)
    {
        await File.WriteAllBytesAsync(path, bytes).ConfigureAwait(false);
    }
}
