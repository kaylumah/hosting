using System.Threading.Tasks;
using Kaylumah.Ssg.Manager.Site.Interface;
using Kaylumah.Ssg.Manager.Site.Service;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.FileProviders;
using Moq;
using Xunit;

namespace Test.Unit
{
    public class SiteManagerTests
    {
        [Fact]
        public async Task Test_SiteManager_GenerateSite()
        {
            var loggerMock = new Mock<ILogger<SiteManager>>();
            var fileProviderMock = new Mock<IFileProvider>();

            ISiteManager sut = new SiteManager(fileProviderMock.Object, loggerMock.Object);
            await sut.GenerateSite();
        }
    }
}