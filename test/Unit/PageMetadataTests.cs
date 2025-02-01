// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

#pragma warning disable
using System;
using System.Collections.Generic;
using FluentAssertions;
using Ssg.Extensions.Metadata.Abstractions;
using Xunit;
using System.Globalization;

namespace Test.Unit
{
    /*

    public class DictionaryExtensionsTests
    {
        [Fact]
        public void GetBoolValue_ShouldReturn_True_WhenValidBooleanString_IgnoringCase()
        {
            // Arrange
            var dictionary = new Dictionary<string, object?>
            {
                { "Enabled".ToLower(CultureInfo.InvariantCulture), "true" }
            };

            // Act
            bool result = dictionary.GetBoolValue("ENABLED"); // Uppercase key

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void GetBoolValue_ShouldReturn_False_WhenInvalidBooleanString()
        {
            // Arrange
            var dictionary = new Dictionary<string, object?>
            {
                { "Enabled".ToLower(CultureInfo.InvariantCulture), "notabool" }
            };

            // Act
            bool result = dictionary.GetBoolValue("ENABLED");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void GetBoolValue_ShouldReturn_False_WhenKeyIsMissing()
        {
            // Arrange
            var dictionary = new Dictionary<string, object?>();

            // Act
            bool result = dictionary.GetBoolValue("MissingKey");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void GetValue_ShouldReturn_ExpectedValue_IgnoringCase()
        {
            // Arrange
            var dictionary = new Dictionary<string, object?>
            {
                { "Count".ToLower(CultureInfo.InvariantCulture), 42 }
            };

            // Act
            int count = dictionary.GetValue<int>("COUNT"); // Uppercase key

            // Assert
            count.Should().Be(42);
        }

        [Fact]
        public void GetValue_ShouldReturn_Default_WhenKeyIsMissing()
        {
            // Arrange
            var dictionary = new Dictionary<string, object?>();

            // Act
            int count = dictionary.GetValue<int>("MissingKey");

            // Assert
            count.Should().Be(default);
        }

        [Fact]
        public void GetStringValues_ShouldReturn_List_WhenKeyExists_IgnoringCase()
        {
            // Arrange
            var dictionary = new Dictionary<string, object?>
            {
                { "Tags".ToLower(CultureInfo.InvariantCulture), new List<string> { "tag1", "tag2" } }
            };

            // Act
            var tags = dictionary.GetStringValues("TAGS"); // Uppercase key

            // Assert
            tags.Should().Contain(new[] { "tag1", "tag2" });
        }

        [Fact]
        public void GetStringValues_ShouldReturn_EmptyList_WhenKeyIsMissing()
        {
            // Arrange
            var dictionary = new Dictionary<string, object?>();

            // Act
            var tags = dictionary.GetStringValues("MissingTags");

            // Assert
            tags.Should().BeEmpty();
        }
    }

    public class PageMetadataTests
    {
        [Fact]
        public void PageMetaData_ShouldInitialize_FromDictionary()
        {
            // Arrange
            var initialData = new Dictionary<string, object?>
            {
                { "id", "123" },
                { "Title", "Test Page" },
                { "Description", "This is a test description" },
                { "Language", "en" },
                { "Sitemap", true },
                { "Published", new DateTimeOffset(2023, 1, 1, 12, 0, 0, TimeSpan.Zero) },
                { "Modified", new DateTimeOffset(2024, 2, 15, 8, 0, 0, TimeSpan.Zero) },
                { "Tags", new List<string> { "test", "metadata" } }
            };

            // Act
            var page = new PageMetaData(initialData);

            // Assert
            page.Id.Should().Be(new PageId("123")); // Updated to use PageId struct
            page.Title.Should().Be("Test Page");
            page.Description.Should().Be("This is a test description");
            page.Language.Should().Be("en");
            page.Sitemap.Should().BeTrue();
            page.Published.Should().Be(new DateTimeOffset(2023, 1, 1, 12, 0, 0, TimeSpan.Zero));
            page.Modified.Should().Be(new DateTimeOffset(2024, 2, 15, 8, 0, 0, TimeSpan.Zero));
            page.Tags.Should().Contain(new[] { "test", "metadata" });
        }

        [Fact]
        public void PageMetaData_ShouldUpdate_MutableProperties()
        {
            // Arrange
            var page = new PageMetaData(new Dictionary<string, object?>());

            // Act
            page.Id = new PageId("456");
            page.Content = "Updated content";
            page.Tags = new List<string> { "updated", "test" };

            // Assert
            page.Id.Should().Be(new PageId("456"));
            page.Content.Should().Be("Updated content");
            page.Tags.Should().Contain(new[] { "updated", "test" });
        }

        [Fact]
        public void PageMetaData_ShouldHandle_MissingValuesGracefully()
        {
            // Arrange
            var page = new PageMetaData(new Dictionary<string, object?>());

            // Act & Assert
            page.Id.Should().Be(new PageId(null)); // Default to empty PageId
            page.Title.Should().BeNull();
            page.Sitemap.Should().BeFalse(); // Default to false if missing
            page.Published.Should().Be(default); // Default to DateTimeOffset.MinValue
            page.Modified.Should().Be(default); // Default to DateTimeOffset.MinValue
            page.Tags.Should().BeEmpty(); // Default empty list
        }

        [Fact]
        public void PageMetaData_ShouldCompute_CanonicalUri_Correctly()
        {
            // Arrange
            var initialData = new Dictionary<string, object?>
            {
                { "BaseUri", "https://example.com/" },
                { "Uri", "index.html" }
            };
            var page = new PageMetaData(initialData);

            // Act
            var canonicalUri = page.CanonicalUri;

            // Assert
            canonicalUri.Should().Be(new Uri("https://example.com/")); // Should resolve to base URI
        }

        [Fact]
        public void PageMetaData_ShouldResolve_WebImage_Correctly()
        {
            // Arrange
            var initialData = new Dictionary<string, object?>
            {
                { "BaseUri", "https://example.com/" },
                { "Image", "images/pic.jpg" }
            };
            var page = new PageMetaData(initialData);

            // Act
            var webImage = page.WebImage;

            // Assert
            webImage.Should().Be(new Uri("https://example.com/images/pic.jpg")); // Correctly resolves full image path
        }
    }*/
}