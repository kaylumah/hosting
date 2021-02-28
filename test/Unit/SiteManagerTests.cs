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
using Microsoft.Extensions.Options;

namespace Test.Unit
{
    // public class SiteManagerTests
    // {
    //     [Fact(Skip = "wip")]
    //     public async Task Test_SiteManager_GenerateSite()
    //     {
    //         var configurationMock = Options.Create<SiteInfo>(new SiteInfo { });

    //         var directoryContentsMock = new Mock<IDirectoryContents>();
    //         directoryContentsMock.Setup(dc => dc.GetEnumerator()).Returns(new List<IFileInfo>().GetEnumerator());
    //         directoryContentsMock.Setup(dc => dc.Exists).Returns(false);
    //         var loggerMock = new Mock<ILogger<SiteManager>>();
    //         var fileProviderMock = new Mock<IFileProvider>();
    //         fileProviderMock.Setup(x => x.GetDirectoryContents(It.IsAny<string>())).Returns(directoryContentsMock.Object);
    //         var fileProvider = fileProviderMock.Object;
    //         var fileSystem = new FileSystem(fileProvider);
    //         var artifactAccessMock = new Mock<IArtifactAccess>();

    //         var fileMetadataParserMock = new Mock<IFileMetadataParser>().Object;
    //         IFileProcessor fileProcessor = new FileProcessor(fileSystem, new Mock<ILogger<FileProcessor>>().Object, new IContentPreprocessorStrategy[] { }, configurationMock, fileMetadataParserMock);
    //         ISiteManager sut = new SiteManager(fileProcessor, artifactAccessMock.Object, fileSystem, loggerMock.Object);
    //         await sut.GenerateSite(new GenerateSiteRequest {
    //             Configuration = new SiteConfiguration {}
    //         });
    //     }
    // }
}