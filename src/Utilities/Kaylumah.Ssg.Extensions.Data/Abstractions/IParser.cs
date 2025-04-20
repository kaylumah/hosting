// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Kaylumah.Ssg.Extensions.Data.Abstractions
{
    public interface IParser
    {
        T Parse<T>(string raw);
    }

    public interface ICollectionParser
    {
        T[] Parse<T>(string raw);
    }
}
