// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
using System;
using System.Linq;
using Kaylumah.Ssg.Engine.Transformation.Interface.Rendering;

namespace Kaylumah.Ssg.Manager.Site.Service.Files.Processor;

public static class FileExtensions
{
    public static PageData ToPage(this File file)
    {
        return new PageData(file.MetaData, file.Name, file.Content, file.LastModified);
    }

    public static PageData ToPage(this File file, Guid siteGuid)
    {
        var page = file.ToPage();
        page.Id = siteGuid.CreatePageGuid(file.MetaData.Uri).ToString();
        return page;
    }

    public static PageData[] ToPages(this File[] files, Guid siteGuid)
    {
        return files.Select(x => ToPage(x, siteGuid)).ToArray();
    }
}