using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Kaylumah.Ssg.Utilities;
using Microsoft.Extensions.FileProviders;
using Moq;
using Test.Utilities;
using Xunit;

namespace Test.Unit
{
    public class TestFileSystemUtils
    {
        [Fact]
        public async Task Test1()
        {
            var fileProviderMock = new Mock<IFileProvider>();
            var rootFolder = "/a/b/c/";
            fileProviderMock.SetupFileProviderMock(rootFolder, new List<FakeDirectory>() {
                new FakeDirectory(string.Empty, new FakeFile[] {}),
                new FakeDirectory("_posts", new FakeFile[] {
                    // new FakeFile("_posts/first.md"),
                    new FakeFile("_posts/second.md", Encoding.UTF8.GetBytes("---\r\n---")),
                    new FakeFile("_posts/third.md", Encoding.UTF8.GetBytes("---\r\nlayout: 'default'---"))
                })
            });

            IFileSystem fileSystem = new FileSystem(fileProviderMock.Object);
            var sut = new TempCollectionProcessor(fileSystem);
            await sut.Process(new string[] { "_posts" });
        }
    }

    public class TempCollectionProcessor
    {
        private readonly IFileSystem _fileSystem;
        public TempCollectionProcessor(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public async Task Process(string[] directories)
        {
            foreach(var directory in directories)
            {
                var collectionDefaults = GetDefaults(directory);
                var directoryFiles = _fileSystem.GetFiles(directory);
                foreach(var file in directoryFiles)
                {
                    var filePath = Path.Combine(directory, file.Name);

                    
                    var fileInfo = await _fileSystem.GetFile<Dictionary<string, object>>(filePath);
                    var fileConfig = new Dictionary<string, object>(collectionDefaults);
                    if (fileInfo.Data != null)
                    {
                        foreach(var entry in fileInfo.Data)
                        {
                            if (fileConfig.ContainsKey(entry.Key))
                            {
                                // TODO log that its overwritten ...
                            }
                            fileConfig[entry.Key] = entry.Value;
                        }
                    }

                }
            }
        }

        private Dictionary<string, object> GetDefaults(string collectionName)
        {
            var collectionDefaults = new Dictionary<string, Dictionary<string, object>>()
            {
                {   
                    "_posts", 
                    new Dictionary<string, object>() {
                        { "layout", "post" }
                    }
                }
            };
            return collectionDefaults.ContainsKey(collectionName) ? collectionDefaults[collectionName] : new Dictionary<string, object>();
        }
    }
}