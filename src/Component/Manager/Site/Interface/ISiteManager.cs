// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Kaylumah.Ssg.Manager.Site.Interface;

public interface ISiteManager
{
    Task GenerateSite(GenerateSiteRequest request);
}