// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Ssg.Manager.Site.Service.Files.Processor;
using Moq;
using Test.Specflow.Entities;
using Test.Specflow.Extensions;
using File = Kaylumah.Ssg.Manager.Site.Service.Files.Processor.File;

namespace Test.Specflow.Utilities;

public class FileProcessorMock : StrictMock<IFileProcessor>
{
    private readonly ArticleCollection _articles;

    public FileProcessorMock(ArticleCollection articles)
    {
        _articles = articles;
        SetupProcess();
    }

    public void SetupProcess()
    {
        Setup(fileProcessor =>
                fileProcessor.Process(It.IsAny<FileFilterCriteria>()))
            .Callback((FileFilterCriteria criteria) => { })
            .ReturnsAsync((FileFilterCriteria criteria) =>
            {
                var result = new List<File>();

                if (_articles.Any())
                {
                    result.AddRange(_articles.ToPageMetaData().ToFile());
                }

                return result;
            });
    }
}
