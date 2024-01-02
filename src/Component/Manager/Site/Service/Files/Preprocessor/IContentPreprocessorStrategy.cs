// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.IO.Abstractions;

namespace Kaylumah.Ssg.Manager.Site.Service.Files.Preprocessor
{
    public interface IContentPreprocessorStrategy
    {
        bool ShouldExecute(IFileSystemInfo fileInfo);
        string Execute(string raw);
    }
}
