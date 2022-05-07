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
            var fileSystem = new System.IO.Abstractions.FileSystem();
            // fileSystem.Directory.SetCurrentDirectory(rootDirectory);


            services.AddSingleton<IFileProvider>(new PhysicalFileProvider(rootDirectory));
            services.AddSingleton<IFileSystem, FileSystem>();
            services.AddSingleton<System.IO.Abstractions.IFileSystem>(fileSystem);
            return services;
        }
    }
}
