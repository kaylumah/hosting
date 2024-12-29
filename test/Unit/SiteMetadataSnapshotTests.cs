// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Ssg.Extensions.Metadata.Abstractions;
using VerifyXunit;
using Xunit;

namespace Test.Unit
{
    public class SiteMetadataSnapshotTests
    {
        [Fact]
        public async Task Test1()
        {
            BuildData buildData = (BuildData) RuntimeHelpers.GetUninitializedObject(typeof(BuildData));
            
            SiteMetaData siteMetaData = new SiteMetaData("", "", "", "", "", "", new(), buildData, new());
            await Verifier.Verify(siteMetaData);
        }
        
        [Fact]
        public async Task Test2()
        {
            BuildData buildData = (BuildData) RuntimeHelpers.GetUninitializedObject(typeof(BuildData));
            
            TagMetaDataCollection tagMetaDataCollection = new();
            Dictionary<string, object> data = new() { { "tags", tagMetaDataCollection } };
            
            SiteMetaData siteMetaData = new SiteMetaData("", "", "", "", "", "", data, buildData, new());
            await Verifier.Verify(siteMetaData);
        }
        
        [Fact]
        public async Task Test3()
        {
            BuildData buildData = (BuildData) RuntimeHelpers.GetUninitializedObject(typeof(BuildData));
            
            TagMetaDataCollection tagMetaDataCollection = new();
            tagMetaDataCollection.Add(new TagMetaData() { Id = "1" });
            Dictionary<string, object> data = new() { { "tags", tagMetaDataCollection } };
            
            SiteMetaData siteMetaData = new SiteMetaData("", "", "", "", "", "", data, buildData, new());
            await Verifier.Verify(siteMetaData);
        }
        
        [Fact]
        public async Task Test4()
        {
            BuildData buildData = (BuildData) RuntimeHelpers.GetUninitializedObject(typeof(BuildData));
            
            List<BasePage> items = new();
            Dictionary<string, object?> pageData = new()
            {
                ["uri"] = "example.html",
                ["tags"] = new List<object> { "1" }
            };
            PageMetaData pageMetaData = new PageMetaData(pageData);
            items.Add(pageMetaData);
            
            SiteMetaData siteMetaData = new SiteMetaData("", "", "", "", "", "", new(), buildData, items);
            await Verifier.Verify(siteMetaData);
        }
    }
}