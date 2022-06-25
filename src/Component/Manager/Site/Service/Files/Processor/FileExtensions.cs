// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Ssg.Engine.Transformation.Interface;

namespace Kaylumah.Ssg.Manager.Site.Service.Files.Processor;

public static class FileExtensions
{
    public static Dictionary<string, object> ToDictionary(this File file)
    {
        var result = new Dictionary<string, object>(file.MetaData);
        // result.SetValue(nameof(file.LastModified), file.LastModified);
        result.SetValue(nameof(file.Content), file.Content);
        result.SetValue(nameof(file.Name), file.Name);
        return result;
    }

    public static PageMetaData ToPage(this File file)
    {
        var data = file.ToDictionary();
        return new PageMetaData(data);
    }

    public static PageMetaData ToPage(this File file, Guid siteGuid)
    {
        var page = file.ToPage();
        page.Id = siteGuid.CreatePageGuid(file.MetaData.Uri).ToString();
        return page;
    }

    public static PageMetaData[] ToPages(this IEnumerable<File> files, Guid siteGuid)
    {
        return files.Select(x => ToPage(x, siteGuid)).ToArray();
    }
}
