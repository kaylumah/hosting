// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;

namespace Kaylumah.Ssg.Utilities.Files
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFileSystem(this IServiceCollection services)
        {
            services.AddSingleton<IFileSystem, FileSystem>();
            services.AddSingleton<System.IO.Abstractions.IFileSystem, System.IO.Abstractions.FileSystem>();
            return services;
        }
    }
}
