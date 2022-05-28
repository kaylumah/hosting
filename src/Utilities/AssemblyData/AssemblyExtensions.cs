// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Reflection;

namespace Kaylumah.Ssg.Utilities;

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

    public static AssemblyInfo RetrieveAssemblyInfo(this Assembly assembly)
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
