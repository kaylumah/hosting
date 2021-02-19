using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    public interface IFileProcessor
    {
        Task<IEnumerable<File>> Process();
    }
}