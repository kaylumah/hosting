using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Ssg.Extensions.Data.Yaml
{
    public class YamlParser : IYamlParser
    {
        private readonly IDeserializer _deserializer;

        public YamlParser()
        {
            _deserializer = new DeserializerBuilder()
               .WithNamingConvention(CamelCaseNamingConvention.Instance)
               .Build();
        }

        public T Parse<T>(string raw)
        {
            return _deserializer.Deserialize<T>(raw);
        }
    }
}