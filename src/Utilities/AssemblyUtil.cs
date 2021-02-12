using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kaylumah.Ssg.Utilities
{
    public class AssemblyUtil
    {
        public AssemblyInfo RetrieveAssemblyInfo(Assembly assembly)
        {
            var copyrightAttribute = assembly.GetAttribute<AssemblyCopyrightAttribute>();
            var informationalVersionAttribute = assembly.GetAttribute<AssemblyInformationalVersionAttribute>();
            var metadataAttributes = assembly
                .GetAttribtutes<AssemblyMetadataAttribute>()
                .ToDictionary(a => a.Key, a => a.Value);

            return new AssemblyInfo()
            {
                Copyright = copyrightAttribute.Copyright,
                Version = informationalVersionAttribute.InformationalVersion,
                Metadata = metadataAttributes
            };
        }
    }

    public class AssemblyInfo
    {
        public string Copyright { get;set; }
        public string Version { get;set; }
        public Dictionary<string, string> Metadata { get; set; }
    }

    public static class AssemblyExtensions
    {
        public static T GetAttribute<T>(this Assembly assembly)
        {
            return assembly.GetCustomAttributes(typeof(T)).Cast<T>().Single();
        }

        public static IEnumerable<T> GetAttribtutes<T>(this Assembly assembly)
        {
            return assembly.GetCustomAttributes(typeof(T))
                .Cast<T>();
        }
    }
}