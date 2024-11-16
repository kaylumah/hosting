// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Kaylumah.Api
{
    public class RedirectOption
    {
        public bool Enabled
        { get; set; }
        public bool Permanent
        { get; set; }
        public string Pattern
        { get; set; }
        public string Rewrite
        { get; set; }

        public RedirectOption(string pattern, string rewrite)
        {
            Pattern = pattern;
            Rewrite = rewrite;
        }
    }
}