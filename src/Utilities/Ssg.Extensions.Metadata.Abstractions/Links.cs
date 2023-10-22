// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Ssg.Extensions.Metadata.Abstractions
{
    public class Links
    {
        public string Twitter { get; set; }
        public string TwitterProfileUrl => string.IsNullOrEmpty(Twitter) ? null : $"https://twitter.com/{Twitter}";
        public string Linkedin { get; set; }
        public string LinkedinProfileUrl => string.IsNullOrEmpty(Linkedin) ? null : $"https://www.linkedin.com/in/{Linkedin}";
        public string Medium { get; set; }
        public string MediumProfileUrl => string.IsNullOrEmpty(Medium) ? null : $"https://{Medium}.medium.com";
        public string Devto { get; set; }
        public string DevtoProfileUrl => string.IsNullOrEmpty(Devto) ? null : $"https://dev.to/{Devto}";
        public string Github { get; set; }
        public string GithubProfileUrl => string.IsNullOrEmpty(Github) ? null : $"https://github.com/{Github}";
    }
}
