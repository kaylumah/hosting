// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
namespace Kaylumah.Ssg.Manager.Site.Service
{
    public class MetadataCriteria
    {
        public string Scope { get; set; }
        public string FileName { get; set; }
        public string Content { get; set; }
        public string Permalink { get; set; }

        public MetadataCriteria()
        {
            // TODO
            Permalink = "/:year/:month/:day/:name:ext";
        }
    }
}