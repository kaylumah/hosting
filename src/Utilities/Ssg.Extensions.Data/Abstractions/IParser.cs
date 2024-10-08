﻿// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Ssg.Extensions.Data.Abstractions
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
