using System.Threading.Tasks;

namespace Kaylumah.Ssg.Manager.Site.Interface
{
    public interface ISiteManager
    {
        Task GenerateSite(GenerateSiteRequest request);
    }
}
