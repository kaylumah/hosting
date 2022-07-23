// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Globalization;
using Test.Specflow.Utilities;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Constants = Test.Specflow.Constants;

// ReSharper disable once CheckNamespace
namespace System.IO.Abstractions.TestingHelpers;

public static class MockFileSystemExtensions
{
    public class CustomDateTimeYamlTypeConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type) => type == typeof (DateTimeOffset);

        public object ReadYaml(IParser parser, Type type)
        {
            throw new NotImplementedException();
        }

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            var dateTime = (DateTimeOffset) value;
            string str = dateTime.ToString("o", CultureInfo.InvariantCulture);
            emitter.Emit((ParsingEvent) new Scalar(AnchorName.Empty, TagName.Empty, str, ScalarStyle.Any, true, false));
        }
    }
    
    public static void AddYamlDataFile(this MockFileSystem mockFileSystem, string fileName, object data)
    {
        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull | DefaultValuesHandling.OmitDefaults)
            .WithTypeConverter(new CustomDateTimeYamlTypeConverter())
            .Build();
        
        var yamlContents = serializer.Serialize(data);
        var dataFileName = Path.Combine(Constants.Directories.SourceDirectory, Constants.Directories.DataDirectory, fileName);
        mockFileSystem.AddFile(dataFileName, MockFileDataFactory.PlainFile(yamlContents));
    }
}
