
// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Ssg.Manager.Site.Service.RenderEngine;
using Kaylumah.Ssg.Manager.Site.Service.Seo;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    public interface IRenderPlugin
    {
        bool ShouldExecute(RenderData renderData);

        void Apply(RenderData renderData);
    }

    public class HtmlSeoRenderPlugin : IRenderPlugin
    {
        private readonly SeoGenerator _seoGenerator;

        public HtmlSeoRenderPlugin(SeoGenerator seoGenerator)
        {
            _seoGenerator = seoGenerator;
        }

        public void Apply(RenderData renderData)
        {
            _seoGenerator.ApplySeo(renderData);
        }

        public bool ShouldExecute(RenderData renderData)
        {
            bool result = renderData.IsHtml();
            return result;
        }
    }
}
