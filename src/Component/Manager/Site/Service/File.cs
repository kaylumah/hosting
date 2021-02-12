namespace Kaylumah.Ssg.Manager.Site.Service
{
    public class File<TMetadata>
    {
        public string Encoding { get;set; }
        public string Name { get;set; }
        public string Path { get;set; }
        public string Content { get;set; }
        public TMetadata Data { get;set; }
    }
}