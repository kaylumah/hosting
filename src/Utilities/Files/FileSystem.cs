// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
using Microsoft.Extensions.FileProviders;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ssg.Extensions.Metadata.Abstractions;

namespace Kaylumah.Ssg.Utilities
{
 public class FileSystem : IFileSystem
    {
        private readonly IFileProvider _fileProvider;
        private readonly IMetadataProvider _metadataProvider;

        public FileSystem(IFileProvider fileProvider, IMetadataProvider metadataProvider)
        {
            _fileProvider = fileProvider;
            _metadataProvider = metadataProvider;
        }

        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public IDirectoryContents GetDirectoryContents(string path)
        {
            return _fileProvider.GetDirectoryContents(path);
        }

        public IFileInfo GetFile(string path)
        {
            return _fileProvider.GetFileInfo(path);
        }

        public async Task<File<TData>> GetFile<TData>(string path)
        {
            var fileInfo = GetFile(path);
            var encoding = new EncodingUtil().DetermineEncoding(fileInfo.CreateReadStream());
            var fileName = fileInfo.Name;
            using var streamReader = new StreamReader(fileInfo.CreateReadStream());
            var text = await streamReader.ReadToEndAsync();
            var metadata = _metadataProvider.Retrieve<TData>(text);
            return new File<TData>
            {
                Encoding = encoding.WebName,
                Name = fileName,
                Path = path,
                Content = metadata.Content,
                Data = metadata.Data
            };
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
            result.AddRange(directoryContents.Where(x => !x.IsDirectory));

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
}