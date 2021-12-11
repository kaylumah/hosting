// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
using System.Runtime.Serialization;

namespace Kaylumah.Ssg.Manager.Site.Interface;

[DataContract]
public class GenerateSiteRequest
{
    [DataMember]
    public SiteConfiguration Configuration { get; set; }
}