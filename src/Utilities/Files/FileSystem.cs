// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using Microsoft.Extensions.FileProviders;

namespace Kaylumah.Ssg.Utilities;

public class FileSystem : IFileSystem
{
    private readonly IFileProvider _fileProvider;
    

    public FileSystem(IFileProvider fileProvider)
    {
        _fileProvider = fileProvider;
    }

    public void CreateDirectory(string path)
    {
        Directory.CreateDirectory(path);
    }

    public IEnumerable<IFileInfo> GetDirectoryContents(string path)
    {
        return GetFiles(path);
    }

    public IFileInfo GetFile(string path)
    {
        return _fileProvider.GetFileInfo(path);
    }

    public byte[] GetFileBytes(string path)
    {
        var fileInfo = GetFile(path);
        var fileStream = fileInfo.CreateReadStream();
        return fileStream.ToByteArray();
    }

    public IEnumerable<IFileInfo> GetFiles(string path, bool recursive = false)
    {
        var result = new List<IFileInfo>();
        var directoryContents = _fileProvider.GetDirectoryContents(path);
        result.AddRange(directoryContents);

        if (recursive)
        {
            var directories = directoryContents.Where(x => x.IsDirectory);
            foreach (var directory in directories)
            {
                result.AddRange(GetFiles(Path.Combine(path, directory.Name), recursive));
            }
        }
        return result;
    }

    public async Task WriteAllBytesAsync(string path, byte[] bytes)
    {
        await File.WriteAllBytesAsync(path, bytes);
    }
}
