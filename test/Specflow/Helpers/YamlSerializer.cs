// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using YamlDotNet.Serialization;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using System.Globalization;
using YamlDotNet.Serialization.NamingConventions;

namespace Test.Specflow.Helpers;

public static class YamlSerializer
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
    
    public static ISerializer Create()
    {
        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull | DefaultValuesHandling.OmitDefaults)
            .WithTypeConverter(new CustomDateTimeYamlTypeConverter())
            .Build();
        return serializer;
    }
}
