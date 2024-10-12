// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using Kaylumah.Ssg.Utilities;
using Ssg.Extensions.Metadata.Abstractions;

namespace Test.Unit.FormerXunit
{
    public class SiteTests
    {
        static SiteMetaData CreateSiteMetaData()
        {
            string id = string.Empty;
            string title = string.Empty;
            string description = string.Empty;
            string language = string.Empty;
            string author = string.Empty;
            string url = string.Empty;
            Dictionary<string,object> data = new();
            BuildData build = new();
            List<BasePage> items = new();
            SiteMetaData result = new SiteMetaData(id, title, description, language, author, url, data, build, items);
            return result;
        }

        static BuildData EnrichSiteWithAssemblyData()
        {
#pragma warning disable
            AssemblyInfo assemblyInfo = Assembly.GetExecutingAssembly().RetrieveAssemblyInfo();
            DateTimeOffset localNow = DateTimeOffset.Now;
            BuildData buildMetadata = new BuildData(assemblyInfo, localNow);
            return buildMetadata;
#pragma warning restore
        }
    }
}