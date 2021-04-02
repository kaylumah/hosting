// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
namespace Kaylumah.Ssg.Manager.Site.Service
{
    public interface IMetadata
    {
        string Title { get; }
        string Description { get; }
        string Language { get; }
        string Author { get; }
        string Url { get; }
    }
}