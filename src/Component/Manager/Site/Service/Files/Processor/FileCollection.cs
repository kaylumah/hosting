// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace Kaylumah.Ssg.Manager.Site.Service.Files.Processor
{
    [DebuggerDisplay("{Name} {Files.Length} Files")]
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
    public class FileCollection
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
    {
        public string Name { get; set; }
        public File[] Files { get; set; }
    }
}
