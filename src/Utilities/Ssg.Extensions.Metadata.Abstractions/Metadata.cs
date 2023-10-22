// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Ssg.Extensions.Metadata.Abstractions
{
    public class Metadata<T>
    {
        public T Data { get; set; }
        public string Content { get; set; }
    }
}
