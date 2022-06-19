// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Text;
using System.Xml;
using Kaylumah.Ssg.Engine.Transformation.Interface;

namespace Kaylumah.Ssg.Manager.Site.Service.Seo;

public partial class SeoGenerator
{
    private readonly MetaTagGenerator _metaTagGenerator;
    private readonly StructureDataGenerator _structureDataGenerator;

    public SeoGenerator(MetaTagGenerator metaTagGenerator, StructureDataGenerator structureDataGenerator)
    {
        _metaTagGenerator = metaTagGenerator;
        _structureDataGenerator = structureDataGenerator;
    }

    public void ApplySeo(RenderData renderData)
    {
        renderData.Page.LdJson = GenerateLdJson(renderData);
        renderData.Page.MetaTags = _metaTagGenerator.ToMetaTags(renderData);
    }

    private string GenerateLdJson(RenderData renderData)
    {
        var json = _structureDataGenerator.ToLdJson(renderData);
        var finalDocument = new XmlDocument();
        var scriptElement = finalDocument.CreateElement("script");
        var typeAttribute = finalDocument.CreateAttribute("type");
        typeAttribute.Value = "application/ld+json";
        scriptElement.Attributes.Append(typeAttribute);
        scriptElement.InnerText = json;

        var sb = new StringBuilder();
        sb.AppendLine("<!-- LdJson Meta Tags -->");
        sb.Append(scriptElement.OuterXml);
        return sb.ToString();
    }
}
