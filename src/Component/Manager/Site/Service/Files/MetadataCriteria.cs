namespace Kaylumah.Ssg.Manager.Site.Service
{
    public class MetadataCriteria
    {
        public string FileName { get; set; }
        public string Content { get; set; }
        public string Permalink { get; set; }

        public MetadataCriteria()
        {
            // TODO
            Permalink = "/:year/:month/:day/:name:ext";
        }
    }
}