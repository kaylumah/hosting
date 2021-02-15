using System.Threading.Tasks;
using Kaylumah.Ssg.Manager.Site.Interface;
using Kaylumah.Ssg.Manager.Site.Service;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.FileProviders;
using Moq;
using Xunit;
using System.Collections.Generic;
using Kaylumah.Ssg.Access.Artifact.Interface;
using Kaylumah.Ssg.Utilities;

namespace Test.Unit
{
    public class SiteManagerTests
    {
        [Fact]
        public async Task Test_SiteManager_GenerateSite()
        {
            var directoryContentsMock = new Mock<IDirectoryContents>();
            directoryContentsMock.Setup(dc => dc.GetEnumerator()).Returns(new List<IFileInfo>().GetEnumerator());
            directoryContentsMock.Setup(dc => dc.Exists).Returns(false);
            var loggerMock = new Mock<ILogger<SiteManager>>();
            var fileProviderMock = new Mock<IFileProvider>();
            fileProviderMock.Setup(x => x.GetDirectoryContents(It.IsAny<string>())).Returns(directoryContentsMock.Object);
            var fileProvider = fileProviderMock.Object;
            var fileSystem = new FileSystem(fileProvider);
            var artifactAccessMock = new Mock<IArtifactAccess>();
            ISiteManager sut = new SiteManager(artifactAccessMock.Object, fileSystem, fileProvider, loggerMock.Object);
            await sut.GenerateSite();
        }
    }
}