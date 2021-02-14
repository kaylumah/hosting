using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;

namespace Kaylumah.Ssg.Utilities
{
    public class File<TMetadata>
    {
        public string Encoding { get;set; }
        public string Name { get;set; }
        public string Path { get;set; }
        public string Content { get;set; }
        public TMetadata Data { get;set; }
    }

    public class FileUtil
    {
        private readonly IFileProvider _fileProvider;
        public FileUtil(IFileProvider fileProvider)
        {
            _fileProvider = fileProvider;
        }
        public async Task<File<T>> GetFileInfo<T>(string relativePath)
        {
            var fileInfo = _fileProvider.GetFileInfo(relativePath);
            var encoding = new EncodingUtil().DetermineEncoding(fileInfo.CreateReadStream());
            var fileName = fileInfo.Name;
            using var streamReader = new StreamReader(fileInfo.CreateReadStream());
            var text = await streamReader.ReadToEndAsync();
            var metadata = new MetadataUtil().Retrieve<T>(text);
            return new File<T>
            {
                Encoding = encoding.WebName,
                Name = fileName,
                Path = relativePath,
                Content = metadata.Content,
                Data = metadata.Data
            };
        }

        public List<IFileInfo> GetFiles(string path)
        {
            var result = new List<IFileInfo>();

            var info = _fileProvider.GetDirectoryContents(path);
            var directories = info.Where(x => x.IsDirectory);
            result.AddRange(info.Where(x => !x.IsDirectory));

            foreach (var directory in directories)
            {
                result.AddRange(GetFiles(Path.Combine(path, directory.Name)));
            }

            return result;
        }
    }
}