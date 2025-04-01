// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using Schema.NET;
using Ssg.Extensions.Metadata.Abstractions;

namespace Kaylumah.Ssg.Manager.Site.Service.Seo
{
    public interface ILdJsonRenderer
    {
        Thing ToLdJson(BasePage page);
    }
}