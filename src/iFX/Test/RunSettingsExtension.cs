// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Testing.Extensions.VSTestBridge.Helpers;
using Microsoft.Testing.Platform.Builder;
using Microsoft.Testing.Platform.Extensions;

namespace Kaylumah.Ssg.iFX.Test
{
    // Keep environment variables in .runsettings while Rider and the MTP command-line
    // runner do not consistently apply the same configuration files.
    public sealed class RunSettingsExtension : IExtension
    {
        public string Uid => "Kaylumah.Ssg.RunSettings";
        public string Version => "1.0.0";
        public string DisplayName => "Kaylumah runsettings support";
        public string Description => "Applies environment variables from .runsettings to the test host.";

        public Task<bool> IsEnabledAsync() => Task.FromResult(true);

        public static void AddExtensions(ITestApplicationBuilder builder, string[] _)
        {
            const string settingsVariable = "TESTINGPLATFORM_VSTESTBRIDGE_RUNSETTINGS_FILE";
            string? configuredSettingsPath = Environment.GetEnvironmentVariable(settingsVariable);
            if (string.IsNullOrEmpty(configuredSettingsPath))
            {
                Assembly? entryAssembly = Assembly.GetEntryAssembly();
                IEnumerable<AssemblyMetadataAttribute>? metadata = entryAssembly?.GetCustomAttributes<AssemblyMetadataAttribute>();
                AssemblyMetadataAttribute? settingsMetadata = metadata?.SingleOrDefault(attribute => attribute.Key == "RunSettingsFilePath");
                string? settingsPath = settingsMetadata?.Value;
                if (File.Exists(settingsPath))
                {
                    Environment.SetEnvironmentVariable(settingsVariable, settingsPath);
                }
            }

            RunSettingsExtension extension = new RunSettingsExtension();
            builder.AddRunSettingsService(extension);
            builder.AddRunSettingsEnvironmentVariableProvider(extension);
        }
    }
}
