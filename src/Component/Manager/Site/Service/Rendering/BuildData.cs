namespace Kaylumah.Ssg.Manager.Site.Service
{
    class BuildData
    {
        public string Version { get;set; }
        public string Copyright { get;set; }
        public string SourceBaseUri { get;set; }
        public string GitHash { get;set; }
        public string ShortGitHash { get;set; }
    }
}