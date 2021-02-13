namespace Kaylumah.Ssg.Manager.Site.Service
{
    class RenderData
    {
        public string Content { get;set; }
        public BuildData Build { get;set; }
        public SiteData Site { get;set; } = new SiteData();
        public PageData Page { get;set; } = new PageData();
    }

    class BuildData
    {
        
    }

    class SiteData
    {
        public string Title { get;set; } = $"{nameof(SiteData)}{nameof(Title)}";
    }

    class PageData
    {
        public string Title { get;set; } = $"{nameof(PageData)}{nameof(Title)}";
    }
}