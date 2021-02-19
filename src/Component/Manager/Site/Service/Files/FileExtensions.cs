using System.Collections.Generic;
using System.Linq;
using Kaylumah.Ssg.Utilities;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    public static class FileExtensions
    {
        public static RenderRequest ToRenderRequest(this File file)
        {
            return new RenderRequest() {
                TemplateName = file.MetaData?.Layout
            };
        }

        public static RenderRequest[] ToRenderRequests(this IEnumerable<File> fileModels)
        {
            return fileModels.Select(ToRenderRequest).ToArray();
        }
    }
}