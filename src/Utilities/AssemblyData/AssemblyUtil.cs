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
}