using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.FileProviders;

namespace Kaylumah.Ssg.Utilities
{
    public interface IFileSystem
    {
        IEnumerable<IFileInfo> GetFiles(string path, bool recursive = false);
        IFileInfo GetFile(string path);
    }

    public class FileSystem : IFileSystem
    {
        private readonly IFileProvider _fileProvider;

        public FileSystem(IFileProvider fileProvider)
        {
            _fileProvider = fileProvider;
        }

        public IFileInfo GetFile(string path)
        {
            return _fileProvider.GetFileInfo(path);
        }

        public IEnumerable<IFileInfo> GetFiles(string path, bool recursive = false)
        {
            var result = new List<IFileInfo>();
            var directoryContents = _fileProvider.GetDirectoryContents(path);
            result.AddRange(directoryContents.Where(x => !x.IsDirectory));

            if (recursive)
            {
                var directories = directoryContents.Where(x => x.IsDirectory);
                foreach(var directory in directories)
                {
                    result.AddRange(GetFiles(Path.Combine(path, directory.Name), recursive));
                }
            }
            return result;
        }
    }
}