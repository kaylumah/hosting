// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;

public class MetadataParserOptions
{
    public const string Options = "Metadata";
    public DefaultMetadatas Defaults { get; set; } = new DefaultMetadatas();
    public Dictionary<string, string> ExtensionMapping { get; set; } = new Dictionary<string, string>();
}