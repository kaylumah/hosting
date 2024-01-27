// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Kaylumah.Ssg.Utilities
{
    public class AssemblyInfo
    {
        public string Copyright { get; set; }
        public string Version { get; set; }
        public Dictionary<string, string> Metadata { get; set; }

        public AssemblyInfo(string copyright, string version, Dictionary<string, string> metaData)
        {
            Copyright = copyright;
            Version = version;
            Metadata = metaData;
        }
    }
}
