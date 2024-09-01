// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;

namespace System.Reflection
{
    public static class AssemblyExtensions
    {
        public static Type[] GetImplementationsForType(this Assembly assembly, Type type)
        {
            if (type.IsInterface == false)
            {
                string errorMessage = $"The type '{type.FullName}' is not an interface and cannot be used to scan for implementations!";
                throw new ArgumentException(errorMessage, nameof(type));
            }

            // Ensure all types from an assembly are included
            Type[] assemblyTypes = assembly.GetTypes();
            IEnumerable<Type> filteredTypes = assemblyTypes.Where(type.IsAssignableFrom);

            // Only interessted in concretions
            filteredTypes = filteredTypes.Where(type => type.IsInterface == false);

            // Cannot use abstractions
            filteredTypes = filteredTypes.Where(type => type.IsAbstract == false);

            Type[] result = filteredTypes.ToArray();
            return result;
        }
    }
}
