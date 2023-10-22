// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Kaylumah.Ssg.Manager.Site.Service.Files.Processor;
using Moq;
using Test.Specflow.Entities;
using Test.Specflow.Extensions;
using File = Kaylumah.Ssg.Manager.Site.Service.Files.Processor.File;

namespace Test.Specflow.Utilities
{
    public class FileProcessorMock : StrictMock<IFileProcessor>
    {
        readonly ArticleCollection _Articles;

        public FileProcessorMock(ArticleCollection articles)
        {
            _Articles = articles;
            SetupProcess();
        }

        public void SetupProcess()
        {
            Setup(fileProcessor =>
                    fileProcessor.Process(It.IsAny<FileFilterCriteria>()))
                .Callback((FileFilterCriteria criteria) => { })
                .ReturnsAsync((FileFilterCriteria criteria) =>
                {
                    List<File> result = new List<File>();

                    if (_Articles.Any())
                    {
                        result.AddRange(_Articles.ToPageMetaData().ToFile());
                    }

                    return result;
                });
        }
    }
}
