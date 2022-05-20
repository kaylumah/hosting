// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;
using Kaylumah.Ssg.Manager.Site.Service.Files.Processor;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Test.Specflow;

[Binding]
public class FileMetadataParserTests
{
    private readonly IFileMetadataParser _fileMetadataParser;
    private readonly MetadataCriteria _metadataCriteria = new();
    private readonly Test.Specflow.Entities.FileCollection _files = new();

    public FileMetadataParserTests(IFileMetadataParser fileMetadataParser)
    {
        _fileMetadataParser = fileMetadataParser;
    }

    [Given("the following files:")]
    public void GivenTheFollowingFiles(Table table)
    {
        var files = table
            .CreateSet<Test.Specflow.Entities.File>();
        _files.AddRange(files);
    }

    [Given(@"a file named '([^']*)' has the following contents:")]
    public void GivenAFileNamedHasTheFollowingContents(string p0, string multilineText)
    {
        _files[p0].Content = multilineText;
    }


    [When("the following is parsed:")]
    public void When()
    {
        var result = _fileMetadataParser.Parse(_metadataCriteria);
    }
}
