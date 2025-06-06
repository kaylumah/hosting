﻿// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using Kaylumah.Ssg.Utilities;

namespace Kaylumah.Ssg.Extensions.Metadata.Abstractions
{
    public class BuildData
    {
        public string BuildId
        { get; set; }
        public string BuildNumber
        { get; set; }
        public string Version
        { get; set; }
        public string Copyright
        { get; set; }
        public string SourceBaseUri
        { get; set; }
        public string SourceBuildUri
        { get; set; }
        public string GitHash
        { get; set; }
        public string ShortGitHash
        { get; set; }
        public DateTimeOffset Time
        { get; set; }

        public BuildData(AssemblyInfo info, DateTimeOffset buildTime)
        {
            Time = buildTime;
            string version = "1.0.0+LOCALBUILD";
            if (6 < info.Version.Length)
            {
                version = info.Version;
            }

            string appVersion = version[..version.IndexOf('+')];
            string gitHash = version[(version.IndexOf('+') + 1)..]; // version.Substring(version.IndexOf('+') + 1);
            string shortGitHash = gitHash[..7];
            string repositoryType = info.Metadata["RepositoryType"];
            string repositoryUrl = info.Metadata["RepositoryUrl"];

            if (repositoryUrl.EndsWith($".{repositoryType}", StringComparison.Ordinal))
            {
                int index = repositoryUrl.LastIndexOf($".{repositoryType}", StringComparison.Ordinal);
                SourceBaseUri = repositoryUrl.Remove(index, repositoryType.Length + 1).Insert(index, "/commit");
                SourceBuildUri = repositoryUrl.Remove(index, repositoryType.Length + 1).Insert(index, "/actions/runs");
            }
            else
            {
                SourceBaseUri = repositoryUrl + "/commit";
                SourceBuildUri = repositoryUrl + "/actions/runs";
            }

            Version = appVersion;
            Copyright = info.Copyright;
            GitHash = gitHash;
            ShortGitHash = shortGitHash;

            BuildId = info.Metadata[nameof(BuildId)];
            BuildNumber = info.Metadata[nameof(BuildNumber)];
        }
    }
}
