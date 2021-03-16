// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
using System;

namespace Ssg.Extensions.Metadata.Abstractions
{
    public interface IMetadataProvider
    {
        Metadata<T> Retrieve<T>(string contents);
    }
}
