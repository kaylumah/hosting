// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
using Kaylumah.Ssg.Manager.Site.Service.Rendering;
namespace Kaylumah.Ssg.Manager.Site.Service.Files.Processor
{
    public static class FileExtensions
    {
        public static PageData ToPage(this File file)
        {
            return new PageData(file.MetaData, file.Content, file.LastModified);
        }
    }
}