// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Kaylumah.Ssg.Manager.Site.Service
{
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
    public class Collection
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
    {
        public string Name { get; set; } = string.Empty;
        public bool Output { get; set; }
        public string TreatAs { get; set; } = string.Empty;
    }
}
