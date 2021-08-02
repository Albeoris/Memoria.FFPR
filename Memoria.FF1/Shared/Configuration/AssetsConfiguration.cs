using System;
using BepInEx.Configuration;
using UnityEngine;

namespace Memoria.FFPR.Configuration
{
    public sealed class AssetsConfiguration
    {
        private const String Section = "Assets";

        private ConfigEntry<Boolean> ExportEnabled { get; }
        private ConfigEntry<String> _exportDirectory { get; }

        private ConfigEntry<Boolean> ImportEnabled { get; }
        private ConfigEntry<String> _importDirectory { get; }

        public AssetsConfiguration(ConfigFile file)
        {
            ExportEnabled = file.Bind(Section, nameof(ExportEnabled), false,
                $"Export the supported resources to the {nameof(ExportDirectory)}.");

            _exportDirectory = file.Bind(Section, nameof(ExportDirectory), "%StreamingAssets%",
                $"Directory into which the supported resources will be exported.",
                new AcceptableDirectoryPath(nameof(ExportDirectory)));

            ImportEnabled = file.Bind(Section, nameof(ImportEnabled), true,
                $"Import the supported resources from the {nameof(ImportDirectory)}.");

            _importDirectory = file.Bind(Section, nameof(ImportDirectory), "%StreamingAssets%",
                $"Directory from which the supported resources will be imported.",
                new AcceptableDirectoryPath(nameof(ImportDirectory)));
        }

        public String ExportDirectory => ExportEnabled.Value
            ? AcceptableDirectoryPath.Preprocess(_exportDirectory.Value)
            : String.Empty;

        public String ImportDirectory => ImportEnabled.Value
            ? AcceptableDirectoryPath.Preprocess(_importDirectory.Value)
            : String.Empty;
    }
}