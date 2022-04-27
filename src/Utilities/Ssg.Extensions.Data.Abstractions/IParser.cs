// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Ssg.Extensions.Data.Abstractions;

public interface IParser
{
    T Parse<T>(string raw);
}