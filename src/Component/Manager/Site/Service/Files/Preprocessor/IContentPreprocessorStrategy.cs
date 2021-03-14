using Microsoft.Extensions.FileProviders;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    public interface IContentPreprocessorStrategy
    {
        bool ShouldExecute(IFileInfo fileInfo);
        string Execute(string raw);
    }
}
