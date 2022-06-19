// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

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
        renderData.Page.LdJson = _structureDataGenerator.ToLdJson(renderData);
        renderData.Page.MetaTags = _metaTagGenerator.ToMetaTags(renderData);
    }
}
