using Microsoft.Extensions.FileProviders;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    public class FileData
    {
        public string Layout { get;set; }
        public string Title { get;set; }
        public string Description { get;set; }
        public string Uri { get;set; }
        public string Image { get;set; }
        public string Type { get;set; }
    }

    public class CollectionLoader
    {
        private readonly IFileProvider _fileProvider;
        public CollectionLoader(IFileProvider fileProvider)
        {
            _fileProvider = fileProvider;
        }
    }
}