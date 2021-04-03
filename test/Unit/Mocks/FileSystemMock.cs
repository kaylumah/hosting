// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
using Kaylumah.Ssg.Utilities;
using Microsoft.Extensions.FileProviders;
using Moq;
using System.Collections;
using System.Collections.Generic;

namespace Test.Unit.Mocks
{
    public class FileSystemMock : Mock<IFileSystem>
    {
        public FileSystemMock()
        {
            // Setup(x => x.GetDirectoryContents(It.IsAny<string>())).Returns(new NotFoundDirectoryContents());


            var directoryContentsMock = new Mock<IDirectoryContents>();
            directoryContentsMock.Setup(dc => dc.GetEnumerator()).Returns(new List<IFileInfo>().GetEnumerator());
            directoryContentsMock.Setup(dc => dc.Exists).Returns(false);
            Setup(x => x.GetDirectoryContents(It.IsAny<string>())).Returns(directoryContentsMock.Object);
        }
    }
}