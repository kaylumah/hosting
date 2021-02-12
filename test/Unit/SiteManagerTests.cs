using System.Threading.Tasks;
using Kaylumah.Ssg.Manager.Site.Interface;
using Kaylumah.Ssg.Manager.Site.Service;
using Xunit;

namespace Test.Unit
{
    public class SiteManagerTests
    {
        [Fact]
        public async Task Test_SiteManager_GenerateSite()
        {
            ISiteManager sut = new SiteManager();
            await sut.GenerateSite();
        }
    }
}