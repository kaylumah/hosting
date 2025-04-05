// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Kaylumah.Ssg.Manager.Site.Service;
using Reqnroll;
using Reqnroll.Assist;

namespace Test.Unit.Steps
{
    [Binding]
    public class SiteInfoStepDefinitions
    {
        readonly SiteInfo _SiteInfo;

        public SiteInfoStepDefinitions(SiteInfo siteInfo)
        {
            _SiteInfo = siteInfo;
        }

        [Given("the following site info:")]
        public void GivenTheFollowingSiteInfo(Table table)
        {
            (string title, string description, string Language, string url, string baseUrl, string[] supportedFileExtensions) data = table.CreateInstance<(string title, string description, string Language, string url, string baseUrl, string[] supportedFileExtensions)>();
            _SiteInfo.Url = data.url;
            _SiteInfo.Title = data.title;
            _SiteInfo.Description = data.description;
            _SiteInfo.Lang = data.Language;
            _SiteInfo.SupportedFileExtensions = new HashSet<string>(data.supportedFileExtensions);
            _SiteInfo.SupportedDataFileExtensions = new HashSet<string>() { ".yml" };
        }
    }
}
