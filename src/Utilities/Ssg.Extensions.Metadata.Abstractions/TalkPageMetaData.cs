// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Ssg.Extensions.Metadata.Abstractions
{
    public class TalkPageMetaData : PublicationPageMetaData
    {
        public Uri PresentationUri => GetPresentationUri();

        public string Location => GetString(nameof(Location));

        public DateTime EventDate => GetDateTimeValue(nameof(EventDate));

        public TalkPageMetaData(Dictionary<string, object?> internalData) : base(internalData)
        {
        }

        Uri GetPresentationUri()
        {
            string presentationUri = GetString(nameof(PresentationUri));
            Uri result = new Uri(presentationUri);
            return result;
        }
    }
}