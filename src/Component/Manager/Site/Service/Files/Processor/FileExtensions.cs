// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Linq;
using Kaylumah.Ssg.Utilities;

namespace Kaylumah.Ssg.Manager.Site.Service.Files.Processor
{
    public static class FileExtensions
    {
        public static RenderRequest ToRenderRequest(this File file)
        {
            return new RenderRequest()
            {
                TemplateName = file.MetaData?.Layout,
                Model = null
            };
        }

        public static RenderRequest[] ToRenderRequests(this IEnumerable<File> fileModels)
        {
            return fileModels.Select(ToRenderRequest).ToArray();
        }
    }
}