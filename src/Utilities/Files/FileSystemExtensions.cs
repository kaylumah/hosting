// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

namespace Kaylumah.Ssg.Utilities
{
    public static class FileSystemExtensions
    {
        public static Stream CreateReadStream(this IFileSystemInfo fileInfo)
        {
            IFileSystem fileSystem = fileInfo.FileSystem;
            FileSystemStream result = fileSystem.FileStream.New(fileInfo.FullName, FileMode.Open);
            return result;
        }

        public static bool IsDirectory(this IFileSystemInfo fileSystemInfo)
        {
            bool result = fileSystemInfo.GetType().IsAssignableTo(typeof(IDirectoryInfo));
            return result;
        }

        public static byte[] GetFileBytes(this IFileSystem fileSystem, string path)
        {
            IFileInfo fileInfo = fileSystem.GetFile(path);
            using Stream fileStream = fileInfo.CreateReadStream();
            byte[] result = fileStream.ToByteArray();
            return result;
        }
        public static IEnumerable<IFileSystemInfo> GetFiles(this IFileSystem fileSystem, string path,
            bool recursive = false)
        {
            // TODO: better solution
            string workingDirectory = string.IsNullOrEmpty(path) ? fileSystem.Directory.GetCurrentDirectory() : path;
            List<IFileSystemInfo> result = new List<IFileSystemInfo>();
            IDirectoryInfo scanDirectory = fileSystem.DirectoryInfo.New(workingDirectory);
            if (!scanDirectory.Exists)
            {
                return result;
            }

            IFileSystemInfo[] scanResult = scanDirectory.GetFileSystemInfos();
            result.AddRange(scanResult);

            if (recursive)
            {
                IEnumerable<IFileSystemInfo> directories = scanResult.Where(x => x.IsDirectory());
                foreach (IFileSystemInfo directory in directories)
                {
                    IEnumerable<IFileSystemInfo> files = fileSystem.GetFiles(directory.FullName, recursive);
                    result.AddRange(files);
                }
            }

            return result;
        }

        public static IFileInfo GetFile(this IFileSystem fileSystem, string path)
        {
            IFileInfo result = fileSystem.FileInfo.New(path);
            return result;
        }
    }
}
