// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using Test.Specflow.Helpers;
using Test.Specflow.Utilities;

using Constants = Test.Specflow.Constants;

// ReSharper disable once CheckNamespace
namespace System.IO.Abstractions.TestingHelpers;

public static class MockFileSystemExtensions
{
    public static void AddYamlDataFile(this MockFileSystem mockFileSystem, string fileName, object data)
    {
        YamlDotNet.Serialization.ISerializer serializer = YamlSerializer.Create();
        string yamlContents = serializer.Serialize(data);
        string dataFileName = Path.Combine(Constants.Directories.SourceDirectory, Constants.Directories.DataDirectory, fileName);
        mockFileSystem.AddFile(dataFileName, MockFileDataFactory.PlainFile(yamlContents));
    }
}
