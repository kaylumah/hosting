// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.IO;
using System.IO.Abstractions.TestingHelpers;
using Reqnroll;
using Test.Unit.Utilities;

namespace Test.Unit.Steps.Utilities
{
    [Binding]
    public class FileSystemStepDefinitions
    {
        readonly MockFileSystem _MockFileSystem;

        public FileSystemStepDefinitions(MockFileSystem mockFileSystem)
        {
            _MockFileSystem = mockFileSystem;
        }

        [Given("'(.*)' is a data file with the following contents:")]
        public void GivenIsADataFileWithTheFollowingContents(string fileName, string contents)
        {
            string dataFileName = Path.Combine(Kaylumah.Ssg.Manager.Site.Service.Constants.Directories.SourceDataDirectory, fileName);
            _MockFileSystem.AddFile(dataFileName, MockFileDataFactory.PlainFile(contents));
        }

        [Given("'(.*)' is a layout file with the following contents:")]
        public void GivenIsALayoutFileWithTheFollowingContents(string fileName, string contents)
        {
            string layoutFileName = Path.Combine(Kaylumah.Ssg.Manager.Site.Service.Constants.Directories.SourceLayoutsDirectory, fileName);
            _MockFileSystem.AddFile(layoutFileName, MockFileDataFactory.PlainFile(contents));
        }

        [Given("'(.*)' is an asset file with the following contents:")]
        public void GivenIsAnAssetFileWithTheFollowingContents(string fileName, string contents)
        {
            string assetFileName = Path.Combine(Kaylumah.Ssg.Manager.Site.Service.Constants.Directories.SourceAssetsDirectory, fileName);
            _MockFileSystem.AddFile(assetFileName, MockFileDataFactory.PlainFile(contents));
        }

        [Given("'(.*)' is a post with the following contents:")]
        public void GivenIsAPostWithTheFollowingContents(string fileName, string contents)
        {
            string postFileName = Path.Combine(Kaylumah.Ssg.Manager.Site.Service.Constants.Directories.SourcePostsDirectory, fileName);
            _MockFileSystem.AddFile(postFileName, MockFileDataFactory.PlainFile(contents));
        }

        [Given("'(.*)' is an empty post:")]
        public void GivenIsAnEmptyPost(string fileName)
        {
            string postFileName = Path.Combine(Kaylumah.Ssg.Manager.Site.Service.Constants.Directories.SourcePostsDirectory, fileName);
            _MockFileSystem.AddFile(postFileName, MockFileDataFactory.EmptyFile());
        }

        [Given("'(.*)' is an empty page:")]
        public void GivenIsAnEmptyPage(string fileName)
        {
            string pageDirectory = Path.Combine(Kaylumah.Ssg.Manager.Site.Service.Constants.Directories.SourcePagesDirectory, fileName);
            _MockFileSystem.AddFile(pageDirectory, MockFileDataFactory.EmptyFile());
        }

        [Given("'(.*)' is an empty file:")]
        public void GivenIsAnEmptyFile(string fileName)
        {
            string normalizedFileName = fileName.Replace('/', Path.DirectorySeparatorChar);
            string filePath = Path.Combine(Kaylumah.Ssg.Manager.Site.Service.Constants.Directories.SourceDirectory, normalizedFileName);
            _MockFileSystem.AddFile(filePath, MockFileDataFactory.EmptyFile());
        }
    }
}
