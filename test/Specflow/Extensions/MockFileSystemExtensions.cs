// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

// ReSharper disable once CheckNamespace

using Test.Specflow;
using Test.Specflow.Utilities;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace System.IO.Abstractions.TestingHelpers;

public static class MockFileSystemExtensions
{
    public static void AddYamlDataFile(this MockFileSystem mockFileSystem, string fileName, object data)
    {
        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
            .Build();
        
        var yamlContents = serializer.Serialize(data);
        var dataFileName = Path.Combine(Constants.Directories.SourceDirectory, Constants.Directories.DataDirectory, fileName);
        mockFileSystem.AddFile(dataFileName, MockFileDataFactory.PlainFile(yamlContents));
    }
}
