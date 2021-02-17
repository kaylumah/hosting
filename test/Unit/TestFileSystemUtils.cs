using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentAssertions;
using Kaylumah.Ssg.Utilities;
using Microsoft.Extensions.FileProviders;
using Moq;
using Test.Utilities;
using Xunit;
using YamlDotNet.Serialization;

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

        [Theory]
        [InlineData("index.html", "", "index.html")]
        //[InlineData("404.html", "", "404.html")]
        //[InlineData("_posts/my-post.md", "", "my-post.md")]
        [InlineData("2021-01-01-my-post.md", "_posts", "my-post.md")]
        public void Test2(string input, string instruction, string output)
        {
            var sut = new PermantUriRewriter();
            var result = sut.Rewrite(string.Empty, instruction, input);
            // result.Should().NotBeNullOrEmpty();
            // result.Should().Be(output);
        }

        [Fact]
        public void Test3()
        {
            var parser = new YamlParser();

            var sb = new StringBuilder()
                .AppendLine("layout: 'default'")
                .AppendLine("permalink: '/:year/:month/:day/:name:ext'")
                .ToString();
            var result = parser.Parse<TempFileMetadata>(sb);
        }
    }

    public class TempFileMetadata
    {
        public Dictionary<string, object> Data { get; } = new Dictionary<string, object>();

        public string Permalink { get;set; }

        public string Layout 
        { 
            get 
            {
                if (Data.ContainsKey(nameof(Layout)))
                {
                    return (string)Data[nameof(Layout)];
                }
                return null;
            }
            set
            {
                Data[nameof(Layout)] = value;
            }
        }

        public object this[string key]
        {
            get
            {
                return Data[key];
            }
            set
            {
                Data[key] = value;
            }
        }
    }

    public class PermantUriRewriter
    {
        public string Rewrite(string collection, string instruction, string fileName)
        {
            var pattern = @"((?<year>\d{4})\-(?<month>\d{2})\-(?<day>\d{2})\-)?(?<filename>[\s\S]*?)\.(?<ext>.*)";
            var match = Regex.Match(fileName, pattern);

            var date = match.FromFileName();
            var outputFileName = match.FileNameByPattern();

            var source = "/:year/:month/:day/:name:ext";
            var result = source
                .Replace("/:year", date == null ? string.Empty : $"/{date?.ToString("yyyy")}")
                .Replace("/:month", date == null ? string.Empty : $"/{date?.ToString("MM")}")
                .Replace("/:day", date == null ? string.Empty : $"/{date?.ToString("dd")}");

            result = result.Replace(":name", Path.GetFileNameWithoutExtension(outputFileName))
                .Replace(":ext", Path.GetExtension(outputFileName));
            if (result.StartsWith("/"))
            {
                result = result[1..];
            }

            /*
                        var pattern = @"((?<year>\d{4})\-(?<month>\d{2})\-(?<day>\d{2})\-)?(?<filename>[\s\S]*?)\.(?<ext>.*)";
            var match = Regex.Match($"{model.ContentFileResourceName}{model.Extension}", pattern);
            if (match.Success)
            {
                // We have a match...
            }
            // Determine Date
            // FrontMatter vs FileName vs FolderStructure
            var date = DateTime.Now;

            // Determine Name (File-name vs FrontMatter)
            var name = "my-post";

            // Determine Extension (ie markdown should be html)
            var extension = ".html";

            // Determine pattern (Site, Collection, File)
            var source = "/:year/:month/:day/:name:ext";
            var result = source
                            .Replace(":year", date.ToString("yyyy"))
                            .Replace(":month", date.ToString("MM"))
                            .Replace(":day", date.ToString("dd"))
                            .Replace(":title", name)
                            .Replace(":ext", extension);
                            */


            return result;
        }
    }

    public static class ExtensionMethods
    {
        public static string FileNameByPattern(this System.Text.RegularExpressions.Match match)
        {
            if (match.Success)
            {
                var filename = match.Groups.GetValueOrDefault("filename");
                var ext = match.Groups.GetValueOrDefault("ext");
                if (filename.Success && ext.Success)
                {
                    return $"{filename}.{ext}";
                }
            }
            return null;
        }

        public static DateTime? FromFileName(this System.Text.RegularExpressions.Match match)
        {
            if (match.Success)
            {
                var year = match.Groups.GetValueOrDefault("year");
                var month = match.Groups.GetValueOrDefault("month");
                var day = match.Groups.GetValueOrDefault("day");

                if (year.Success && month.Success && day.Success)
                {
                    return DateTime.Parse($"{year.Value}-{month.Value}-{day.Value}");
                }
            }
            return null;
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