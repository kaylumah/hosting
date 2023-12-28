// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Threading.Tasks;

namespace Kaylumah.Ssg.Manager.Site.Interface
{
    public interface ISiteManager
    {
        Task GenerateSite(GenerateSiteRequest request);
    }
}
