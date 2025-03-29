// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Ssg.Extensions.Metadata.Abstractions
{
    public abstract class PublicationMetaData : PageMetaData
    {
        public PublicationMetaData(Dictionary<string, object?> internalData) : base(internalData)
        {
        }
    }
}