// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using Kaylumah.Ssg.Utilities;

namespace Kaylumah.Ssg.Manager.Site.Service.Files.Processor
{
    public static class FileExtensions
    {
        public static RenderRequest ToRenderRequest(this File file, BuildData buildData, SiteData siteData, Guid siteGuid)
        {
            return new RenderRequest {
                Model = new RenderData() {
                    Build = buildData,
                    Site = siteData,
                    Page = new PageData(file) {
                        Id = siteGuid.CreatePageGuid(file.MetaData.Uri).ToString()
                    }
                },
                TemplateName = file.MetaData.Layout
            };
        }

        public static RenderRequest[] ToRenderRequests(this IEnumerable<File> files, BuildData buildData, SiteData siteData, Guid siteGuid)
        {
            return files.Select(x => ToRenderRequest(x, buildData, siteData, siteGuid)).ToArray();
        }
    }
}