// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;
using TechTalk.SpecFlow;

namespace Test.Specflow;

[Binding]
public class MetadataParserTests
{
    private readonly IFileMetadataParser _fileMetadataParser;

    public MetadataParserTests(IFileMetadataParser fileMetadataParser)
    {
        _fileMetadataParser = fileMetadataParser;
    }

    [When("the following is parsed:")]
    public void When()
    {
        _fileMetadataParser.Parse(new MetadataCriteria { });
    }
}
