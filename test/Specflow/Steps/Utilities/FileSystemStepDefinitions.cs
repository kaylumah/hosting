// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.IO;
using System.IO.Abstractions.TestingHelpers;
using TechTalk.SpecFlow;
using Test.Specflow.Utilities;

namespace Test.Specflow.Steps.Utilities
{
    [Binding]
    public class FileSystemStepDefinitions
    {
        readonly MockFileSystem _mockFileSystem;

        public FileSystemStepDefinitions(MockFileSystem mockFileSystem)
        {
            _mockFileSystem = mockFileSystem;
        }

        [Given("'(.*)' is a data file with the following contents:")]
        public void GivenIsADataFileWithTheFollowingContents(string fileName, string contents)
        {
            string dataFileName = Path.Combine(Constants.Directories.SourceDirectory, Constants.Directories.DataDirectory, fileName);
            _mockFileSystem.AddFile(dataFileName, MockFileDataFactory.PlainFile(contents));
        }

        [Given("'(.*)' is a layout file with the following contents:")]
        public void GivenIsALayoutFileWithTheFollowingContents(string fileName, string contents)
        {
            string layoutFileName = Path.Combine(Constants.Directories.SourceDirectory, Constants.Directories.LayoutDirectory, fileName);
            _mockFileSystem.AddFile(layoutFileName, MockFileDataFactory.PlainFile(contents));
        }

        [Given("'(.*)' is an asset file with the following contents:")]
        public void GivenIsAnAssetFileWithTheFollowingContents(string fileName, string contents)
        {
            string assetFileName = Path.Combine(Constants.Directories.SourceDirectory, Constants.Directories.AssetDirectory, fileName);
            _mockFileSystem.AddFile(assetFileName, MockFileDataFactory.PlainFile(contents));
        }

        [Given("'(.*)' is a post with the following contents:")]
        public void GivenIsAPostWithTheFollowingContents(string fileName, string contents)
        {
            string postFileName = Path.Combine(Constants.Directories.SourceDirectory, Constants.Directories.PostDirectory, fileName);
            _mockFileSystem.AddFile(postFileName, MockFileDataFactory.PlainFile(contents));
        }

        [Given("'(.*)' is an empty post:")]
        public void GivenIsAnEmptyPost(string fileName)
        {
            string postFileName = Path.Combine(Constants.Directories.SourceDirectory, Constants.Directories.PostDirectory, fileName);
            _mockFileSystem.AddFile(postFileName, MockFileDataFactory.EmptyFile());
        }

        [Given("'(.*)' is an empty page:")]
        public void GivenIsAnEmptyPage(string fileName)
        {
            string pageDirectory = Path.Combine(Constants.Directories.SourceDirectory, Constants.Directories.PageDirectory, fileName);
            _mockFileSystem.AddFile(pageDirectory, MockFileDataFactory.EmptyFile());
        }

        [Given("'(.*)' is an empty file:")]
        public void GivenIsAnEmptyFile(string fileName)
        {
            string normalizedFileName = fileName.Replace('/', Path.DirectorySeparatorChar);
            string filePath = Path.Combine(Constants.Directories.SourceDirectory, normalizedFileName);
            _mockFileSystem.AddFile(filePath, MockFileDataFactory.EmptyFile());
        }
    }
}
