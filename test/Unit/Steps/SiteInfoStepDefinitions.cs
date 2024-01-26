// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Kaylumah.Ssg.Manager.Site.Service;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

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

        [Given("the following collections:")]
        public void GivenTheFollowingCollections(Table table)
        {
            IEnumerable<Collection> collections = table.CreateSet<Collection>();
            _SiteInfo.Collections = new Kaylumah.Ssg.Manager.Site.Service.Collections();
            foreach (Collection collection in collections)
            {
                _SiteInfo.Collections.Add(collection);
            }
        }

        [Given("the following site info:")]
        public void GivenTheFollowingSiteInfo(Table table)
        {
            (string title, string description, string Language, string url, string baseUrl, string[] supportedFileExtensions) data = table.CreateInstance<(string title, string description, string Language, string url, string baseUrl, string[] supportedFileExtensions)>();
            _SiteInfo.Url = data.url;
            _SiteInfo.Title = data.title;
            _SiteInfo.Description = data.description;
            _SiteInfo.Lang = data.Language;
            _SiteInfo.BaseUrl = data.baseUrl;
            _SiteInfo.SupportedFileExtensions = new HashSet<string>(data.supportedFileExtensions);
            _SiteInfo.SupportedDataFileExtensions = new HashSet<string>() { ".yml" };
        }
    }
}
