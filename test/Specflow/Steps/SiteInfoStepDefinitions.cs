// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Ssg.Manager.Site.Service;

namespace Test.Specflow.Steps;

[Binding]
public class SiteInfoStepDefinitions
{
    private readonly SiteInfo _siteInfo;

    public SiteInfoStepDefinitions(SiteInfo siteInfo)
    {
        _siteInfo = siteInfo;
    }

    [Given("the following collections:")]
    public void GivenTheFollowingCollections(Table table)
    {
        var collections = table.CreateSet<Collection>();
        _siteInfo.Collections = new Collections();
        foreach (var collection in collections)
        {
            _siteInfo.Collections.Add(collection);
        }
    }
}
