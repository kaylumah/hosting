// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace Kaylumah.Ssg.Utilities.Files
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFileSystem(this IServiceCollection services, string rootDirectory)
        {
            services.AddSingleton<IFileProvider>(new PhysicalFileProvider(rootDirectory));
            services.AddSingleton<IFileSystem, FileSystem>();
            return services;
        }
    }
}
