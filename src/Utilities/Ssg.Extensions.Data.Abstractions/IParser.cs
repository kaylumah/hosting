using System;

namespace Ssg.Extensions.Data.Abstractions
{
    public interface IParser
    {
        T Parse<T>(string raw);
    }
}
