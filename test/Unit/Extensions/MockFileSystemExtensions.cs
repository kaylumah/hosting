// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using Test.Unit.Helpers;
using Test.Unit.Utilities;

using Constants = Test.Unit.Constants;

// ReSharper disable once CheckNamespace
namespace System.IO.Abstractions.TestingHelpers
{
    public static class MockFileSystemExtensions
    {
        public static void AddYamlDataFile(this MockFileSystem mockFileSystem, string fileName, object data)
        {
            YamlDotNet.Serialization.ISerializer serializer = YamlSerializer.Create();
            string yamlContents = serializer.Serialize(data);
            string dataFileName = Path.Combine(Kaylumah.Ssg.Manager.Site.Service.Constants.Directories.DataDirectory, fileName);
            mockFileSystem.AddFile(dataFileName, MockFileDataFactory.PlainFile(yamlContents));
        }
    }
}
