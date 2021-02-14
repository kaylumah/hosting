using System;
using Microsoft.Extensions.FileProviders;
using Moq;
using Xunit;
using Test.Utilities;
using System.Collections.Generic;
using static Test.Utilities.FileProviderExtensions;
using System.Linq;

namespace Test.Unit
{
    public class UnitTest1
    {

        private List<object> Process(Mock<IFileProvider> fileProvider, List<FakeDirectory> directories, string basePath = "", int depth = 1)
        {
            var result = new List<object>();
            var targets = directories.Where(x => x.Depth == depth && x.FolderPath.Equals(basePath)).ToList();
            foreach(var target in targets)
            {
                directories.Remove(target);
            }
            foreach(var target in targets)
            {
                var subTargets = Process(fileProvider, directories, target.Name, depth + 1);
                


                // result.Add(new { Name = target.Name });
                result.AddRange(subTargets);
            }
            return result;
        }



        [Fact]
        public void Test1()
        {
            var provider = new Mock<IFileProvider>();
            var directories = new List<FakeDirectory>()
            {
                new FakeDirectory("assets", new FakeFile[] {}),
                new FakeDirectory("assets/css", new FakeFile[] {}),
                new FakeDirectory("assets/js", new FakeFile[] {}),
                new FakeDirectory("assets/images", new FakeFile[] {}),
                // new FakeDirectory("posts", new FakeFile[] {}),
                new FakeDirectory("posts/january", new FakeFile[] {})
            };
            var y = Process(provider, directories);
        }
    }
}
