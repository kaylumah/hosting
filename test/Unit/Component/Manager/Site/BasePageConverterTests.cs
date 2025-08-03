// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using FluentAssertions;
using Kaylumah.Ssg.Extensions.Metadata.Abstractions;
using Kaylumah.Ssg.Manager.Site.Service;
using Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;
using Kaylumah.Ssg.Manager.Site.Service.Files.Processor;
using Xunit;

namespace Test.Unit.Component.Manager.Site
{
    public class BasePageConverterTests
    {
        readonly string _BaseUrl;
        readonly Guid _SiteGuid;

        public BasePageConverterTests()
        {
            _BaseUrl = "https://localhost";
            _SiteGuid = Guid.Empty;
        }

        [Fact]
        public void EmptyFileList()
        {
            TextFile[] files = [];
            List<BasePage> result = BasePageConverter.ToPageMetadata(files, _SiteGuid, _BaseUrl);
            result.Should().BeEmpty();
        }

        [Fact]
        public void EmptyFile()
        {
            FileMetaData fileMetaData = CreateFileMetaData();
            TextFile textFile = CreateTextFile(fileMetaData);
            TextFile[] files = [textFile];
            List<BasePage> result = BasePageConverter.ToPageMetadata(files, _SiteGuid, _BaseUrl);
            result.Should().BeEmpty();
        }
        
        [Fact]
        public void UnknownType()
        {
            FileMetaData fileMetaData = CreateFileMetaData("UnknownType");
            TextFile textFile = CreateTextFile(fileMetaData);
            TextFile[] files = [textFile];
            Action act = () => BasePageConverter.ToPageMetadata(files, _SiteGuid, _BaseUrl);
            act.Should().Throw<InvalidOperationException>();
        }

        FileMetaData CreateFileMetaData(string? type = null)
        {
            FileMetaData fileMetaData = new();
            if (type != null)
            {
                fileMetaData["type"] = type;
            }

            return fileMetaData;
        }

        TextFile CreateTextFile(FileMetaData fileMetaData, string? encoding = null, byte[]? contents = null)
        {
            if (contents == null)
            {
                contents = [];
            }

            if (encoding == null)
            {
                encoding = "UTF-8";
            }

            TextFile file = new TextFile(fileMetaData, contents, encoding);
            return file;
        }
    }
}