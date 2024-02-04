// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kaylumah.Ssg.Utilities
{
    public static class AssemblyExtensions
    {
        public static T GetAttribute<T>(this Assembly assembly)
        {
            T result = assembly.GetCustomAttributes(typeof(T)).Cast<T>().Single();
            return result;
        }

        public static IEnumerable<T> GetAttributes<T>(this Assembly assembly)
        {
            IEnumerable<T> result = assembly.GetCustomAttributes(typeof(T)).Cast<T>();
            return result;
        }

        public static AssemblyInfo RetrieveAssemblyInfo(this Assembly assembly)
        {
            AssemblyCopyrightAttribute copyrightAttribute = assembly.GetAttribute<AssemblyCopyrightAttribute>();
            AssemblyInformationalVersionAttribute informationalVersionAttribute = assembly.GetAttribute<AssemblyInformationalVersionAttribute>();
            Dictionary<string, string> metadataAttributes = assembly
                .GetAttributes<AssemblyMetadataAttribute>()
                .ToDictionary(a => a.Key, a => a.Value!);

            AssemblyInfo result = new AssemblyInfo(copyrightAttribute.Copyright, informationalVersionAttribute.InformationalVersion, metadataAttributes);
            return result;
        }
    }
}
