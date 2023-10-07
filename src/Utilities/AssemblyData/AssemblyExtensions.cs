// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kaylumah.Ssg.Utilities;

public static class AssemblyExtensions
{
    public static T GetAttribute<T>(this Assembly assembly)
    {
        var result = assembly.GetCustomAttributes(typeof(T)).Cast<T>().Single();
        return result;
    }

    public static IEnumerable<T> GetAttribtutes<T>(this Assembly assembly)
    {
        var result = assembly.GetCustomAttributes(typeof(T)).Cast<T>();
        return result;
    }

    public static AssemblyInfo RetrieveAssemblyInfo(this Assembly assembly)
    {
        var copyrightAttribute = assembly.GetAttribute<AssemblyCopyrightAttribute>();
        var informationalVersionAttribute = assembly.GetAttribute<AssemblyInformationalVersionAttribute>();
        var metadataAttributes = assembly
            .GetAttribtutes<AssemblyMetadataAttribute>()
            .ToDictionary(a => a.Key, a => a.Value);

#pragma warning disable IDESIGN103
        var result = new AssemblyInfo()
        {
            Copyright = copyrightAttribute.Copyright,
            Version = informationalVersionAttribute.InformationalVersion,
            Metadata = metadataAttributes
        };
#pragma warning restore IDESIGN103
        return result;
    }
}
