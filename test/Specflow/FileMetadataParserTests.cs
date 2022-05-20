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
public class FileMetadataParserTests
{
    private readonly IFileMetadataParser _fileMetadataParser;
    private readonly MetadataCriteria _metadataCriteria = new();

    public FileMetadataParserTests(IFileMetadataParser fileMetadataParser)
    {
        _fileMetadataParser = fileMetadataParser;
    }

    [Given(@"a file named '([^']*)' has the following contents:")]
    public void GivenAFileNamedHasTheFollowingContents(string p0, string multilineText)
    {
        _metadataCriteria.FileName = p0;
        _metadataCriteria.Content = multilineText;
    }


    [When("the following is parsed:")]
    public void When()
    {
        var result = _fileMetadataParser.Parse(_metadataCriteria);
    }
}
