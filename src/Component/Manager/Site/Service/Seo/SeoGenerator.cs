// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Text;
using System.Xml;
using Kaylumah.Ssg.Manager.Site.Service.RenderEngine;
namespace Kaylumah.Ssg.Manager.Site.Service.Seo
{
    public partial class SeoGenerator
    {
        readonly MetaTagGenerator _MetaTagGenerator;
        readonly StructureDataGenerator _StructureDataGenerator;

        public SeoGenerator(MetaTagGenerator metaTagGenerator, StructureDataGenerator structureDataGenerator)
        {
            _MetaTagGenerator = metaTagGenerator;
            _StructureDataGenerator = structureDataGenerator;
        }

        public void ApplySeo(RenderData renderData)
        {
            renderData.Page!.LdJson = GenerateLdJson(renderData);
            renderData.Page.MetaTags = _MetaTagGenerator.ToMetaTags(renderData);
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
                return sb.ToString();
            }

            return string.Empty;
        }
    }
}
