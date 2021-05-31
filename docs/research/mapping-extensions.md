 # Mapping Code via Extensions
 
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