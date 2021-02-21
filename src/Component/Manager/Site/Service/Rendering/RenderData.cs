using System.Collections.Generic;
using Kaylumah.Ssg.Utilities;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    class RenderData : IRenderModel
    {
        public BuildData Build { get;set; }
        public SiteData Site { get;set; } = new SiteData();
        public PageData Page { get;set; }
        public string Content => Page?.Content ?? string.Empty;
    }

    class SiteData 
    {
        public string Title { get; set; } = $"{nameof(SiteData)}{nameof(Title)}";
        public Dictionary<string, object> Data { get; set; }
        public Dictionary<string, object> Collections { get; set; }
    }

    class PageData : Dictionary<string, object>, IMetadata, IPageMetadata
    {
        public string Title { get; }
        public string Description { get; }
        public string Language { get; }
        public string Author { get; }
        public string Content { get; }

        public PageData(File file) : base(file.MetaData)
        {
            Content = file.Content;
        }
    }

    public interface IMetadata
    {
        string Title { get; }
        string Description { get; }
        string Language { get; }
        string Author { get; }
    }

    public interface IPageMetadata
    {
        string Content { get; }
    }
}