// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Text;
using System.Xml;
using Kaylumah.Ssg.Extensions.Metadata.Abstractions;
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
        readonly MetaTagGenerator _MetaTagGenerator;
        readonly StructureDataGenerator _StructureDataGenerator;

        public HtmlSeoRenderPlugin(MetaTagGenerator metaTagGenerator, StructureDataGenerator structureDataGenerator)
        {
            _MetaTagGenerator = metaTagGenerator;
            _StructureDataGenerator = structureDataGenerator;
        }

        public void Apply(RenderData renderData)
        {
            if (renderData.Page is PageMetaData pageMetaData)
            {
                string ldJson = GenerateLdJson(renderData);
                string metaTags = _MetaTagGenerator.ToMetaTags(renderData);

                pageMetaData.LdJson = ldJson;
                pageMetaData.MetaTags = metaTags;
            }
        }

        public bool ShouldExecute(RenderData renderData)
        {
            bool result = renderData.IsHtml();
            return result;
        }

        string GenerateLdJson(RenderData renderData)
        {
            string json = _StructureDataGenerator.ToLdJson(renderData);
            if (!string.IsNullOrEmpty(json))
            {
                XmlDocument finalDocument = new XmlDocument();
                XmlElement scriptElement = finalDocument.CreateElement("script");
                XmlAttribute typeAttribute = finalDocument.CreateAttribute("type");
                typeAttribute.Value = "application/ld+json";
                scriptElement.Attributes.Append(typeAttribute);
                scriptElement.InnerText = json;

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<!-- LdJson Meta Tags -->");
                sb.Append(scriptElement.OuterXml);
                string result = sb.ToString();
                return result;
            }

            return string.Empty;
        }
    }
}
