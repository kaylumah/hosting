// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;
using Kaylumah.Ssg.Manager.Site.Service.Files.Processor;
using Moq;
using File = Kaylumah.Ssg.Manager.Site.Service.Files.Processor.File;

namespace Test.Specflow.Utilities;

public class FileProcessorMock : StrictMock<IFileProcessor>
{
    public FileProcessorMock()
    {
        SetupProcess();
    }

    public void SetupProcess()
    {
        Setup(fileProcessor =>
                fileProcessor.Process(It.IsAny<FileFilterCriteria>()))
            .Callback((FileFilterCriteria criteria) => { })
            .ReturnsAsync((FileFilterCriteria criteria) =>
            {
                var result = new List<File>
                {
                    new File()
                    {
                        Content = String.Empty,
                        Name = "Hello World!",
                        MetaData = new FileMetaData()
                    }
                };

                return result;
            });
    }
}
