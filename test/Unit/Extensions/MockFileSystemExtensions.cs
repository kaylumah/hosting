// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using Test.Unit.Helpers;
using Test.Unit.Utilities;

// ReSharper disable once CheckNamespace
namespace System.IO.Abstractions.TestingHelpers
{
    public static class MockFileSystemExtensions
    {
        public static void AddYamlDataFile(this MockFileSystem mockFileSystem, string fileName, object data)
        {
            YamlDotNet.Serialization.ISerializer serializer = YamlSerializer.Create();
            string yamlContents = serializer.Serialize(data);
            string dataFileName = Path.Combine(Kaylumah.Ssg.Manager.Site.Service.Constants.Directories.SourceDataDirectory, fileName);
            MockFileData file = MockFileDataFactory.PlainFile(yamlContents);
            mockFileSystem.AddFile(dataFileName, file);
        }
    }
}
