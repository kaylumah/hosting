using System;
using System.Threading.Tasks;
using Kaylumah.Ssg.Manager.Site.Interface;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    public class SiteManager : ISiteManager
    {
        public Task GenerateSite()
        {
            return Task.CompletedTask;
        }
    }
}
